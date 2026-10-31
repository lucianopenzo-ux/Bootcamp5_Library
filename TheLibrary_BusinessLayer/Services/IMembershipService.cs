using TheLibrary.Common.DTOs;

namespace TheLibrary_BusinessLayer.Services
{
    public interface IMembershipService
    {
        Task<List<MembershipDTO>> GetAllAsync(string? memberId = null);
        Task<MembershipDTO> GetByIdAsync(int id);
        Task<int> AddAsync(MembershipCreateDTO dto);
        Task UpdateAsync(int id, MembershipCreateDTO dto);
        Task SetActiveAsync(int id, bool isActive);
        Task DeleteAsync(int id);
    }
}
