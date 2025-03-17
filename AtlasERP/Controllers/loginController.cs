using AtlasERP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AtlasERP.Controllers
{
    [Route("api/login")]
    [ApiController]
    public class loginController : ControllerBase
    {

        private readonly atlasErpContext _context;
        private readonly IConfiguration _configuration;

        public loginController(atlasErpContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }


        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] Usuario userInfo)
        {
            try
            {
                if (userInfo == null)
                {
                    return BadRequest("Usuario no puede ser nulo");
                }

                if (_context == null)
                {
                    return BadRequest("DbContext no está inicializado");
                }

                var result = await _context.Usuarios
                    .FirstOrDefaultAsync(x => x.Email == userInfo.Email && x.Contrasenia == userInfo.Contrasenia);

                if (result == null)
                {
                    return BadRequest("Datos incorrectos");
                }

                if (result.Rol == null)
                {
                    return BadRequest("El rol del usuario no está definido");
                }

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.Email, result.Email),
                new Claim(ClaimTypes.Name, result.Nombre + ' ' + result.Apellido),
                new Claim(ClaimTypes.NameIdentifier, result.Coduser),
                new Claim(ClaimTypes.PrimaryGroupSid, result.CodCia),
                new Claim(ClaimTypes.Role, result.Rol.ToString())
            }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                return Ok(new { Token = tokenString });
            }
            catch (Exception ex)
            {
                // Log the exception (usar un logger como Serilog, NLog, etc.)
                Console.WriteLine($"Error en el login: {ex.Message}");
                return StatusCode(500, "Ocurrió un error interno en el servidor.");
            }
        }


    }
}
