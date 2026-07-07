using AutoMapper;
using ECommerce.Application.Common;
using ECommerce.Application.Contracts;
using ECommerce.Application.DTO_s.Baskets;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities.Baskets;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Services
{
    public class BasketServices : IBasketServices
    {
        private readonly IBasketRepository basketRepository;
        private readonly IMapper mapper;

        public BasketServices(IBasketRepository basketRepository, IMapper mapper)
        {
            this.basketRepository = basketRepository;
            this.mapper = mapper;
        }
        public async Task<Result<BasketDto>> CreateOrUpdateBasketAsync(BasketDto basket, CancellationToken ct = default)
        {
            var CustomerBasket = mapper.Map<CustomerBasket>(basket);
            var Result = await basketRepository.CreateOrUpdateBasketAsync(CustomerBasket, TimeSpan.FromDays(1), ct);
            return Result is not null ? Result<BasketDto>.Ok(mapper.Map<BasketDto>(Result)) : Result<BasketDto>.Fail(Error.Failure("CreateOrUpdateBasket.Failure", "Can not set this basket"));
        }

        public async Task<Result<bool>> DeleteBasketAsync(string id, CancellationToken ct = default)
        {
            var Result = await basketRepository.DeleteBasketAsync(id, ct);
            return Result ? Result<bool>.Ok(true) : Result<bool>.Fail(Error.Failure("DeleteBasket.Failure", "Can not delete this basket"));
        }

        public async Task<Result<BasketDto>> GetBasketAsync(string id, CancellationToken ct = default)
        {
            var basket = await basketRepository.GetBasketAsync(id, ct);

            if (basket is null)
                return Result<BasketDto>.Fail(Error.NotFound("GetBasket.NotFound", "Basket not found"));

            return Result<BasketDto>.Ok(mapper.Map<BasketDto>(basket));

        }
    }
}
