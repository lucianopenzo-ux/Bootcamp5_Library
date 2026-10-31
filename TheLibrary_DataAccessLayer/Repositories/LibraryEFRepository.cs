using Microsoft.EntityFrameworkCore;

using TheLibrary_DataAccessLayer.Data;
using TheLibrary_DataAccessLayer.Models;

namespace TheLibrary_DataAccessLayer.Repositories
{
    public class LibraryEFRepository : ILibraryRepository
    {
        private readonly TheLibraryDbContext _context;

        public LibraryEFRepository(TheLibraryDbContext context)
        {
            _context = context;
        }

        public async Task<List<Library>> GetAllAsync()
        {
            return await _context.Libraries
                .Include(l => l.LibraryBranches)
                .Include(l => l.Members)
                .ToListAsync();
        }

        public async Task<Library?> GetByIdAsync(int id)
        {
            return await _context.Libraries
                .Include(l => l.LibraryBranches)
                .Include(l => l.Members)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task AddAsync(Library library)
        {
            _context.Libraries.Add(library);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Library library)
        {
            _context.Libraries.Update(library);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Library library)
        {
            _context.Libraries.Remove(library);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Libraries.AnyAsync(l => l.Id == id);
        }
    }
}
