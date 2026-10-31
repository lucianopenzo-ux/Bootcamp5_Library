namespace TheLibrary.Common.DTOs
{
    public class LibraryBranchCreateDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int LibraryId { get; set; }
    }
}
