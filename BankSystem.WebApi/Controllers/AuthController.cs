using BankSystem.Business.Operations.Sms;
using BankSystem.Business.Operations.User;
using BankSystem.Business.Operations.User.Dtos;
using BankSystem.Data.Entities;
using BankSystem.Data.Repositories;
using BankSystem.WebApi.Jwt;
using BankSystem.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankSystem.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ISmsService _smsService;
        private readonly IRepository<UserEntity> _userRepository;
        public AuthController(IUserService userService, ISmsService smsService, IRepository<UserEntity> userRepository)
        {
            _userService = userService;
            _smsService = smsService;   
            _userRepository = userRepository;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var registerDto = new RegisterDto
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Password = request.Password,
                PhoneNumber = request.PhoneNumber,
                NationalId = request.NationalId,
            };
            var result = await _userService.Register(registerDto);
            if (result.IsSucceed)
                return Ok(result.Message);
            else
                return BadRequest(result.Message);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var loginDto = new LoginDto
            {
                NationalId = request.NationalId,
                Password = request.Password,
            };
            var result = await _userService.Login(loginDto);

            if (!result.IsSucceed)
                return BadRequest(result.Message);

            var user = result.Data;

            var userEntity = await _userRepository.GetByIdAsync(user.Id);

            if (userEntity is null)
            {
                return BadRequest("Kullanıcı bulunamadı");
            }
            if (userEntity.TwoFactorEnabled)
            {
                 await _userService.GenerateTwoFactorCode(user.Id);

                return Ok(new
                {
                    Message = "Giriş başarılı lütfen telefonunuza gelen doğrulama kodunu giriniz.",
                    UserId = user.Id,
                });
            }
            else
            {
                var configuration = HttpContext.RequestServices.GetRequiredService<IConfiguration>();

                var token = JwtHelper.GenerateJwt(new JwtDto
                {
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Id = user.Id,
                    UserType = user.UserType,
                    SecretKey = configuration["Jwt:SecretKey"]!,
                    Issuer = configuration["Jwt:Issuer"]!,
                    Audience = configuration["Jwt:Audience"]!,
                    ExpireMinutes = int.Parse(configuration["Jwt:ExpireMinutes"]!)
                });

                return Ok(new LoginResponse
                {
                    Message = "Giriş başarılı",
                    Token = token
                });
            }
        }

        [HttpPost("verify-2fa")]
        public async Task<IActionResult> VerifyTwoFactor(Verify2FARequest request)
        {
            var result = await _userService.VerifyTwoFactorCode(request.UserId, request.Code);
            if (!result.IsSucceed)
                return BadRequest(result.Message);
            var user = result.Data;
            var configuration = HttpContext.RequestServices.GetRequiredService<IConfiguration>();


            var token = JwtHelper.GenerateJwt(new JwtDto
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Id = user.Id,
                UserType = user.UserType,
                SecretKey = configuration["Jwt:SecretKey"]!,
                Issuer = configuration["Jwt:Issuer"]!,
                Audience = configuration["Jwt:Audience"]!,
                ExpireMinutes = int.Parse(configuration["Jwt:ExpireMinutes"]!)
            });

            return Ok(new LoginResponse
            {
                Message = "2FA Doğrulandı. Giriş başarılı",
                Token = token
            });
        }
        

        [HttpGet("{id}")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userService.GetUser(id);
            if (user is null)
                return NotFound("Kullanıcı bulunamadı");
            else
                return Ok(user);
        }

        [HttpPut("{id}")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult>UpdateUser(UpdateRequest request , int id)
        {
            var updateDto = new UpdateDto
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.LastName,

            };
            var result = await _userService.UpdateUser(updateDto, id);
            if (result.IsSucceed)
                return Ok(result);
            else
                return NotFound(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _userService.Delete(id);
            if (result.IsSucceed)
                return Ok(result.Message);
            else
                return NotFound(result.Message);
        }

    }
}
