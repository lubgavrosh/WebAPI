using System.Security.Claims;
using WebStore.Data.Entitties.Identity;
using WebStore.Models.Account;

namespace WebStore.Repository.Security
{
    public interface IJwtTokenService
    {
        Task<TokensModel?> GenerateToken(UserEntity? user);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}