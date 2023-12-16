using AutoMapper;
using Talabat.API.Dtos;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;
using UserAddress = Talabat.Core.Entities.Identity.Address;
using OrderAddress = Talabat.Core.Entities.Order_Aggregate.Address;

namespace Talabat.API.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles() 
        {
            CreateMap<Product, ProductToReturnDtos>()
                .ForMember(d => d.Brand, o => o.MapFrom(s => s.Brand.Name))
                .ForMember(d => d.Category, o => o.MapFrom(s => s.Category.Name))
                .ForMember(d => d.PictureUrl, o => o.MapFrom<ProductPictureUrlResolver>());

            CreateMap<CustomerBasketDto, CustomerBasket>();
            CreateMap<BasketItemDto, BasketItem>();
            CreateMap<AddressDto, OrderAddress>();
            CreateMap<UserAddress, AddressDto>().ReverseMap();

            CreateMap<Order, OrderToReturnDto>()
                .ForMember(d => d.DeliveryMethod, o => o.MapFrom(s => s.DeliveryMethod.ShortName))
                .ForMember(d => d.DeliveryMethodCost, o => o.MapFrom(s => s.DeliveryMethod.Cost));

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(d => d.ProductId, o => o.MapFrom(s => s.Product.ProductId))
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.ProductName))
                .ForMember(d => d.PictureUrl, o => o.MapFrom(s => s.Product.PictureUrl))
                .ForMember(d => d.PictureUrl, o => o.MapFrom<OrderItemPictureUrlResolver>());

        }
    }
}
