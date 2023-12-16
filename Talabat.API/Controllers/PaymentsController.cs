using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Talabat.API.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Services.Contract;

namespace Talabat.API.Controllers
{
    public class PaymentsController : ApiBaseController
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentsController> _logger;
        private const string _whSecret = "whsec_aeab7c412a6d40f7b8c791a72fb79d92bdc643c7404a88d5956ecae1d26a77e5";

        public PaymentsController(IPaymentService paymentService , ILogger<PaymentsController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        [Authorize]
        [ProducesResponseType(typeof(CustomerBasket) , StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [HttpPost("{basketid}")]
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentIntent(string basketId)
        {
            var basket = await _paymentService.CreateOrUpdatePaymentIntent(basketId);
            if (basket == null) return BadRequest(new ApiResponse(400, "An Error With Your Basket"));
            return Ok(basket);
        }


        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            
                var stripeEvent = EventUtility.ConstructEvent(json,
                    Request.Headers["Stripe-Signature"], _whSecret);

                var paymentIntent = (PaymentIntent) stripeEvent.Data.Object;
                Order order;
                switch(stripeEvent.Type)
                {
                    case Events.PaymentIntentSucceeded:
                        order = await _paymentService.UpdatePaymentIntentToSucceededOrFailed(paymentIntent.Id, true);
                        _logger.LogInformation("Payment is Succeeded :)" , paymentIntent.Id);
                        break;
                    case Events.PaymentIntentPaymentFailed:
                        order = await _paymentService.UpdatePaymentIntentToSucceededOrFailed(paymentIntent.Id, false);
                        _logger.LogInformation("Payment is Failed :(", paymentIntent.Id);
                        break;
                    
                }

                return Ok();
            
        }

    }
}
