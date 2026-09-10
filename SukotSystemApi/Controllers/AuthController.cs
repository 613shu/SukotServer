using Microsoft.AspNetCore.Mvc;
using SukotSystemApi.Helpers;
using SukotSystemCore.DTOs.Common;
using SukotSystemCore.Models;
using SukotSystemCore.Services;
using System.Threading;

namespace SukotSystemApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        // One shared login for all three roles (Admin/Rabbi/Customer) - the phone number alone
        // determines which table/role resolves the login (see AuthService.LoginAsync).
        // Errors are deliberately NOT caught here - they propagate to
        // ExceptionHandlingMiddleware, which already handles KeyNotFoundException (-> 404) and
        // UnauthorizedAccessException (-> 401) uniformly. This also fixes the local try/catch
        // inconsistency flagged in earlier notes: those two exception types now log through the
        // same Warning-level path as every other handled error, instead of bypassing it.
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto<LoginResultDTO>>> Login(LoginModel loginModel, CancellationToken cancellationToken)
        {
            var login = await _authService.LoginAsync(loginModel, cancellationToken);
            var token = AuthHelper.CreateToken(login, _configuration);

            return Ok(new AuthResponseDto<LoginResultDTO>
            {
                Token = token,
                Profile = login
            });
        }
    }
}
