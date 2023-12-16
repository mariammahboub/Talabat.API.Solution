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

namespace Talabat.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBasketRepository _basketRepository;
        private readonly IPaymentService _paymentService;

        public OrderService(IUnitOfWork unitOfWork,IBasketRepository basketRepository,IPaymentService paymentService)
        {
            _unitOfWork = unitOfWork;
            _basketRepository = basketRepository;
            _paymentService = paymentService;
        }
        public async Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int deliveryMethodId, Address shippingAddress)
        {
            //1. Get Basket from Baskets Repo
            var basket = await _basketRepository.GetBasketAsync(basketId);

            //2. Get Selected Items at Basket From Products Repo
            var orderItems = new List<OrderItem>();
            if(basket?.Items?.Count > 0)
            {
                var productRepo = _unitOfWork.Repository<Product>();
                foreach (var item in basket.Items)
                {
                    var product = await productRepo.GetAsync(item.Id);
                    var productItemOrder = new ProductItemOrder(item.Id, product.Name, product.PictureUrl);
                    var orderItem = new OrderItem(product.Price, item.Quantity, productItemOrder);
                    orderItems.Add(orderItem);
                }
            }

            //3. Calculate subtotal
            var subTotal = orderItems.Sum(orderItem => orderItem.Price * orderItem.Quantity);

            //4. Get Delivery Method From DeliveryMethods repo
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetAsync(deliveryMethodId);

            var orderRepo = _unitOfWork.Repository<Order>();
            var orderSpec = new OrderWithPaymentIntentSpecification(basket.PaymentIntentId);
            var existingOrder = await orderRepo.GetWithSpecAsync(orderSpec);
            if(existingOrder != null)
            {
                orderRepo.Delete(existingOrder);
                await _paymentService.CreateOrUpdatePaymentIntent(basketId);
            }


            //5. Create order
            var order = new Order(buyerEmail, shippingAddress,deliveryMethod,orderItems,subTotal,basket.PaymentIntentId);
            await orderRepo.AddAsync(order);

            //6. Save to database [TODO]
            var result = await _unitOfWork.CompleteAsync();
            if (result <= 0) return null;
            return order;
        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
            => await _unitOfWork.Repository<DeliveryMethod>().GetAllAsync();
        

        public Task<Order?> GetOrderByIdForUserAsync(int orderId, string buyerEmail)
        {
            var orderRepo = _unitOfWork.Repository<Order>();
            var spec = new OrderSpecification(orderId, buyerEmail);
            var order = orderRepo.GetWithSpecAsync(spec);
            return order;

        }

        public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
        {
            var orderRepo = _unitOfWork.Repository<Order>();
            var spec = new OrderSpecification(buyerEmail);
            var orders = await orderRepo.GetAllWithSpecAsync(spec);
            return orders;
        }
    }
}
