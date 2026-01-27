using System.Text;
using System.Security.Claims;
using HomeTech.Domain.Entities;
using HomeTech.Application.DTOs;
using HomeTech.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using HomeTech.Application.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;

public class JwtService : IJwtService
{
    private readonly IConfiguration _config;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly HomeTechDbContext _context;

    public JwtService(IConfiguration config, UserManager<ApplicationUser> userManager, HomeTechDbContext context)
    {
        _config = config;
        _userManager = userManager;
        _context = context;
    }

    public async Task<LoginResponseDto> GenerateTokenAsync(ApplicationUser user, string ipAddress)
    {
        var sessionId = Guid.NewGuid();
        var refreshToken = Guid.NewGuid().ToString("N");
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var Expires = DateTime.UtcNow.AddMinutes(
        double.Parse(_config["Jwt:DurationInMinutes"]!));

        var Claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(ClaimTypes.NameIdentifier, user.Id),
            new("sessionId", sessionId.ToString()),
            new("ip", ipAddress),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: Claims,
            expires: Expires,
            signingCredentials: new SigningCredentials(
                key, SecurityAlgorithms.HmacSha256)
        );

        await _context.UserSessions.AddAsync(new UserSession
        {
            Id = sessionId,
            UserId = user.Id,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(
                int.Parse(_config["Jwt:RefreshTokenDays"]!)),
            IpAddress = ipAddress
        });
        await _context.SaveChangesAsync(CancellationToken.None);

        return new LoginResponseDto
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            RefreshToken = refreshToken,
            ExpiresAt = Expires
        };
    }
}