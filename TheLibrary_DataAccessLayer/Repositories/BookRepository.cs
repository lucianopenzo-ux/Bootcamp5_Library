using TheLibrary.Common.DTOs;

using TheLibrary_DataAccessLayer.Models;

namespace TheLibrary_DataAccessLayer.Repositories
{
    public class BookRepository : IBookRepository
    {
        private static readonly List<Book> _books = new()
        {
            new Book { Id = 1, Title = "El Quijote", Year = "1605", IsAvailable = true },
            new Book { Id = 2, Title = "Cien Años de Soledad", Year = "1967", IsAvailable = true },
            new Book { Id = 3, Title = "La Sombra del Viento", Year = "2001", IsAvailable = true }
        };

        public Task<PagedResult<BookDTO>> GetAllBooksAsync(int pageIndex, int pageSize, string? search)
        {
            var booksQuery = _books.AsQueryable();

            var result = new PagedResult<BookDTO>
            {
                Items = booksQuery
                    .Where(b => string.IsNullOrEmpty(search) || b.Title.Contains(search) || b.Author.Contains(search))
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .Select(b => new BookDTO
                    {
                        Id = b.Id,
                        Title = b.Title,
                        Author = b.Author,
                        ISBN = b.ISBN,
                        Year = b.Year,
                        IsAvailable = b.IsAvailable
                    })
                    .ToList(),
                TotalCount = booksQuery.Count(),
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            return Task.FromResult(result);
        }

        public Task<Book?> GetBookByIdAsync(int id)
        {
            return Task.FromResult(_books.FirstOrDefault(b => b.Id == id));
        }

        public Task AddBookAsync(Book book)
        {
            book.Id = _books.Count == 0 ? 1 : _books.Max(b => b.Id) + 1;
            _books.Add(book);
            return Task.CompletedTask;
        }

        public async Task UpdateBookAsync(Book book)
        {
            var existingBook = await GetBookByIdAsync(book.Id);
            if (existingBook != null)
            {
                existingBook.Title = book.Title;
                existingBook.Author = book.Author;
                existingBook.ISBN = book.ISBN;
                existingBook.Year = book.Year;
                existingBook.IsAvailable = book.IsAvailable;
                existingBook.LibraryBranchId = book.LibraryBranchId;
            }
        }

        public async Task DeleteBookAsync(int id)
        {
            var book = await GetBookByIdAsync(id);
            if (book != null)
            {
                _books.Remove(book);
            }
        }
    }
}
