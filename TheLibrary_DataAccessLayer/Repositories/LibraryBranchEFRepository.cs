using Microsoft.EntityFrameworkCore;

using TheLibrary_DataAccessLayer.Data;
using TheLibrary_DataAccessLayer.Models;

namespace TheLibrary_DataAccessLayer.Repositories
{
    public class LibraryBranchEFRepository : ILibraryBranchRepository
    {
        private readonly TheLibraryDbContext _context;

        public LibraryBranchEFRepository(TheLibraryDbContext context)
        {
            _context = context;
        }

        public async Task<List<LibraryBranch>> GetAllAsync()
        {
            return await _context.LibraryBranches
                .Include(lb => lb.Library)
                .Include(lb => lb.Books)
                .ToListAsync();
        }

        public async Task<List<LibraryBranch>> GetByLibraryIdAsync(int libraryId)
        {
            return await _context.LibraryBranches
                .Include(lb => lb.Library)
                .Include(lb => lb.Books)
                .Where(lb => lb.LibraryId == libraryId)
                .ToListAsync();
        }

        public async Task<LibraryBranch?> GetByIdAsync(int id)
        {
            return await _context.LibraryBranches
                .Include(lb => lb.Library)
                .Include(lb => lb.Books)
                .FirstOrDefaultAsync(lb => lb.Id == id);
        }

        public async Task AddAsync(LibraryBranch branch)
        {
            _context.LibraryBranches.Add(branch);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(LibraryBranch branch)
        {
            _context.LibraryBranches.Update(branch);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(LibraryBranch branch)
        {
            _context.LibraryBranches.Remove(branch);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.LibraryBranches.AnyAsync(lb => lb.Id == id);
        }
    }
}
