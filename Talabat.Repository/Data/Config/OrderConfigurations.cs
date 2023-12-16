using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.Repository.Data.Config
{
    internal class OrderConfigurations : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.OwnsOne(order => order.ShippingAddress, ShippingAddress => ShippingAddress.WithOwner());
            builder.HasOne(order => order.DeliveryMethod)
                .WithMany()
                .OnDelete(DeleteBehavior.SetNull);
            builder.Property(order => order.Status)
                .HasConversion(
                    orderStatus => orderStatus.ToString(),
                    orderStatus => (OrderStatus) Enum.Parse(typeof(OrderStatus), orderStatus)
                );
            builder.Property(order => order.SubTotal)
                .HasColumnType("decimal(18, 2)");
        }
    }
}
