using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TaskManagerPro.TaskMasterPro.Application.Common.Interfaces;
using TaskManagerPro.TaskMasterPro.Application.DTOs.Auth;
using TaskManagerPro.TaskMasterPro.Domain;
using TaskManagerPro.TaskMasterPro.Domain.Interfaces;


namespace TaskManagerPro.TaskMasterPro.Infrastructure.Auth;

public class GenerateTokenService : ITokenService
{
    private readonly IConfiguration _config;
    private readonly IRefreshTokenRepository _refreshTokenRepo;

    public GenerateTokenService(IConfiguration config, IRefreshTokenRepository refreshTokenRepo)
    {
        _config = config;
        _refreshTokenRepo = refreshTokenRepo;
    }

    // ESTE ES EL ÚNICO MÉTODO PÚBLICO QUE NECESITAMOS
    public async Task<AuthResponseDto> GenerateTokensAsync(User user)
    {
        var accessToken = CreateJwtToken(user);
        var refreshTokenString = Guid.NewGuid().ToString();

        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshTokenString,
            UserId = user.Id,
            ExpiryDate = DateTime.UtcNow.AddDays(7), // Aquí ya no marcará error
            CreatedDate = DateTime.UtcNow
        };

        await _refreshTokenRepo.SaveAsync(refreshTokenEntity);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenString
        };
    }

    // MÉTODO PRIVADO: Nadie fuera de esta clase necesita saber cómo se hace el JWT
    private string CreateJwtToken(User user)
    {
        var keyString = _config["Jwt:Key"];
        if (string.IsNullOrEmpty(keyString)) return string.Empty;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("userId", user.Id.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15), 
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    
}