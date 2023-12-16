using Microsoft.AspNetCore.Mvc;
using Talabat.API.Errors;
using Talabat.API.Helpers;
using Talabat.Core;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Services.Contract;
using Talabat.Repository;
using Talabat.Services;

namespace Talabat.API.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            //services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddAutoMapper(typeof(MappingProfiles));
            services.AddSingleton(typeof(IResponseCacheServices), typeof(ResponseCacheServices));
            #region Handle Validation Error Response
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    var errors = actionContext.ModelState.Where(P => P.Value.Errors.Count() > 0)
                                                         .SelectMany(P => P.Value.Errors)
                                                         .Select(E => E.ErrorMessage)
                                                         .ToArray();

                    var ValidationErrorResponse = new ApiValidationErrorResponse()
                    {
                        Errors = errors
                    };
                    return new BadRequestObjectResult(ValidationErrorResponse);
                };
            });
            #endregion

            services.AddScoped(typeof(IPaymentService), typeof(PaymentService));
            services.AddScoped(typeof(IProductService), typeof(ProductService));
            services.AddScoped(typeof(IBasketRepository), typeof(BasketRepository));
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfwork));
            services.AddScoped(typeof(IOrderService), typeof(OrderService));
            return services;
        }
    }
}
