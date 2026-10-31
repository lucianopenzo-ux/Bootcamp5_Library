using TheLibrary_DataAccessLayer.Models;

namespace TheLibrary_DataAccessLayer.Repositories
{
    public interface ILibraryRepository
    {
        Task<List<Library>> GetAllAsync();
        Task<Library?> GetByIdAsync(int id);
        Task AddAsync(Library library);
        Task UpdateAsync(Library library);
        Task DeleteAsync(Library library);
        Task<bool> ExistsAsync(int id);
    }
}
