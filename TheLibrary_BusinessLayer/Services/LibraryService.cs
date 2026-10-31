using TheLibrary.Common.DTOs;

using TheLibrary_DataAccessLayer.Models;
using TheLibrary_DataAccessLayer.Repositories;

namespace TheLibrary_BusinessLayer.Services
{
    public class LibraryService : ILibraryService
    {
        private readonly ILibraryRepository _libraryRepository;

        public LibraryService(ILibraryRepository libraryRepository)
        {
            _libraryRepository = libraryRepository;
        }

        public async Task<List<LibraryDTO>> GetAllAsync()
        {
            var libraries = await _libraryRepository.GetAllAsync();
            return libraries.Select(MapToDto).ToList();
        }

        public async Task<LibraryDTO> GetByIdAsync(int id)
        {
            ValidateId(id);

            var library = await _libraryRepository.GetByIdAsync(id);
            if (library == null)
            {
                throw new KeyNotFoundException($"No se encontró una biblioteca con ID {id}.");
            }

            return MapToDto(library);
        }

        public async Task<int> AddAsync(LibraryCreateDTO dto)
        {
            ValidateDto(dto);

            var library = new Library
            {
                Name = dto.Name.Trim(),
                Address = dto.Address.Trim()
            };

            await _libraryRepository.AddAsync(library);
            return library.Id;
        }

        public async Task UpdateAsync(int id, LibraryCreateDTO dto)
        {
            ValidateId(id);
            ValidateDto(dto);

            var library = await _libraryRepository.GetByIdAsync(id);
            if (library == null)
            {
                throw new KeyNotFoundException($"No se encontró una biblioteca con ID {id}.");
            }

            library.Name = dto.Name.Trim();
            library.Address = dto.Address.Trim();

            await _libraryRepository.UpdateAsync(library);
        }

        public async Task DeleteAsync(int id)
        {
            ValidateId(id);

            var library = await _libraryRepository.GetByIdAsync(id);
            if (library == null)
            {
                throw new KeyNotFoundException($"No se encontró una biblioteca con ID {id}.");
            }

            await _libraryRepository.DeleteAsync(library);
        }

        private static void ValidateId(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("El ID debe ser mayor a cero.");
            }
        }

        private static void ValidateDto(LibraryCreateDTO dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new ArgumentException("El nombre de la biblioteca es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(dto.Address))
            {
                throw new ArgumentException("La dirección de la biblioteca es obligatoria.");
            }
        }

        private static LibraryDTO MapToDto(Library library)
        {
            return new LibraryDTO
            {
                Id = library.Id,
                Name = library.Name,
                Address = library.Address
            };
        }
    }
}
