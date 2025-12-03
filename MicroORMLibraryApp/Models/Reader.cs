// MicroORMLibraryApp/Models/Reader.cs
namespace MicroORMLibraryApp.Models
{
    public class Reader
    {
        public int ReaderId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateOnly RegistrationDate { get; set; }  // Змінив на DateOnly
        public bool IsActive { get; set; }
    }
}