using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebStore.Data.Entitties.Identity;
using WebStore.Models.Account;

namespace WebStore.Repository.Security
{
    public class JwtTokenServiceImpl : IJwtTokenService
    {
        private IConfiguration Configuration { get; }
        private UserManager<UserEntity> UserManager { get; }

        public JwtTokenServiceImpl(
            IConfiguration configuration,
            UserManager<UserEntity> userManager)
        {
            Configuration = configuration;
            UserManager = userManager;
        }

        public async Task<TokensModel?> GenerateToken(UserEntity? user)
        {
            if (user == null) return null;

            return new TokensModel
            {
                AccessToken = await CreateAccessToken(user),
                RefreshToken = CreateRefreshToken()
            };
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var key = Encoding.UTF8.GetBytes(Configuration["JwtSecretKey"]);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, // on production make true
                ValidateIssuer = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidIssuer = Configuration["JwtIssuer"],
                ClockSkew = TimeSpan.Zero
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(
                token, tokenValidationParameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken
                || !jwtSecurityToken.Header.Alg.Equals(
                    SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }

        private async Task<string> CreateAccessToken(UserEntity user)
        {
            var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName),
            new (ClaimTypes.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("image", user.Image ?? "user.jpg"),
        };

            var roles = await UserManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(
                role => new Claim(ClaimTypes.Role, role)
                ));

            string secretKey = Configuration["JwtSecretKey"];
            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(secretKey));
            SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken jwt = new(
                signingCredentials: credentials,
                claims: claims,
                expires: DateTime.Now.AddDays(15),
                issuer: Configuration["JwtIssuer"]
            );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        private string CreateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
