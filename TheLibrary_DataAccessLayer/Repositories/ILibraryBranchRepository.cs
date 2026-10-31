using TheLibrary_DataAccessLayer.Models;

namespace TheLibrary_DataAccessLayer.Repositories
{
    public interface ILibraryBranchRepository
    {
        Task<List<LibraryBranch>> GetAllAsync();
        Task<List<LibraryBranch>> GetByLibraryIdAsync(int libraryId);
        Task<LibraryBranch?> GetByIdAsync(int id);
        Task AddAsync(LibraryBranch branch);
        Task UpdateAsync(LibraryBranch branch);
        Task DeleteAsync(LibraryBranch branch);
        Task<bool> ExistsAsync(int id);
    }
}
