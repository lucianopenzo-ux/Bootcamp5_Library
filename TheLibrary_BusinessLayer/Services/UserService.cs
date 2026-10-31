using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using TheLibrary.Common.DTOs;

using TheLibrary_DataAccessLayer.Models;

namespace TheLibrary_BusinessLayer.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<PagedResult<UserDTO>> GetAllAsync(int pageIndex, int pageSize, string? search)
        {
            if (pageIndex <= 0)
            {
                throw new ArgumentException("pageIndex debe ser mayor a cero.");
            }

            if (pageSize <= 0)
            {
                throw new ArgumentException("pageSize debe ser mayor a cero.");
            }

            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(u =>
                    u.Email!.ToLower().Contains(term) ||
                    u.FirstName.ToLower().Contains(term) ||
                    u.LastName.ToLower().Contains(term));
            }

            var totalCount = await query.CountAsync();

            var users = await query
                .OrderBy(u => u.Email)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new List<UserDTO>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                result.Add(MapToDto(user, roles.ToList()));
            }

            return new PagedResult<UserDTO>
            {
                Items = result,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<UserDTO> GetByIdAsync(string id)
        {
            ValidateId(id);

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException($"No se encontró un usuario con ID {id}.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            return MapToDto(user, roles.ToList());
        }

        public async Task<string> CreateAsync(UserCreateDTO dto)
        {
            ValidateCreateDto(dto);

            var existing = await _userManager.FindByEmailAsync(dto.Email.Trim());
            if (existing != null)
            {
                throw new ArgumentException("Ya existe un usuario con ese email.");
            }

            var user = new User
            {
                UserName = dto.Email.Trim(),
                Email = dto.Email.Trim(),
                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                DateOfBirth = dto.DateOfBirth
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(string.Join(" | ", result.Errors.Select(e => e.Description)));
            }

            var role = string.IsNullOrWhiteSpace(dto.Role) ? "User" : dto.Role.Trim();
            if (!await _roleManager.RoleExistsAsync(role))
            {
                throw new ArgumentException($"El rol '{role}' no existe.");
            }

            await _userManager.AddToRoleAsync(user, role);
            return user.Id;
        }

        public async Task UpdateAsync(string id, UserUpdateDTO dto)
        {
            ValidateId(id);
            ValidateUpdateDto(dto);

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException($"No se encontró un usuario con ID {id}.");
            }

            var normalizedEmail = dto.Email.Trim().ToUpperInvariant();
            var emailUsedByAnotherUser = await _userManager.Users.AnyAsync(u => u.NormalizedEmail == normalizedEmail && u.Id != id);
            if (emailUsedByAnotherUser)
            {
                throw new ArgumentException("El email ya está en uso por otro usuario.");
            }

            user.FirstName = dto.FirstName.Trim();
            user.LastName = dto.LastName.Trim();
            user.Email = dto.Email.Trim();
            user.UserName = dto.Email.Trim();
            user.DateOfBirth = dto.DateOfBirth;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(string.Join(" | ", result.Errors.Select(e => e.Description)));
            }
        }

        public async Task DeleteAsync(string id)
        {
            ValidateId(id);

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException($"No se encontró un usuario con ID {id}.");
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(string.Join(" | ", result.Errors.Select(e => e.Description)));
            }
        }

        private static void ValidateId(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("El ID es obligatorio.");
            }
        }

        private static void ValidateCreateDto(UserCreateDTO dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            if (string.IsNullOrWhiteSpace(dto.FirstName))
            {
                throw new ArgumentException("FirstName es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(dto.LastName))
            {
                throw new ArgumentException("LastName es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                throw new ArgumentException("Email es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                throw new ArgumentException("Password es obligatorio.");
            }
        }

        private static void ValidateUpdateDto(UserUpdateDTO dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            if (string.IsNullOrWhiteSpace(dto.FirstName))
            {
                throw new ArgumentException("FirstName es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(dto.LastName))
            {
                throw new ArgumentException("LastName es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                throw new ArgumentException("Email es obligatorio.");
            }
        }

        private static UserDTO MapToDto(User user, List<string> roles)
        {
            return new UserDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty,
                DateOfBirth = user.DateOfBirth,
                Roles = roles
            };
        }
    }
}
