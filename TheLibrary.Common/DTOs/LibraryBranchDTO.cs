namespace TheLibrary.Common.DTOs
{
    public class LibraryBranchDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int LibraryId { get; set; }
        public string LibraryName { get; set; } = string.Empty;
    }
}
