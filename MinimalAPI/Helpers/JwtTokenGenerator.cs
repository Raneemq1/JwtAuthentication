using Microsoft.IdentityModel.Tokens;
using MinimalAPI.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MinimalAPI.Helpers
{
    public class JwtTokenGenerator
    {
        private readonly IConfiguration _config;

        public JwtTokenGenerator(IConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public string GenerateToken(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserName),
                new Claim(ClaimTypes.Email, user.UserEmail)
            };
            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_config["Jwt:ExpireMinutes"])), // Set expiration dynamically
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return false;

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _config["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _config["Jwt:Audience"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"])),
                ValidateLifetime = true
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, validationParameters, out _);
                return true;
            }
            catch (SecurityTokenException)
            {
                return false;
            }
            catch (Exception)
            {
                throw; 
            }
        }
    }
}
