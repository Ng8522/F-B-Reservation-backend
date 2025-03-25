using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using UserModel = FnbReservationAPI.src.features.User.User;

namespace FnbReservationAPI.src.features.Jwt
{
    public class Jwt: IJwtService
    {
        private readonly byte[] _keyBytes;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expirationHours;
        private static readonly Dictionary<string, string> ActiveTokens = [];

        public Jwt(IConfiguration configuration)
        {
            _keyBytes = Convert.FromBase64String(configuration["Jwt:SecretKey"] ?? throw new ArgumentNullException("SecretKey is missing"));
            _issuer = configuration["Jwt:Issuer"] ?? throw new ArgumentNullException("Issuer is missing");
            _audience = configuration["Jwt:Audience"] ?? throw new ArgumentNullException("Audience is missing");
            _expirationHours = int.Parse(configuration["Jwt:ExpirationHours"] ?? "24");
        }

         public string GenerateToken(UserModel user)
        {
            var securityKey = new SymmetricSecurityKey(_keyBytes);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenId = Guid.NewGuid().ToString();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, tokenId),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name)
            };

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(_expirationHours),
                signingCredentials: credentials
            );

            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            ActiveTokens[user.Id.ToString()] = tokenString;
            return tokenString;
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                IssuerSigningKey = new SymmetricSecurityKey(_keyBytes) 
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return principal;
            }
            catch
            {
                return null;
            }
        }

        public bool IsTokenActive(string userId, string token)
        {
            if (!ActiveTokens.TryGetValue(userId, out var activeToken) || activeToken != token)
                return false;

            return ValidateToken(token) != null;
        }

        public void InvalidateToken(string userId)
        {
            ActiveTokens.Remove(userId);
        }
    }

}
