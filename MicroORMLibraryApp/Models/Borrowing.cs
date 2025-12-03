// MicroORMLibraryApp/Models/Borrowing.cs
namespace MicroORMLibraryApp.Models
{
    public class Borrowing
    {
        public int BorrowingId { get; set; }
        public int BookId { get; set; }
        public int ReaderId { get; set; }
        public DateOnly BorrowDate { get; set; }  // Змінив на DateOnly
        public DateOnly DueDate { get; set; }     // Змінив на DateOnly
        public DateOnly? ReturnDate { get; set; } // Змінив на DateOnly?
        public string Status { get; set; }
    }
}