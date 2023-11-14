using WebStore.Models.Account;
namespace WebStore.Business_logic.Authentication
{

    public interface IAuthService
    {
        /// <summary>
        /// Checks if user can be authenticated with given credentials.
        /// </summary>
        /// <param name="model">Model holding user credentials required to login</param>
        /// <returns>An access token and a refresh token</returns>
        public Task<TokensModel?> Login(LoginModel model);
        /// <summary>
        /// Registers new user.
        /// </summary>
        /// <param name="model">Credentials required to register.</param>
        /// <returns>If registered - true, else returns false</returns>
        public Task Register(RegisterModel model);
        /// <summary>
        /// Tries to refresh the access token.
        /// </summary>
        /// <param name="model">Model holding the access and refresh tokens</param>
        /// <returns>A new set of access and refresh token</returns>
        public Task<TokensModel> Refresh(TokensModel model);
    }
}
