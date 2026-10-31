using TheLibrary.Common.DTOs;

namespace TheLibrary_BusinessLayer.Services
{
    public interface ILoanService
    {
        Task<List<LoanDTO>> GetAllAsync(bool onlyActive = false, string? userId = null);
        Task<LoanDTO> GetByIdAsync(int id);
        Task<int> AddAsync(LoanCreateDTO dto);
        Task UpdateAsync(int id, LoanCreateDTO dto);
        Task ReturnLoanAsync(int id);
        Task DeleteAsync(int id);
    }
}
