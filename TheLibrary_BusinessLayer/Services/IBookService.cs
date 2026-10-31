using TheLibrary.Common.DTOs;

namespace TheLibrary_BusinessLayer.Services
{
    public interface IBookService
    {
        Task<PagedResult<BookDTO>> GetAllBooksAsync(int pageIndex, int pageSize, string? search);
        Task<BookDTO> GetBookByIdAsync(int id);
        Task<int> AddBookAsync(BookCreateDTO book);
        Task UpdateBookAsync(int id, BookCreateDTO book);
        Task DeleteBookAsync(int id);
    }
}
