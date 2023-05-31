using JWTExample.Models.Request;
using JWTExample.Models.Response;
using JWTExample.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWTExample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly IOptions<JwtOption> _jwtOption;

        public AuthenticateController(IOptions<JwtOption> jwtOption)
        {
            _jwtOption = jwtOption;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request.Name == "system")
            {
                var token = GenerateJwtToken(request);

                return Ok(new LoginResponse{ Token = token });
            }

            return Unauthorized();
        }

        private string GenerateJwtToken(LoginRequest request)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOption.Value.Key.ToString()));

            var tokenDescriptior = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, request.Password.ToString()),
                    new Claim(ClaimTypes.Name, request.Name!),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256),
                Issuer = _jwtOption.Value.Issuer,
                Audience = _jwtOption.Value.Audience,

            };
            var token = tokenHandler.CreateToken(tokenDescriptior);

            return tokenHandler.WriteToken(token);
        }
    }
}
