using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebStore.Business_logic.Authentication;
using WebStore.Data.Entitties.Identity;
using WebStore.Models.Account;

namespace WebStore.Controllers
{
    [ApiController, AllowAnonymous, Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private IAuthService AuthService { get; }
        public AuthController(IAuthService authService)
        {
            AuthService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                var tokens = await AuthService.Login(model);
                return Ok(tokens);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterModel model)
        {
            try
            {
                await AuthService.Register(model);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] TokensModel tokens)
        {
            try
            {
                var newTokens = await AuthService.Refresh(tokens);
                return Ok(newTokens);
            }
            catch (Exception exception)
            {
                return Unauthorized(new { message = exception.Message });
            }
        }
    }
}