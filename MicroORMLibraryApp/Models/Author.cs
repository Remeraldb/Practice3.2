// MicroORMLibraryApp/Models/Author.cs
namespace MicroORMLibraryApp.Models
{
    public class Author
    {
        public int AuthorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateOnly? BirthDate { get; set; }  // Змінив на DateOnly?
        public string Country { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}