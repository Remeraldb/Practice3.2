// MicroORMLibraryApp/Models/Borrowing.cs
namespace MicroORMLibraryApp.Models
{
    public class Borrowing
    {
        public int BorrowingId { get; set; }
        public int BookId { get; set; }
        public int ReaderId { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string Status { get; set; }
    }
}