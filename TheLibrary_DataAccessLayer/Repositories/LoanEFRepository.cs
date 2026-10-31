using Microsoft.EntityFrameworkCore;

using TheLibrary_DataAccessLayer.Data;
using TheLibrary_DataAccessLayer.Models;

namespace TheLibrary_DataAccessLayer.Repositories
{
    public class LoanEFRepository : ILoanRepository
    {
        private readonly TheLibraryDbContext _context;

        public LoanEFRepository(TheLibraryDbContext context)
        {
            _context = context;
        }

        public async Task<List<Loan>> GetAllAsync()
        {
            return await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.User)
                .ToListAsync();
        }

        public async Task<List<Loan>> GetByUserIdAsync(string userId)
        {
            return await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.User)
                .Where(l => l.UserId == userId)
                .ToListAsync();
        }

        public async Task<List<Loan>> GetActiveLoansAsync()
        {
            return await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.User)
                .Where(l => l.ReturnDate == null)
                .ToListAsync();
        }

        public async Task<Loan?> GetByIdAsync(int id)
        {
            return await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task AddAsync(Loan loan)
        {
            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Loan loan)
        {
            _context.Loans.Update(loan);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Loan loan)
        {
            _context.Loans.Remove(loan);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Loans.AnyAsync(l => l.Id == id);
        }
    }
}
