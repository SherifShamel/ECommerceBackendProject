using ECommerce.Application.Contracts;
using ECommerce.Application.DTO_s.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    public class AuthenticationController : ApiBaseController
    {
        private readonly IAuthenticationServices authenticationService;

        public AuthenticationController(IAuthenticationServices authenticationService)
        {
            this.authenticationService = authenticationService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto, CancellationToken ct)
        {
            return ToActionResult(await authenticationService.LoginAsync(loginDto, ct));
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto, CancellationToken ct)
        {
            return ToActionResult(await authenticationService.RegisterAsync(registerDto, ct));
        }


        [HttpGet("emailexists")]
        public async Task<ActionResult<bool>> CheckEmail([FromQuery] string email, CancellationToken ct)
        {
            return ToActionResult(await authenticationService.CheckEmailAsync(email, ct));
        }

        [Authorize]
        [HttpGet("currentUser")]
        public async Task<ActionResult<UserDto>> GetCurrentUser([FromQuery] string email, CancellationToken ct)
        {
            return ToActionResult(await authenticationService.GetCurrentuser(email, ct));
        }

        [HttpGet("address")]
        [Authorize]
        public async Task<ActionResult<AddressDto>> GetuserAddress([FromQuery] string email, CancellationToken ct)
        {
            return ToActionResult(await authenticationService.GetUserAddressAync(email, ct));
        }

        [HttpPost("address")]
        [Authorize]
        public async Task<ActionResult<AddressDto>> UpdateUserAddress([FromQuery] string email, [FromBody] AddressDto addressDto, CancellationToken ct)
        {
            return ToActionResult(await authenticationService.UpdateUserAddressAsync(addressDto, email, ct));
        }


    }
}
