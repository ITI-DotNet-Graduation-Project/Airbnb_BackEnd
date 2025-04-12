using Airbnb.Application.Services.Abstract;
using Airbnb.Application.Settings;
using Airbnb.DATA.models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Airbnb.Application.Services.Implementation
{
    public class JwtProvider : IJwtProvider
    {
        private readonly JwtSetting _jwtSettings;

        public JwtProvider(IOptions<JwtSetting> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public JwtResult GenerateTaken(User user, IEnumerable<string> roles)
        {
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, (user.Id).ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
                new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
                new Claim(nameof(roles), JsonSerializer.Serialize(roles)),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var expiresIn = _jwtSettings.ExpiryMinutes;
            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresIn),
                signingCredentials: signingCredentials
            );

            return new JwtResult
            (
                 new JwtSecurityTokenHandler().WriteToken(token),
                  expiresIn
            );
        }

        public string? ValidateTaken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)),
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken securityToken);

                var jwtToken = (JwtSecurityToken)securityToken;
                return jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
