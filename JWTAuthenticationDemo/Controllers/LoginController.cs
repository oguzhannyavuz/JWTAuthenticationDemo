using JWTAuthenticationDemo.DataLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWTAuthenticationDemo.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class LoginController : ControllerBase
	{
		private readonly Context _context;
		private readonly IConfiguration _configuration;

		public LoginController(Context context, IConfiguration configuration)
		{
			_context = context;
			_configuration = configuration;
		}

		[HttpPost]
		public IActionResult Login([FromBody] LoginDto loginDto)
		{
			var check = _context.Users.FirstOrDefault(x => x.Username == loginDto.UserName && x.Password == loginDto.Password);
			if (check != null)
			{
				var jwtSettings = _configuration.GetSection("Jwt");

				var claims = new[]
				{
						new Claim(JwtRegisteredClaimNames.Sub, jwtSettings["Subject"]),
						new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
						//new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(),ClaimValueTypes.Integer64), // alttaki version çalışıyor 
						new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
						new Claim("UserId", check.Id.ToString()), // Örnek kullanıcı ID'si
				        new Claim("Username",check.Username), // Örnek kullanıcı adı
				        //new Claim("Email", "john.doe@example.com") // Örnek e-posta
				};

				var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
				var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

				var token = new JwtSecurityToken(
				issuer: jwtSettings["Issuer"],
				audience: jwtSettings["Audience"],
				claims: claims,
				expires: DateTime.Now.AddDays(1),
				signingCredentials: creds);

				return Ok(new JwtSecurityTokenHandler().WriteToken(token));
			}
			return BadRequest("Hatalı kullanıcı adı ya da parola!");
		}

		[HttpGet("users")]
		public IActionResult Users()
		{
			var data = _context.Users.ToList();
			return Ok(data);
		}
	}
}
