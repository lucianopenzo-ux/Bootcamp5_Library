using TheLibrary.Common.DTOs;

namespace TheLibrary_BusinessLayer.Services
{
    public interface ILibraryBranchService
    {
        Task<List<LibraryBranchDTO>> GetAllAsync(int? libraryId = null);
        Task<LibraryBranchDTO> GetByIdAsync(int id);
        Task<int> AddAsync(LibraryBranchCreateDTO dto);
        Task UpdateAsync(int id, LibraryBranchCreateDTO dto);
        Task DeleteAsync(int id);
    }
}
