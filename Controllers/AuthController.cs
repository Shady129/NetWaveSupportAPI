using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NetWaveSupportAPI.Data;
using NetWaveSupportAPI.DTOs;
using NetWaveSupportAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


namespace NetWaveSupportAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        private readonly IConfiguration _configuration;

        private readonly ILogger<AuthController> _logger;


        public AuthController(AppDbContext context, IConfiguration configuration, ILogger<AuthController> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }


        private string GenerateJwtToken(Customer customer)
        {
            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString()),
            new Claim(ClaimTypes.Email, customer.Email),
            new Claim(ClaimTypes.Role, customer.Role)
    };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        private string GenerateRefreshToken()
        {
            byte[] randomBytes = RandomNumberGenerator.GetBytes(64);

            return Convert.ToBase64String(randomBytes);
        }



        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            bool emailExists = await _context.Customers
                .AnyAsync(c => c.Email == dto.Email);

            if (emailExists)
                return BadRequest("Email already exists.");

            string passwordHash =
                BCrypt.Net.BCrypt.HashPassword(dto.Password);

            Customer customer = new Customer
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = passwordHash,
                Role = "Customer",
                IsRefreshTokenRevoked = false
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return Ok("Customer registered successfully.");
        }


        [EnableRateLimiting("fixed")]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            Customer? customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == dto.Email);


            if (customer == null)
            {
                _logger.LogWarning(
                    "Failed login attempt for email {Email} from IP {IP}",
                    dto.Email,
                    HttpContext.Connection.RemoteIpAddress);

                return Unauthorized("Invalid email or password.");
            }


            bool isPasswordValid =
                BCrypt.Net.BCrypt.Verify(dto.Password, customer.PasswordHash);


            if (!isPasswordValid)
            {
                _logger.LogWarning(
                    "Failed login attempt for email {Email} from IP {IP}",
                    dto.Email,
                    HttpContext.Connection.RemoteIpAddress);

                return Unauthorized("Invalid email or password.");
            }


            string token = GenerateJwtToken(customer);

            string refreshToken = GenerateRefreshToken();

            string refreshTokenHash = BCrypt.Net.BCrypt.HashPassword(refreshToken);


            customer.RefreshTokenHash = refreshTokenHash;
            customer.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            customer.IsRefreshTokenRevoked = false;

            await _context.SaveChangesAsync();


            return Ok(new
            {
                AccessToken = token,
                RefreshToken = refreshToken
            });
        }




        [EnableRateLimiting("fixed")]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenDto dto)
        {

            Customer? customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == dto.Email);

            if (customer == null)
            {
                _logger.LogWarning(
                    "Failed refresh attempt for email {Email} from IP {IP}",
                    dto.Email,
                    HttpContext.Connection.RemoteIpAddress);

                return Unauthorized();
            }


            if (customer.IsRefreshTokenRevoked)
            {
                _logger.LogWarning(
                    "Revoked refresh token used for email {Email} from IP {IP}",
                    dto.Email,
                    HttpContext.Connection.RemoteIpAddress);

                return Unauthorized();
            }



            if (customer.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                _logger.LogWarning(
                    "Expired refresh token used for email {Email} from IP {IP}",
                    dto.Email,
                    HttpContext.Connection.RemoteIpAddress);

                return Unauthorized();
            }



            bool isRefreshTokenValid = BCrypt.Net.BCrypt.Verify(dto.RefreshToken,customer.RefreshTokenHash);


            if (!isRefreshTokenValid)
            {
                _logger.LogWarning(
                    "Invalid refresh token used for email {Email} from IP {IP}",
                    dto.Email,
                    HttpContext.Connection.RemoteIpAddress);

                return Unauthorized();
            }




            string newAccessToken = GenerateJwtToken(customer);

            string newRefreshToken = GenerateRefreshToken();

            string newRefreshTokenHash = BCrypt.Net.BCrypt.HashPassword(newRefreshToken);




            customer.RefreshTokenHash = newRefreshTokenHash;
            customer.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            customer.IsRefreshTokenRevoked = false;


            await _context.SaveChangesAsync();



            return Ok(new
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });

        }



        [EnableRateLimiting("fixed")]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(LogoutDto dto)
        {

            Customer? customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == dto.Email);

            if (customer == null)
                return Unauthorized();



            bool isRefreshTokenValid = BCrypt.Net.BCrypt.Verify(dto.RefreshToken,customer.RefreshTokenHash);

            if (!isRefreshTokenValid)
                return Unauthorized();


            customer.IsRefreshTokenRevoked = true;

            await _context.SaveChangesAsync();

            return Ok("Logged out successfully.");

        }

    }
}
