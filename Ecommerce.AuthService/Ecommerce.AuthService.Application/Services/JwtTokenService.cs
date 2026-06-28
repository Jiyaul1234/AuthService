using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Ecommerce.AuthService.Application.Interfaces.IService;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Ecommerce.AuthService.Application.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expirationMinutes;

        public JwtTokenService(IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");

            _secretKey = jwtSettings["SecretKey"];
            _issuer = jwtSettings["Issuer"];
            _audience = jwtSettings["Audience"];
            _expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"] ?? "60");

            if (string.IsNullOrEmpty(_secretKey) || _secretKey.Length < 32)
                throw new ArgumentException("JWT SecretKey must be at least 32 characters long.", nameof(_secretKey));

            if (string.IsNullOrEmpty(_issuer))
                throw new ArgumentException("JWT Issuer is required.", nameof(_issuer));

            if (string.IsNullOrEmpty(_audience))
                throw new ArgumentException("JWT Audience is required.", nameof(_audience));
        }

        public string GenerateToken(int userId, string email, string role)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException(nameof(email));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role ?? "User")
            };

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_expirationMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
