using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Services.Contract;
using Talabat.Core.Specifications.Order_Specs;
using Product = Talabat.Core.Entities.Product;

namespace Talabat.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBasketRepository _basketRepository;
        private readonly IConfiguration _configuration;

        public PaymentService(
            IUnitOfWork unitOfWork,
            IBasketRepository basketRepository,
            IConfiguration configuration
            )
        {
            _unitOfWork = unitOfWork;
            _basketRepository = basketRepository;
            _configuration = configuration;
        }
        public async Task<CustomerBasket?> CreateOrUpdatePaymentIntent(string basketId)
        {
            StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"];
            var basket = await _basketRepository.GetBasketAsync( basketId );
            if ( basket == null ) return null;
            var shippingPrice = 0m;
            if(basket.DeliveryMethodId.HasValue)
            {
                var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetAsync(basket.DeliveryMethodId.Value);
                basket.ShippingPrice = deliveryMethod.Cost;
                shippingPrice = deliveryMethod.Cost;
            }

            if (basket?.Items.Count > 0)
            {
                foreach ( var item in basket.Items ) 
                {
                    var product = await _unitOfWork.Repository<Product>().GetAsync(item.Id);
                    if(item.Price != product.Price)
                        item.Price = product.Price;
                }
            }

            PaymentIntentService paymentIntentService = new PaymentIntentService();
            PaymentIntent paymentIntent;

            if(string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                var createOptions = new PaymentIntentCreateOptions()
                {
                    Amount = (long)basket.Items.Sum(item => item.Price * 100 * item.Quantity) + (long)shippingPrice * 100,
                    Currency = "usd",
                    PaymentMethodTypes = new List<string>() { "card" }
                };

                paymentIntent = await paymentIntentService.CreateAsync(createOptions);
                basket.PaymentIntentId = paymentIntent.Id;
                basket.ClientSecret = paymentIntent.ClientSecret;
            }
            else
            {
                var updateOptions = new PaymentIntentUpdateOptions()
                {
                    Amount = (long)basket.Items.Sum(item => item.Price * 100 * item.Quantity) + (long)shippingPrice * 100,
                };

                await paymentIntentService.UpdateAsync(basket.PaymentIntentId, updateOptions);
            }

            await _basketRepository.CreateOrUpdateBasketAsync(basket);
            return basket;
        }

        public async Task<Order> UpdatePaymentIntentToSucceededOrFailed(string paymentIntentId, bool isSucceeded)
        {
            var spec = new OrderWithPaymentIntentSpecification(paymentIntentId);
            var order = await _unitOfWork.Repository<Order>().GetWithSpecAsync(spec);
            if (isSucceeded)
                order.Status = OrderStatus.PaymentRecieved;
            else
                order.Status = OrderStatus.PaymentFailed;

            _unitOfWork.Repository<Order>().Update(order);
            await _unitOfWork.CompleteAsync();
            return order;


        }
    }
}
