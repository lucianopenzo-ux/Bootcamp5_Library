namespace TheLibrary.Common.DTOs
{
    public class LoanCreateDTO
    {
        public DateTime LoanDate { get; set; }
        public DateTime? DueDate { get; set; }
        public int BookId { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}
