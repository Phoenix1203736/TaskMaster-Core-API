using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using TaskManagerPro.TaskManagerPro.Interfaces;
using TaskManagerPro.TaskMasterPro.Domain;

namespace TaskManagerPro.TaskMasterPro.Infrastructure.Services;

public class GenerateTokenService : ITokenService
{
    private readonly IConfiguration _config;

    // El "Portero" que recibe la configuración del appsettings.json
    public GenerateTokenService(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateToken(UserEntity userEntity)
    {
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userEntity.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            // Ahora sí, sacamos la llave del appsettings.json
            var keyString = _config["Jwt:Key"];
            if (keyString != null)
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));

                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _config["Jwt:Issuer"],
                    audience: _config["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(2),
                    signingCredentials: creds
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }

            return null;
        }
    }
}