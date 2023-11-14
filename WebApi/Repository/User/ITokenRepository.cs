using WebStore.Data.Entitties.Identity;
using WebStore.Models.Account;

namespace WebStore.Repository.User
{
    public interface ITokenRepository
    {
        Task<bool> IsValidUserAsync(LoginModel user);

        Task<UserRefreshTokens> AddUserRefreshTokens(UserRefreshTokens tokens);

        UserRefreshTokens? GetSavedRefreshTokens(string username, string refreshToken);

        Task DeleteUserRefreshTokens(string username, string refreshToken);
    }
}
