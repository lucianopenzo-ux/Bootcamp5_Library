namespace TheLibrary.Common.DTOs
{
    public class LoanDTO
    {
        public int Id { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public bool IsReturned => ReturnDate.HasValue;
    }
}
