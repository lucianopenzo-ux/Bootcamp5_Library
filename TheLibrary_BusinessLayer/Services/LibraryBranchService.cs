using TheLibrary.Common.DTOs;

using TheLibrary_DataAccessLayer.Models;
using TheLibrary_DataAccessLayer.Repositories;

namespace TheLibrary_BusinessLayer.Services
{
    public class LibraryBranchService : ILibraryBranchService
    {
        private readonly ILibraryBranchRepository _branchRepository;
        private readonly ILibraryRepository _libraryRepository;

        public LibraryBranchService(
            ILibraryBranchRepository branchRepository,
            ILibraryRepository libraryRepository)
        {
            _branchRepository = branchRepository;
            _libraryRepository = libraryRepository;
        }

        public async Task<List<LibraryBranchDTO>> GetAllAsync(int? libraryId = null)
        {
            var branches = libraryId.HasValue
                ? await _branchRepository.GetByLibraryIdAsync(libraryId.Value)
                : await _branchRepository.GetAllAsync();

            return branches.Select(MapToDto).ToList();
        }

        public async Task<LibraryBranchDTO> GetByIdAsync(int id)
        {
            ValidateId(id);

            var branch = await _branchRepository.GetByIdAsync(id);
            if (branch == null)
            {
                throw new KeyNotFoundException($"No se encontró una sucursal con ID {id}.");
            }

            return MapToDto(branch);
        }

        public async Task<int> AddAsync(LibraryBranchCreateDTO dto)
        {
            ValidateDto(dto);
            await ValidateLibraryExistsAsync(dto.LibraryId);

            var branch = new LibraryBranch
            {
                Name = dto.Name.Trim(),
                Address = dto.Address.Trim(),
                LibraryId = dto.LibraryId
            };

            await _branchRepository.AddAsync(branch);
            return branch.Id;
        }

        public async Task UpdateAsync(int id, LibraryBranchCreateDTO dto)
        {
            ValidateId(id);
            ValidateDto(dto);
            await ValidateLibraryExistsAsync(dto.LibraryId);

            var branch = await _branchRepository.GetByIdAsync(id);
            if (branch == null)
            {
                throw new KeyNotFoundException($"No se encontró una sucursal con ID {id}.");
            }

            branch.Name = dto.Name.Trim();
            branch.Address = dto.Address.Trim();
            branch.LibraryId = dto.LibraryId;

            await _branchRepository.UpdateAsync(branch);
        }

        public async Task DeleteAsync(int id)
        {
            ValidateId(id);

            var branch = await _branchRepository.GetByIdAsync(id);
            if (branch == null)
            {
                throw new KeyNotFoundException($"No se encontró una sucursal con ID {id}.");
            }

            await _branchRepository.DeleteAsync(branch);
        }

        private async Task ValidateLibraryExistsAsync(int libraryId)
        {
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

        private static void ValidateDto(LibraryBranchCreateDTO dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new ArgumentException("El nombre de la sucursal es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(dto.Address))
            {
                throw new ArgumentException("La dirección de la sucursal es obligatoria.");
            }

            if (dto.LibraryId <= 0)
            {
                throw new ArgumentException("LibraryId debe ser mayor a cero.");
            }
        }

        private static LibraryBranchDTO MapToDto(LibraryBranch branch)
        {
            return new LibraryBranchDTO
            {
                Id = branch.Id,
                Name = branch.Name,
                Address = branch.Address,
                LibraryId = branch.LibraryId,
                LibraryName = branch.Library?.Name ?? string.Empty
            };
        }
    }
}
