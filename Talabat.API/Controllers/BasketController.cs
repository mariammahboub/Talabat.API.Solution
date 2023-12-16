using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.API.Dtos;
using Talabat.API.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;

namespace Talabat.API.Controllers
{
    //[Authorize]
    public class BasketController : ApiBaseController
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;

        public BasketController(IBasketRepository basketRepository,IMapper mapper)
        {
            _basketRepository = basketRepository;
            _mapper = mapper;
        }
        #region End Points

        #region Get Basket By Id
        
        [HttpGet]   //Get: /api/basket?id=1
        public async Task<ActionResult<CustomerBasket>> GetBasket(string id)
        {
            var basket = await _basketRepository.GetBasketAsync(id);
            return basket ?? new CustomerBasket(id);
        }
        #endregion

        #region Create Or Update Basket
        
        [HttpPost]    //Post: /api/basket          => Data in passing in Body as json
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdateBasket(CustomerBasketDto basket)
        {
            var mappedBasket = _mapper.Map<CustomerBasketDto, CustomerBasket>(basket);
            var createdOrUpdatedBasket = await _basketRepository.CreateOrUpdateBasketAsync(mappedBasket);
            if (createdOrUpdatedBasket is null) return BadRequest(new ApiResponse(400));
            return createdOrUpdatedBasket;
        }

        #endregion

        #region Delete Basket
        
        [HttpDelete]    //Delete: /api/basket?id=1 
        public async Task DeleteBasket(string id)
        {
            await _basketRepository.DeleteBasketAsync(id);
        }

        #endregion

        #endregion
    }
}
