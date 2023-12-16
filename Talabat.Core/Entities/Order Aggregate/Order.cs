using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entities.Order_Aggregate
{
    public class Order : BaseEntity
    {
        public Order() 
        { }

        public Order(string buyerEmail, Address shippingAddress, DeliveryMethod deliveryMethod, ICollection<OrderItem> items, decimal subTotal, string paymentIntentId)
        {
            BuyerEmail = buyerEmail;
            ShippingAddress = shippingAddress;
            DeliveryMethod = deliveryMethod;
            Items = items;
            SubTotal = subTotal;
            PaymentIntentId = paymentIntentId;
        }

        public string BuyerEmail { get; set; }
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.UtcNow;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public Address ShippingAddress { get; set; }  // Map In Owner
        public DeliveryMethod? DeliveryMethod { get; set; } // Navigation property [ONE]
        public ICollection<OrderItem> Items { get; set; } = new HashSet<OrderItem>();// Navigation property [Many]
        public decimal SubTotal { get; set; }
        public decimal GetTotal()
            => SubTotal + DeliveryMethod.Cost;
        public string PaymentIntentId { get; set; } 

    }
}
