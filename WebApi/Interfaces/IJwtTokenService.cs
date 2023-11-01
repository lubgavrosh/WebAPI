using WebStore.Data.Entitties.Identity;

namespace WebStore.Interfaces
{
    public interface IJwtTokenService
    {
        Task<string> CreateToken(UserEntity user);
    }
}