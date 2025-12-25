using NotesApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using NotesApp.Application.Interfaces;


namespace NotesApp.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            //var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            //var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            // ... inside GenerateToken(User user)
            var keyString = _config["Jwt:Key"] ?? throw new InvalidOperationException("Missing Jwt:Key configuration.");
            byte[] keyBytes;

            // if caller provided a base64-encoded key, decode it; otherwise use UTF8 bytes
            try
            {
                keyBytes = Convert.FromBase64String(keyString);
            }
            catch (FormatException)
            {
                keyBytes = Encoding.UTF8.GetBytes(keyString);
            }

            // require at least 32 bytes (256 bits) for HS256
            if (keyBytes.Length < 32)
                throw new InvalidOperationException("Jwt:Key must be at least 256 bits (32 bytes). Provide a longer secret or a base64-encoded key with sufficient entropy.");

            var key = new SymmetricSecurityKey(keyBytes);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            // ...
            // Safe parsing with fallback
            var expiresInSetting = _config["Jwt:ExpiresInMinutes"];
            var expiresInMinutes = string.IsNullOrWhiteSpace(expiresInSetting) ? 60 : int.Parse(expiresInSetting);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
