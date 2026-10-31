using Microsoft.EntityFrameworkCore;

using TheLibrary_DataAccessLayer.Data;
using TheLibrary_DataAccessLayer.Models;

namespace TheLibrary_DataAccessLayer.Repositories
{
    public class MembershipEFRepository : IMembershipRepository
    {
        private readonly TheLibraryDbContext _context;

        public MembershipEFRepository(TheLibraryDbContext context)
        {
            _context = context;
        }

        public async Task<List<Membership>> GetAllAsync()
        {
            return await _context.Memberships
                .Include(m => m.Library)
                .Include(m => m.Member)
                .ToListAsync();
        }

        public async Task<List<Membership>> GetByMemberIdAsync(string memberId)
        {
            return await _context.Memberships
                .Include(m => m.Library)
                .Include(m => m.Member)
                .Where(m => m.MemberId == memberId)
                .ToListAsync();
        }

        public async Task<Membership?> GetByIdAsync(int id)
        {
            return await _context.Memberships
                .Include(m => m.Library)
                .Include(m => m.Member)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task AddAsync(Membership membership)
        {
            _context.Memberships.Add(membership);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Membership membership)
        {
            _context.Memberships.Update(membership);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Membership membership)
        {
            _context.Memberships.Remove(membership);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Memberships.AnyAsync(m => m.Id == id);
        }
    }
}
