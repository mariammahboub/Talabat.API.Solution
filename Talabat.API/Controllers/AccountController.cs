using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.API.Dtos;
using Talabat.API.Errors;
using Talabat.API.Extensions;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Services.Contract;

namespace Talabat.API.Controllers
{
    public class AccountController : ApiBaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public AccountController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IAuthService authService,
            IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authService = authService;
            _mapper = mapper;
        }

        #region Login End point
        [HttpPost("Login")]  // Post : /api/account/login
        public async Task<ActionResult<UserDto>> Login(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized(new ApiResponse(401));
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
                return Unauthorized(new ApiResponse(401));
            return Ok(new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await _authService.CreateTokenAsync(user, _userManager)
            }); ;
        }
        #endregion

        #region Register End Point

        [HttpPost("Register")] // Post: /api/account/register
        public async Task<ActionResult<UserDto>> Register(RegisterDto model)
        {
            if (CheckEmailExists(model.Email).Result.Value)
                return BadRequest(new ApiValidationErrorResponse() { Errors = new string[] { "this email is already in user!!" } });
            var user = new AppUser()
            {
                DisplayName = model.DisplayName,
                Email = model.Email,
                UserName = model.Email.Split("@")[0],
                PhoneNumber = model.PhoneNumber,
            };
            var result = await _userManager.CreateAsync(user,model.Password);
            if (!result.Succeeded)
                return BadRequest(new ApiResponse(400));
            return Ok(new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await _authService.CreateTokenAsync(user, _userManager)
            });
        }


        #endregion

        #region Get Current user
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);
            return Ok(new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await _authService.CreateTokenAsync(user, _userManager)
            });
        }
        #endregion

        #region Get Current user Address
        [Authorize]
        [HttpGet("address")]
        public async Task<ActionResult<AddressDto>> GetUserAddress()
        {
            var user = await _userManager.FindUserWithAddressAsync(User);
            var Address = _mapper.Map<AddressDto>(user.Address);
            return Ok(Address);
        }
        #endregion

        #region Update Current user Address
        [Authorize]
        [HttpPut("address")]
        public async Task<ActionResult<AddressDto>> UpdateUserAddress(AddressDto updatedAddress)
        {
            var address = _mapper.Map<AddressDto, Address>(updatedAddress);
            var user = await _userManager.FindUserWithAddressAsync(User);
            address.Id = user.Address.Id;
            user.Address = address;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) return BadRequest(new ApiResponse(400));
            return Ok(updatedAddress);
        }
        #endregion

        #region Email Exist
        [HttpGet("emailexists")]
        public async Task<ActionResult<bool>> CheckEmailExists(string email)
        {
            return await _userManager.FindByEmailAsync(email) is not null;
        }
        #endregion

    }
}
