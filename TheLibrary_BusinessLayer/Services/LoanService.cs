using Microsoft.EntityFrameworkCore;

using TheLibrary.Common.DTOs;

using TheLibrary_DataAccessLayer.Data;
using TheLibrary_DataAccessLayer.Models;
using TheLibrary_DataAccessLayer.Repositories;

namespace TheLibrary_BusinessLayer.Services
{
    public class LoanService : ILoanService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IBookRepository _bookRepository;
        private readonly TheLibraryDbContext _dbContext;

        public LoanService(
            ILoanRepository loanRepository,
            IBookRepository bookRepository,
            TheLibraryDbContext dbContext)
        {
            _loanRepository = loanRepository;
            _bookRepository = bookRepository;
            _dbContext = dbContext;
        }

        public async Task<List<LoanDTO>> GetAllAsync(bool onlyActive = false, string? userId = null)
        {
            List<Loan> loans;

            if (!string.IsNullOrWhiteSpace(userId))
            {
                loans = await _loanRepository.GetByUserIdAsync(userId);
            }
            else if (onlyActive)
            {
                loans = await _loanRepository.GetActiveLoansAsync();
            }
            else
            {
                loans = await _loanRepository.GetAllAsync();
            }

            return loans.Select(MapToDto).ToList();
        }

        public async Task<LoanDTO> GetByIdAsync(int id)
        {
            ValidateId(id);

            var loan = await _loanRepository.GetByIdAsync(id);
            if (loan == null)
            {
                throw new KeyNotFoundException($"No se encontró un préstamo con ID {id}.");
            }

            return MapToDto(loan);
        }

        public async Task<int> AddAsync(LoanCreateDTO dto)
        {
            ValidateDto(dto);

            var book = await _bookRepository.GetBookByIdAsync(dto.BookId);
            if (book == null)
            {
                throw new ArgumentException($"No existe un libro con ID {dto.BookId}.");
            }

            if (!book.IsAvailable)
            {
                throw new InvalidOperationException("El libro no está disponible para préstamo.");
            }

            if (!await _dbContext.Users.AnyAsync(u => u.Id == dto.UserId))
            {
                throw new ArgumentException($"No existe un usuario con ID {dto.UserId}.");
            }

            var loan = new Loan
            {
                LoanDate = dto.LoanDate,
                DueDate = dto.DueDate,
                BookId = dto.BookId,
                UserId = dto.UserId,
                ReturnDate = null
            };

            await _loanRepository.AddAsync(loan);

            book.IsAvailable = false;
            await _bookRepository.UpdateBookAsync(book);

            return loan.Id;
        }

        public async Task UpdateAsync(int id, LoanCreateDTO dto)
        {
            ValidateId(id);
            ValidateDto(dto);

            var loan = await _loanRepository.GetByIdAsync(id);
            if (loan == null)
            {
                throw new KeyNotFoundException($"No se encontró un préstamo con ID {id}.");
            }

            if (!await _dbContext.Users.AnyAsync(u => u.Id == dto.UserId))
            {
                throw new ArgumentException($"No existe un usuario con ID {dto.UserId}.");
            }

            if (loan.BookId != dto.BookId)
            {
                var newBook = await _bookRepository.GetBookByIdAsync(dto.BookId);
                if (newBook == null)
                {
                    throw new ArgumentException($"No existe un libro con ID {dto.BookId}.");
                }

                if (!newBook.IsAvailable)
                {
                    throw new InvalidOperationException("El nuevo libro no está disponible para préstamo.");
                }

                var oldBook = await _bookRepository.GetBookByIdAsync(loan.BookId);
                if (oldBook != null)
                {
                    oldBook.IsAvailable = true;
                    await _bookRepository.UpdateBookAsync(oldBook);
                }

                newBook.IsAvailable = false;
                await _bookRepository.UpdateBookAsync(newBook);
            }

            loan.LoanDate = dto.LoanDate;
            loan.DueDate = dto.DueDate;
            loan.BookId = dto.BookId;
            loan.UserId = dto.UserId;

            await _loanRepository.UpdateAsync(loan);
        }

        public async Task ReturnLoanAsync(int id)
        {
            ValidateId(id);

            var loan = await _loanRepository.GetByIdAsync(id);
            if (loan == null)
            {
                throw new KeyNotFoundException($"No se encontró un préstamo con ID {id}.");
            }

            if (loan.ReturnDate.HasValue)
            {
                throw new InvalidOperationException("El préstamo ya fue devuelto.");
            }

            loan.ReturnDate = DateTime.UtcNow;
            await _loanRepository.UpdateAsync(loan);

            var book = await _bookRepository.GetBookByIdAsync(loan.BookId);
            if (book != null)
            {
                book.IsAvailable = true;
                await _bookRepository.UpdateBookAsync(book);
            }
        }

        public async Task DeleteAsync(int id)
        {
            ValidateId(id);

            var loan = await _loanRepository.GetByIdAsync(id);
            if (loan == null)
            {
                throw new KeyNotFoundException($"No se encontró un préstamo con ID {id}.");
            }

            if (!loan.ReturnDate.HasValue)
            {
                var book = await _bookRepository.GetBookByIdAsync(loan.BookId);
                if (book != null)
                {
                    book.IsAvailable = true;
                    await _bookRepository.UpdateBookAsync(book);
                }
            }

            await _loanRepository.DeleteAsync(loan);
        }

        private static void ValidateId(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("El ID debe ser mayor a cero.");
            }
        }

        private static void ValidateDto(LoanCreateDTO dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            if (dto.BookId <= 0)
            {
                throw new ArgumentException("BookId debe ser mayor a cero.");
            }

            if (string.IsNullOrWhiteSpace(dto.UserId))
            {
                throw new ArgumentException("UserId es obligatorio.");
            }

            if (dto.DueDate.HasValue && dto.DueDate < dto.LoanDate)
            {
                throw new ArgumentException("La fecha de vencimiento no puede ser menor a la fecha de préstamo.");
            }
        }

        private static LoanDTO MapToDto(Loan loan)
        {
            return new LoanDTO
            {
                Id = loan.Id,
                LoanDate = loan.LoanDate,
                DueDate = loan.DueDate,
                ReturnDate = loan.ReturnDate,
                BookId = loan.BookId,
                BookTitle = loan.Book?.Title ?? string.Empty,
                UserId = loan.UserId
            };
        }
    }
}
