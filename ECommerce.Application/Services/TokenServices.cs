using ECommerce.Application.Contracts;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ECommerce.Application.Services
{
    public class TokenServices(IOptions<JwtSettings> options) : ITokenServices
    {
        private readonly JwtSettings settings = options.Value;
        public string CreateToken(string userId, string email, string userName, IEnumerable<string> roles)
        {
            //Private Claims
            var Claims = new List<Claim>()
            {
                new(ClaimTypes.NameIdentifier,userId),
                new(ClaimTypes.Email,email),
                new(ClaimTypes.Name,userName)
            };

            Claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SecretKey));

            var Credentials = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256); //Header

            var Token = new JwtSecurityToken(
                issuer: settings.Issuer,
                audience: settings.Audience,
                claims: Claims,
                expires: DateTime.Now.AddMinutes(settings.ExpiryInMinutes),
                signingCredentials: Credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(Token);
        }
    }

    public class JwtSettings
    {
        public string SecretKey { get; set; } = default!;
        public string Issuer { get; set; } = default!;
        public string Audience { get; set; } = default!;
        public int ExpiryInMinutes { get; set; } = 60;
    }
}
