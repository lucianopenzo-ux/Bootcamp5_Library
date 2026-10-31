namespace TheLibrary.Common.DTOs
{
    public class MembershipCreateDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public string MemberId { get; set; } = string.Empty;
        public int LibraryId { get; set; }
    }
}
