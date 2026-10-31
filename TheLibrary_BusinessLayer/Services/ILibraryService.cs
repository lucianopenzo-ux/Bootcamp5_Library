using TheLibrary.Common.DTOs;

namespace TheLibrary_BusinessLayer.Services
{
    public interface ILibraryService
    {
        Task<List<LibraryDTO>> GetAllAsync();
        Task<LibraryDTO> GetByIdAsync(int id);
        Task<int> AddAsync(LibraryCreateDTO dto);
        Task UpdateAsync(int id, LibraryCreateDTO dto);
        Task DeleteAsync(int id);
    }
}
