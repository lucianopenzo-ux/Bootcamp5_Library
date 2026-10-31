using TheLibrary.Common.DTOs;

namespace TheLibrary_BusinessLayer.Services
{
    public interface IUserService
    {
        Task<PagedResult<UserDTO>> GetAllAsync(int pageIndex, int pageSize, string? search);
        Task<UserDTO> GetByIdAsync(string id);
        Task<string> CreateAsync(UserCreateDTO dto);
        Task UpdateAsync(string id, UserUpdateDTO dto);
        Task DeleteAsync(string id);
    }
}
