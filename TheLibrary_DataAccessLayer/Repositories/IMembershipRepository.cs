using TheLibrary_DataAccessLayer.Models;

namespace TheLibrary_DataAccessLayer.Repositories
{
    public interface IMembershipRepository
    {
        Task<List<Membership>> GetAllAsync();
        Task<List<Membership>> GetByMemberIdAsync(string memberId);
        Task<Membership?> GetByIdAsync(int id);
        Task AddAsync(Membership membership);
        Task UpdateAsync(Membership membership);
        Task DeleteAsync(Membership membership);
        Task<bool> ExistsAsync(int id);
    }
}
