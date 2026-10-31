using TheLibrary_DataAccessLayer.Models;

namespace TheLibrary_BusinessLayer.Services.Auth
{
    public interface IJwtService
    {
        Task<string> GenerateJwtTokenAsync(User user, IList<string> roles);
    }
}
