using System.Threading.Tasks;
using CursosOnline.Application.DTOs.Auth;
using CursosOnline.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CursosOnline.Web.Controllers
{
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("/api/auth/login")]
        public async Task<IActionResult> LoginApi([FromBody] LoginDto loginDto)
        {
            try
            {
                var result = await _authService.LoginAsync(loginDto);
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPost("/api/auth/register")]
        public async Task<IActionResult> RegisterApi([FromBody] RegisterDto registerDto)
        {
            try
            {
                var result = await _authService.RegisterAsync(registerDto);
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
