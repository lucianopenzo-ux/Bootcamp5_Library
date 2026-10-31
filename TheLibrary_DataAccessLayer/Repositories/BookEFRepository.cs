using Microsoft.EntityFrameworkCore;

using TheLibrary.Common.DTOs;

using TheLibrary_DataAccessLayer.Data;
using TheLibrary_DataAccessLayer.Models;

namespace TheLibrary_DataAccessLayer.Repositories
{
    public class BookEFRepository : IBookRepository
    {
        private readonly TheLibraryDbContext _context;

        public BookEFRepository(TheLibraryDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<BookDTO>> GetAllBooksAsync(int pageIndex, int pageSize, string? search)
        {
            IQueryable<Book> query = _context.Books
                .AsNoTracking()
                .Include(b => b.LibraryBranch)
                    .ThenInclude(lb => lb.Library);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchTerm = search.Trim().ToLower();
                query = query.Where(b =>
                    b.Title.ToLower().Contains(searchTerm) ||
                    b.Author.ToLower().Contains(searchTerm) ||
                    b.ISBN.ToLower().Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();

            var books = await query
                .OrderBy(b => b.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BookDTO
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    ISBN = b.ISBN,
                    Year = b.Year,
                    IsAvailable = b.IsAvailable,
                    LibraryBranchName = b.LibraryBranch.Name,
                    LibraryName = b.LibraryBranch.Library.Name
                })
                .ToListAsync();

            return new PagedResult<BookDTO>
            {
                Items = books,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<Book?> GetBookByIdAsync(int id)
        {
            return await _context.Books
                .Include(b => b.LibraryBranch)
                    .ThenInclude(lb => lb.Library)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task AddBookAsync(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBookAsync(Book book)
        {
            var existingBook = await _context.Books.FirstOrDefaultAsync(b => b.Id == book.Id);
            if (existingBook != null)
            {
                existingBook.Title = book.Title;
                existingBook.Author = book.Author;
                existingBook.ISBN = book.ISBN;
                existingBook.Year = book.Year;
                existingBook.LibraryBranchId = book.LibraryBranchId;
                existingBook.IsAvailable = book.IsAvailable;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteBookAsync(int id)
        {
            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
        }
    }
}
