using BankSystem.Business.Operations.Security;
using BankSystem.Business.Operations.Security.Dtos;
using BankSystem.WebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankSystem.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        private readonly ISecurityService _securityService;

        public SecurityController(ISecurityService securityService)
        {
            _securityService = securityService;

        }

        [HttpPost("two-factor-auth")]
        public async Task<IActionResult> SetTwoFactorAuth(TwoFactorAuthRequest request)
        {
            var userId = int.Parse(User.FindFirst("Id").Value);
            var ipAddress = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? HttpContext.Connection.RemoteIpAddress?.ToString();

            var twoFactorAuthDto = new TwoFactorAuthDto
            {
                Enabled = request.Enabled
            };

            var result = await _securityService.SetTwoFactorAuth(userId, twoFactorAuthDto, ipAddress);

            if (result.IsSucceed)
                return Ok(result.Message);
            else
                return BadRequest(result.Message);
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
            var userId = int.Parse(User.FindFirst("Id").Value);
            var changePasswordDto = new ChangePasswordDto
            {
                Password = request.Password,
                NewPassword = request.NewPassword,
            };
            var ipAddress = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? HttpContext.Connection.RemoteIpAddress?.ToString();
            var result = await _securityService.ChangePassword(userId, changePasswordDto,ipAddress);

            if(result.IsSucceed)
                return Ok(result.Message);
            else
                return BadRequest(result.Message);
        }

        [HttpGet("logs")]
        public async Task<IActionResult> GetLogs()
        {
            var userId = int.Parse(User.FindFirst("Id").Value);
            var result = await _securityService.GetLogs(userId);
            
            return Ok(result);
        }
    }
}
