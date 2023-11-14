namespace WebStore.Models.Account
{
    public class LoginModel
    {
        /// <summary>
        /// 
        /// </summary>
        /// <example>lysyi@gmail.com</example>
        public string Username { get; set; } = String.Empty;
        /// <summary>
        /// Password of the user.
        /// </summary>
        public string Password { get; set; } = String.Empty;
    }
}