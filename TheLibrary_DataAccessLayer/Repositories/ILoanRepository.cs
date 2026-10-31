using TheLibrary_DataAccessLayer.Models;

namespace TheLibrary_DataAccessLayer.Repositories
{
    public interface ILoanRepository
    {
        Task<List<Loan>> GetAllAsync();
        Task<List<Loan>> GetByUserIdAsync(string userId);
        Task<List<Loan>> GetActiveLoansAsync();
        Task<Loan?> GetByIdAsync(int id);
        Task AddAsync(Loan loan);
        Task UpdateAsync(Loan loan);
        Task DeleteAsync(Loan loan);
        Task<bool> ExistsAsync(int id);
    }
}
