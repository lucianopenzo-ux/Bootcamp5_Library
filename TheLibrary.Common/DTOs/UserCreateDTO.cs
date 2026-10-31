namespace TheLibrary.Common.DTOs
{
    public class UserCreateDTO
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string Role { get; set; } = "User";
    }
}
