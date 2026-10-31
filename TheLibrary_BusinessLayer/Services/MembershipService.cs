using Microsoft.EntityFrameworkCore;

using TheLibrary.Common.DTOs;

using TheLibrary_DataAccessLayer.Data;
using TheLibrary_DataAccessLayer.Models;
using TheLibrary_DataAccessLayer.Repositories;

namespace TheLibrary_BusinessLayer.Services
{
    public class MembershipService : IMembershipService
    {
        private readonly IMembershipRepository _membershipRepository;
        private readonly ILibraryRepository _libraryRepository;
        private readonly TheLibraryDbContext _dbContext;

        public MembershipService(
            IMembershipRepository membershipRepository,
            ILibraryRepository libraryRepository,
            TheLibraryDbContext dbContext)
        {
            _membershipRepository = membershipRepository;
            _libraryRepository = libraryRepository;
            _dbContext = dbContext;
        }

        public async Task<List<MembershipDTO>> GetAllAsync(string? memberId = null)
        {
            var memberships = string.IsNullOrWhiteSpace(memberId)
                ? await _membershipRepository.GetAllAsync()
                : await _membershipRepository.GetByMemberIdAsync(memberId);

            return memberships.Select(MapToDto).ToList();
        }

        public async Task<MembershipDTO> GetByIdAsync(int id)
        {
            ValidateId(id);

            var membership = await _membershipRepository.GetByIdAsync(id);
            if (membership == null)
            {
                throw new KeyNotFoundException($"No se encontró una membresía con ID {id}.");
            }

            return MapToDto(membership);
        }

        public async Task<int> AddAsync(MembershipCreateDTO dto)
        {
            ValidateDto(dto);
            await ValidateRelationsAsync(dto.MemberId, dto.LibraryId);

            var membership = new Membership
            {
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsActive = dto.IsActive,
                MemberId = dto.MemberId,
                LibraryId = dto.LibraryId
            };

            await _membershipRepository.AddAsync(membership);
            return membership.Id;
        }

        public async Task UpdateAsync(int id, MembershipCreateDTO dto)
        {
            ValidateId(id);
            ValidateDto(dto);
            await ValidateRelationsAsync(dto.MemberId, dto.LibraryId);

            var membership = await _membershipRepository.GetByIdAsync(id);
            if (membership == null)
            {
                throw new KeyNotFoundException($"No se encontró una membresía con ID {id}.");
            }

            membership.StartDate = dto.StartDate;
            membership.EndDate = dto.EndDate;
            membership.IsActive = dto.IsActive;
            membership.MemberId = dto.MemberId;
            membership.LibraryId = dto.LibraryId;

            await _membershipRepository.UpdateAsync(membership);
        }

        public async Task SetActiveAsync(int id, bool isActive)
        {
            ValidateId(id);

            var membership = await _membershipRepository.GetByIdAsync(id);
            if (membership == null)
            {
                throw new KeyNotFoundException($"No se encontró una membresía con ID {id}.");
            }

            membership.IsActive = isActive;
            if (!isActive && membership.EndDate == null)
            {
                membership.EndDate = DateTime.UtcNow;
            }

            await _membershipRepository.UpdateAsync(membership);
        }

        public async Task DeleteAsync(int id)
        {
            ValidateId(id);

            var membership = await _membershipRepository.GetByIdAsync(id);
            if (membership == null)
            {
                throw new KeyNotFoundException($"No se encontró una membresía con ID {id}.");
            }

            await _membershipRepository.DeleteAsync(membership);
        }

        private async Task ValidateRelationsAsync(string memberId, int libraryId)
        {
            if (!await _dbContext.Users.AnyAsync(u => u.Id == memberId))
            {
                throw new ArgumentException($"No existe un usuario con ID {memberId}.");
            }

            if (!await _libraryRepository.ExistsAsync(libraryId))
            {
                throw new ArgumentException($"No existe una biblioteca con ID {libraryId}.");
            }
        }

        private static void ValidateId(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("El ID debe ser mayor a cero.");
            }
        }

        private static void ValidateDto(MembershipCreateDTO dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            if (string.IsNullOrWhiteSpace(dto.MemberId))
            {
                throw new ArgumentException("MemberId es obligatorio.");
            }

            if (dto.LibraryId <= 0)
            {
                throw new ArgumentException("LibraryId debe ser mayor a cero.");
            }

            if (dto.EndDate.HasValue && dto.EndDate < dto.StartDate)
            {
                throw new ArgumentException("La fecha de fin no puede ser menor a la fecha de inicio.");
            }
        }

        private static MembershipDTO MapToDto(Membership membership)
        {
            return new MembershipDTO
            {
                Id = membership.Id,
                StartDate = membership.StartDate,
                EndDate = membership.EndDate,
                IsActive = membership.IsActive,
                MemberId = membership.MemberId,
                LibraryId = membership.LibraryId,
                LibraryName = membership.Library?.Name ?? string.Empty
            };
        }
    }
}
