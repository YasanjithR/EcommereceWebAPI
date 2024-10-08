using EcommereceWebAPI.Data;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

//this service is responsible for handling authentication operations
namespace EcommereceWebAPI.Middleware
{
    public class AuthService
    {
        private readonly JwtSettings _jwtSettings;

        public AuthService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        // Generates a JWT token based on the provided user ID, email, and role
        public string GenerateJwtToken(string userId, string email, string role)
        {
            // Define the claims for the token, including user ID, email, and role
            var claims = new[]
            {
                    new Claim(JwtRegisteredClaimNames.Sub, userId),
                    new Claim(JwtRegisteredClaimNames.Email, email),
                    new Claim(ClaimTypes.Role, role)
                };

            // Create a symmetric security key based on the JWT secret
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));

            // Create signing credentials using the security key and HMACSHA256 algorithm
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create a new JWT token with the specified issuer, audience, claims, expiration, and signing credentials
            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwtSettings.ExpiryMinutes),
                signingCredentials: creds);

            // Write the token as a string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
