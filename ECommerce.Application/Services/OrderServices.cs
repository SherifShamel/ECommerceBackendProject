using AutoMapper;
using ECommerce.Application.Common;
using ECommerce.Application.Contracts;
using ECommerce.Application.DTO_s.Orders;
using ECommerce.Application.Specifications;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities.Orders;
using ECommerce.Domain.Entities.Products;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Services
{
    public class OrderServices : IOrderServices
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly IBasketRepository basketRepository;

        public OrderServices(IMapper mapper, IUnitOfWork unitOfWork, IBasketRepository basketRepository)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.basketRepository = basketRepository;
        }

        public async Task<Result<OrderToReturnDto>> CreateOrderAsync(OrderDto orderDto, string email, CancellationToken ct = default)
        {
            var basket = await basketRepository.GetBasketAsync(orderDto.BasketId, ct);

            if (basket is null)
                return Result<OrderToReturnDto>.Fail(Error.NotFound("Basket.NotFound", "Basket Is Not Found"));

            if (basket.Items.Count == 0)
                return Result<OrderToReturnDto>.Fail(Error.Validation("Basket.Empty", "Basket Is Empty"));

            //--------------------------------------------------------------------------------------------------------------------------

            var productRepo = unitOfWork.GetRepository<Product, int>();
            var productIds = basket.Items.Select(i => i.Id).ToHashSet();
            var Products = await productRepo.GetAllWithSpecificationsAsync(new ProductWithIdsSpecifications(productIds), ct);

            var OrderItems = new List<OrderItem>(basket.Items.Count);

            foreach (var item in basket.Items)
            {
                var product = Products.FirstOrDefault(p => p.Id == item.Id);

                if (product is null)
                    return Result<OrderToReturnDto>.Fail(Error.NotFound("Product.NotFound", "Product Not Found"));

                OrderItems.Add(new OrderItem()
                {
                    Price = product.Price,
                    Quantity = item.Quantity,
                    Product = new ProductItemOrder()
                    {
                        ProductId = product.Id,
                        PictureUrl = product.PictureUrl,
                        ProductName = product.Name
                    }
                });

            }

            //------------------------------------------------------------------------------------------------------------
            var orderAddress = mapper.Map<OrderAddress>(orderDto.ShippingAddress);

            //------------------------------------------------------------------------------------------------------------
            var deliveryMethod = await unitOfWork.GetRepository<DeliveryMethod, int>().GetById(orderDto.DeliveryMethodId, ct);

            if (deliveryMethod is null)
                return Result<OrderToReturnDto>.Fail(Error.NotFound("Basket.NotFound", "Delivery Method Is Not Found"));

            //------------------------------------------------------------------------------------------------------------
            var SubTotal = OrderItems.Sum(i => i.Price * i.Quantity);

            var order = new Order()
            {
                BuyerEmail = email,
                Items = OrderItems,
                ShippingAddress = orderAddress,
                DeliveryMethodId = deliveryMethod.Id,
                Subtotal = SubTotal,
                DeliveryMethod = deliveryMethod
            };

            unitOfWork.GetRepository<Order, Guid>().Add(order);
            var result = await unitOfWork.SaveChangesAsync();

            if (result <= 0)
                return Result<OrderToReturnDto>.Fail(Error.Failure("Order.Failure", "Failed to place Order"));

            await basketRepository.DeleteBasketAsync(orderDto.BasketId, ct);

            return Result<OrderToReturnDto>.Ok(mapper.Map<OrderToReturnDto>(order));
        }

        public async Task<Result<IReadOnlyList<DeliveryMethodDto>>> GetAllDeliveryMethodsAsync(CancellationToken ct = default)
        {
            var DeliveryMethods = await unitOfWork.GetRepository<DeliveryMethod, int>().GetAllAsync(ct);
            return Result<IReadOnlyList<DeliveryMethodDto>>.Ok(mapper.Map<IReadOnlyList<DeliveryMethodDto>>(DeliveryMethods));
        }

        public async Task<Result<IReadOnlyList<OrderToReturnDto>>> GetAllOrdersByEmailAsync(string email)
        {
            var Orders = await unitOfWork.GetRepository<Order, Guid>().GetAllWithSpecificationsAsync(new OrderSpecifications(email));
            return Result<IReadOnlyList<OrderToReturnDto>>.Ok(mapper.Map<IReadOnlyList<OrderToReturnDto>>(Orders));

        }

        public async Task<Result<OrderToReturnDto>> GetOrderByIdAndEmailAsync(Guid id, string email, CancellationToken ct = default)
        {
            var Order = await unitOfWork.GetRepository<Order, Guid>().GetByIdWithSpecificationAsync(new OrderSpecifications(id, email));

            if (Order is null)
                return Result<OrderToReturnDto>.Fail(Error.NotFound("Order.NotFound", "Order Is Not Found"));

            return Result<OrderToReturnDto>.Ok(mapper.Map<OrderToReturnDto>(Order));
        }
    }
}
