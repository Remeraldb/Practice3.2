// MicroORMLibraryApp/Models/ViewModels.cs
namespace MicroORMLibraryApp.Models
{
    public class BookWithAuthor
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public int? PublicationYear { get; set; }
        public decimal? Price { get; set; }
        public int? AuthorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? AuthorOrder { get; set; }
        public string AuthorNames => $"{FirstName} {LastName}".Trim();
    }

    public class AuthorWithBookCount
    {
        public int AuthorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
        public int BookCount { get; set; }
    }

    public class BorrowingWithDetails
    {
        public int BorrowingId { get; set; }
        public DateTime BorrowDate { get; set; }  // Лишаємо DateTime для зручності відображення
        public DateTime DueDate { get; set; }     // Лишаємо DateTime для зручності відображення
        public string Status { get; set; }
        public string Title { get; set; }
        public string ISBN { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }

    public class BookWithAuthorCount
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public int AuthorCount { get; set; }
    }

    public class ReaderWithOverdueCount
    {
        public int ReaderId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int OverdueCount { get; set; }
        public DateTime OldestOverdue { get; set; }  // Лишаємо DateTime
    }

    public class BooksStatistics
    {
        public int TotalBooks { get; set; }
        public decimal? AveragePrice { get; set; }
        public int? OldestYear { get; set; }
        public int? NewestYear { get; set; }
        public int? TotalPages { get; set; }
        public int GenreCount { get; set; }
    }
}