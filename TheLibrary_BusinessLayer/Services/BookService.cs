using TheLibrary.Common.DTOs;

using TheLibrary_DataAccessLayer.Models;
using TheLibrary_DataAccessLayer.Repositories;

namespace TheLibrary_BusinessLayer.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly ILibraryBranchRepository _libraryBranchRepository;

        public BookService(IBookRepository bookRepository, ILibraryBranchRepository libraryBranchRepository)
        {
            _bookRepository = bookRepository;
            _libraryBranchRepository = libraryBranchRepository;
        }

        public async Task<PagedResult<BookDTO>> GetAllBooksAsync(int pageIndex, int pageSize, string? search)
        {
            if (pageIndex <= 0)
            {
                throw new ArgumentException("pageIndex debe ser mayor a cero.");
            }

            if (pageSize <= 0)
            {
                throw new ArgumentException("pageSize debe ser mayor a cero.");
            }

            return await _bookRepository.GetAllBooksAsync(pageIndex, pageSize, search?.Trim());
        }

        public async Task<BookDTO> GetBookByIdAsync(int id)
        {
            ValidateId(id);

            var book = await _bookRepository.GetBookByIdAsync(id);
            if (book == null)
            {
                throw new KeyNotFoundException($"No se encontró un libro con el ID {id}.");
            }

            return MapToDto(book);
        }

        public async Task<int> AddBookAsync(BookCreateDTO dto)
        {
            ValidateBookDto(dto);
            await ValidateBranchAsync(dto.LibraryBranchId);

            var book = new Book
            {
                Title = dto.Title.Trim(),
                Author = dto.Author.Trim(),
                ISBN = dto.ISBN.Trim(),
                Year = dto.Year.Trim(),
                LibraryBranchId = dto.LibraryBranchId,
                IsAvailable = true
            };

            await _bookRepository.AddBookAsync(book);
            return book.Id;
        }

        public async Task UpdateBookAsync(int id, BookCreateDTO dto)
        {
            ValidateId(id);
            ValidateBookDto(dto);
            await ValidateBranchAsync(dto.LibraryBranchId);

            var existingBook = await _bookRepository.GetBookByIdAsync(id);
            if (existingBook == null)
            {
                throw new KeyNotFoundException($"No se encontró un libro con el ID {id}.");
            }

            existingBook.Title = dto.Title.Trim();
            existingBook.Author = dto.Author.Trim();
            existingBook.ISBN = dto.ISBN.Trim();
            existingBook.Year = dto.Year.Trim();
            existingBook.LibraryBranchId = dto.LibraryBranchId;

            await _bookRepository.UpdateBookAsync(existingBook);
        }

        public async Task DeleteBookAsync(int id)
        {
            ValidateId(id);

            var book = await _bookRepository.GetBookByIdAsync(id);
            if (book == null)
            {
                throw new KeyNotFoundException($"No se encontró un libro con el ID {id}.");
            }

            await _bookRepository.DeleteBookAsync(id);
        }

        private async Task ValidateBranchAsync(int libraryBranchId)
        {
            if (!await _libraryBranchRepository.ExistsAsync(libraryBranchId))
            {
                throw new ArgumentException($"No existe una sucursal con ID {libraryBranchId}.");
            }
        }

        private static void ValidateId(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("El ID debe ser mayor a cero.");
            }
        }

        private static void ValidateBookDto(BookCreateDTO dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            if (string.IsNullOrWhiteSpace(dto.Title))
            {
                throw new ArgumentException("El título del libro es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(dto.Author))
            {
                throw new ArgumentException("El autor del libro es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(dto.ISBN))
            {
                throw new ArgumentException("El ISBN del libro es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(dto.Year) || dto.Year.Length != 4)
            {
                throw new ArgumentException("El año del libro debe tener 4 dígitos.");
            }

            if (dto.LibraryBranchId <= 0)
            {
                throw new ArgumentException("LibraryBranchId debe ser mayor a cero.");
            }
        }

        private static BookDTO MapToDto(Book book)
        {
            return new BookDTO
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                ISBN = book.ISBN,
                Year = book.Year,
                IsAvailable = book.IsAvailable,
                LibraryBranchName = book.LibraryBranch?.Name ?? string.Empty,
                LibraryName = book.LibraryBranch?.Library?.Name ?? string.Empty
            };
        }
    }
}
