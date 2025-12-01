// MicroORMLibraryApp/Models/Author.cs
namespace MicroORMLibraryApp.Models
{
    public class Author
    {
        public int AuthorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Country { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}