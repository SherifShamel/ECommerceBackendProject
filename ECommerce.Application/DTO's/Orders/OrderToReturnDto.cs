using ECommerce.Application.DTO_s.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.DTO_s.Orders
{
    public class OrderToReturnDto
    {
        public Guid Id { get; set; }
        public string BuyerEmail { get; set; } = default!;
        public DateTime OrderDate { get; set; }
        public ICollection<OrderItemDto> orderItems { get; set; } = [];
        public AddressDto ShippingAddress { get; set; } = default!;

        public string DeliveryMethod { get; set; } = default!;
        public string Status { get; set; } = default!;

        public decimal SubTotal { get; set; } = default!;
        public decimal DeliveryCost { get; set; }
        public decimal Total { get; set; }
    }
}
