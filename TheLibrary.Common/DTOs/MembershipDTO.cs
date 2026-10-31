namespace TheLibrary.Common.DTOs
{
    public class MembershipDTO
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public string MemberId { get; set; } = string.Empty;
        public int LibraryId { get; set; }
        public string LibraryName { get; set; } = string.Empty;
    }
}
