using Microsoft.AspNetCore.Identity;

using TheLibrary_BusinessLayer.DTOs;
using TheLibrary_DataAccessLayer.Models;

namespace TheLibrary_BusinessLayer.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IJwtService _jwtService;
        private readonly UserManager<User> _userManager;

        public AuthService(IJwtService jwtService, UserManager<User> userManager)
        {
            _jwtService = jwtService;
            _userManager = userManager;
        }

        public async Task<AuthResponseDTO> RegisterAsync(RegisterDTO registerDto)
        {
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                return new AuthResponseDTO { Token = null };
            }

            var user = new User
            {
                UserName = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                return new AuthResponseDTO { Token = null };
            }

            await _userManager.AddToRoleAsync(user, "User");
            var token = await _jwtService.GenerateJwtTokenAsync(user, new List<string> { "User" });
            return new AuthResponseDTO { Token = token };
        }

        public async Task<AuthResponseDTO> LoginAsync(LoginDTO loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                return new AuthResponseDTO { Token = null };
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = await _jwtService.GenerateJwtTokenAsync(user, roles);
            return new AuthResponseDTO { Token = token };
        }
    }
}
