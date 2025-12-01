// MicroORMLibraryApp/Repository/LibraryRepository.cs
using System.Data;
using Dapper;
using MicroORMLibraryApp.Models;
using Npgsql;

namespace MicroORMLibraryApp.Repository
{
    public class LibraryRepository
    {
        private readonly string _connectionString;

        public LibraryRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);

        // 1. CRUD для Authors
        public IEnumerable<Author> GetAllAuthors()
        {
            using var conn = CreateConnection();
            return conn.Query<Author>("SELECT * FROM Authors ORDER BY LastName, FirstName");
        }

        public Author GetAuthorById(int id)
        {
            using var conn = CreateConnection();
            return conn.QueryFirstOrDefault<Author>("SELECT * FROM Authors WHERE AuthorId = @Id", new { Id = id });
        }

        public int CreateAuthor(Author author)
        {
            using var conn = CreateConnection();
            var sql = @"INSERT INTO Authors (FirstName, LastName, BirthDate, Country) 
                       VALUES (@FirstName, @LastName, @BirthDate, @Country) 
                       RETURNING AuthorId";
            return conn.ExecuteScalar<int>(sql, author);
        }

        public bool UpdateAuthor(Author author)
        {
            using var conn = CreateConnection();
            var sql = @"UPDATE Authors 
                       SET FirstName = @FirstName, LastName = @LastName, 
                           BirthDate = @BirthDate, Country = @Country 
                       WHERE AuthorId = @AuthorId";
            return conn.Execute(sql, author) > 0;
        }

        public bool DeleteAuthor(int id)
        {
            using var conn = CreateConnection();
            return conn.Execute("DELETE FROM Authors WHERE AuthorId = @Id", new { Id = id }) > 0;
        }

        // 2. CRUD для Books
        public IEnumerable<Book> GetAllBooks()
        {
            using var conn = CreateConnection();
            return conn.Query<Book>("SELECT * FROM Books ORDER BY Title");
        }

        public Book GetBookById(int id)
        {
            using var conn = CreateConnection();
            return conn.QueryFirstOrDefault<Book>("SELECT * FROM Books WHERE BookId = @Id", new { Id = id });
        }

        public int CreateBook(Book book)
        {
            using var conn = CreateConnection();
            var sql = @"INSERT INTO Books (Title, ISBN, PublicationYear, Genre, Publisher, PageCount, Price) 
                       VALUES (@Title, @ISBN, @PublicationYear, @Genre, @Publisher, @PageCount, @Price) 
                       RETURNING BookId";
            return conn.ExecuteScalar<int>(sql, book);
        }

        public bool UpdateBook(Book book)
        {
            using var conn = CreateConnection();
            var sql = @"UPDATE Books 
                       SET Title = @Title, ISBN = @ISBN, PublicationYear = @PublicationYear,
                           Genre = @Genre, Publisher = @Publisher, PageCount = @PageCount, Price = @Price
                       WHERE BookId = @BookId";
            return conn.Execute(sql, book) > 0;
        }

        public bool DeleteBook(int id)
        {
            using var conn = CreateConnection();
            return conn.Execute("DELETE FROM Books WHERE BookId = @Id", new { Id = id }) > 0;
        }

        // 3. CRUD для Readers
        public IEnumerable<Reader> GetAllReaders()
        {
            using var conn = CreateConnection();
            return conn.Query<Reader>("SELECT * FROM Readers ORDER BY LastName, FirstName");
        }

        public Reader GetReaderById(int id)
        {
            using var conn = CreateConnection();
            return conn.QueryFirstOrDefault<Reader>("SELECT * FROM Readers WHERE ReaderId = @Id", new { Id = id });
        }

        public int CreateReader(Reader reader)
        {
            using var conn = CreateConnection();
            var sql = @"INSERT INTO Readers (FirstName, LastName, Email, Phone) 
                       VALUES (@FirstName, @LastName, @Email, @Phone) 
                       RETURNING ReaderId";
            return conn.ExecuteScalar<int>(sql, reader);
        }

        public bool UpdateReader(Reader reader)
        {
            using var conn = CreateConnection();
            var sql = @"UPDATE Readers 
                       SET FirstName = @FirstName, LastName = @LastName, 
                           Email = @Email, Phone = @Phone, IsActive = @IsActive
                       WHERE ReaderId = @ReaderId";
            return conn.Execute(sql, reader) > 0;
        }

        public bool DeleteReader(int id)
        {
            using var conn = CreateConnection();
            return conn.Execute("DELETE FROM Readers WHERE ReaderId = @Id", new { Id = id }) > 0;
        }

        // 4. CRUD для Borrowings
        public IEnumerable<Borrowing> GetAllBorrowings()
        {
            using var conn = CreateConnection();
            return conn.Query<Borrowing>("SELECT * FROM Borrowings ORDER BY BorrowDate DESC");
        }

        public Borrowing GetBorrowingById(int id)
        {
            using var conn = CreateConnection();
            return conn.QueryFirstOrDefault<Borrowing>("SELECT * FROM Borrowings WHERE BorrowingId = @Id", new { Id = id });
        }

        public int CreateBorrowing(Borrowing borrowing)
        {
            using var conn = CreateConnection();
            var sql = @"INSERT INTO Borrowings (BookId, ReaderId, BorrowDate, DueDate, ReturnDate, Status) 
                       VALUES (@BookId, @ReaderId, @BorrowDate, @DueDate, @ReturnDate, @Status) 
                       RETURNING BorrowingId";
            return conn.ExecuteScalar<int>(sql, borrowing);
        }

        public bool UpdateBorrowing(Borrowing borrowing)
        {
            using var conn = CreateConnection();
            var sql = @"UPDATE Borrowings 
                       SET BookId = @BookId, ReaderId = @ReaderId, BorrowDate = @BorrowDate,
                           DueDate = @DueDate, ReturnDate = @ReturnDate, Status = @Status
                       WHERE BorrowingId = @BorrowingId";
            return conn.Execute(sql, borrowing) > 0;
        }

        public bool DeleteBorrowing(int id)
        {
            using var conn = CreateConnection();
            return conn.Execute("DELETE FROM Borrowings WHERE BorrowingId = @Id", new { Id = id }) > 0;
        }

        // ЗАПИТИ ЗА ВИМОГАМИ

        // 2.2.1. Запит з об'єднанням таблиць: Книги з авторами (JOIN)
        public IEnumerable<BookWithAuthor> GetBooksWithAuthors()
        {
            using var conn = CreateConnection();
            var sql = @"
                SELECT 
                    b.BookId, b.Title, b.Genre, b.PublicationYear, b.Price,
                    a.AuthorId, a.FirstName, a.LastName,
                    ba.AuthorOrder
                FROM Books b
                LEFT JOIN BookAuthors ba ON b.BookId = ba.BookId
                LEFT JOIN Authors a ON ba.AuthorId = a.AuthorId
                ORDER BY b.Title, ba.AuthorOrder";
            
            return conn.Query<BookWithAuthor>(sql);
        }

        // 2.2.2. Запит з фільтрацією: Книги за жанром (WHERE)
        public IEnumerable<Book> GetBooksByGenre(string genre)
        {
            using var conn = CreateConnection();
            return conn.Query<Book>(
                "SELECT * FROM Books WHERE Genre = @Genre ORDER BY Title",
                new { Genre = genre });
        }

        // 2.2.3. Запит з агрегатними функціями: Статистика по книгах (COUNT, AVG, MIN, MAX, SUM)
        public BooksStatistics GetBooksStatistics()
        {
            using var conn = CreateConnection();
            var sql = @"
                SELECT 
                    COUNT(*) as TotalBooks,
                    AVG(Price) as AveragePrice,
                    MIN(PublicationYear) as OldestYear,
                    MAX(PublicationYear) as NewestYear,
                    SUM(PageCount) as TotalPages,
                    COUNT(DISTINCT Genre) as GenreCount
                FROM Books";
            
            return conn.QueryFirstOrDefault<BooksStatistics>(sql);
        }

        // Автори з кількістю книг (GROUP BY)
        public IEnumerable<AuthorWithBookCount> GetAuthorsWithBookCount()
        {
            using var conn = CreateConnection();
            var sql = @"
                SELECT 
                    a.AuthorId, a.FirstName, a.LastName, a.Country,
                    COUNT(ba.BookId) as BookCount
                FROM Authors a
                LEFT JOIN BookAuthors ba ON a.AuthorId = ba.AuthorId
                GROUP BY a.AuthorId
                ORDER BY BookCount DESC";
            
            return conn.Query<AuthorWithBookCount>(sql);
        }

        // Книги з кількома авторами (HAVING)
        public IEnumerable<BookWithAuthorCount> GetBooksWithMultipleAuthors()
        {
            using var conn = CreateConnection();
            var sql = @"
                SELECT 
                    b.BookId, b.Title, b.Genre,
                    COUNT(ba.AuthorId) as AuthorCount
                FROM Books b
                JOIN BookAuthors ba ON b.BookId = ba.BookId
                JOIN Authors a ON ba.AuthorId = a.AuthorId
                GROUP BY b.BookId, b.Title, b.Genre
                HAVING COUNT(ba.AuthorId) > 1
                ORDER BY AuthorCount DESC";
            
            return conn.Query<BookWithAuthorCount>(sql);
        }

        // Поточні позичення з інформацією про читачів та книги (Множинне JOIN)
        public IEnumerable<BorrowingWithDetails> GetCurrentBorrowings()
        {
            using var conn = CreateConnection();
            var sql = @"
                SELECT 
                    br.BorrowingId, br.BorrowDate, br.DueDate, br.Status,
                    b.Title, b.ISBN,
                    r.FirstName, r.LastName, r.Email
                FROM Borrowings br
                JOIN Books b ON br.BookId = b.BookId
                JOIN Readers r ON br.ReaderId = r.ReaderId
                WHERE br.ReturnDate IS NULL
                ORDER BY br.DueDate";
            
            return conn.Query<BorrowingWithDetails>(sql);
        }

        // Читачі з простроченими книгами (WHERE з умовою дати)
        public IEnumerable<ReaderWithOverdueCount> GetReadersWithOverdueBooks()
        {
            using var conn = CreateConnection();
            var sql = @"
                SELECT 
                    r.ReaderId, r.FirstName, r.LastName, r.Email,
                    COUNT(br.BorrowingId) as OverdueCount,
                    MIN(br.DueDate) as OldestOverdue
                FROM Readers r
                JOIN Borrowings br ON r.ReaderId = br.ReaderId
                WHERE br.Status = 'Overdue' 
                  AND br.ReturnDate IS NULL
                  AND br.DueDate < CURRENT_DATE
                GROUP BY r.ReaderId, r.FirstName, r.LastName, r.Email
                ORDER BY OverdueCount DESC";
            
            return conn.Query<ReaderWithOverdueCount>(sql);
        }

        // Пошук книг за назвою (LIKE)
        public IEnumerable<Book> SearchBooks(string searchTerm)
        {
            using var conn = CreateConnection();
            return conn.Query<Book>(
                "SELECT * FROM Books WHERE Title ILIKE @SearchTerm OR ISBN ILIKE @SearchTerm ORDER BY Title",
                new { SearchTerm = $"%{searchTerm}%" });
        }

        // Книги конкретного автора
        public IEnumerable<Book> GetBooksByAuthor(int authorId)
        {
            using var conn = CreateConnection();
            var sql = @"
                SELECT b.* FROM Books b
                JOIN BookAuthors ba ON b.BookId = ba.BookId
                WHERE ba.AuthorId = @AuthorId
                ORDER BY b.Title";
            
            return conn.Query<Book>(sql, new { AuthorId = authorId });
        }

        // Перевірка каскадного видалення
        public bool TestCascadeDeleteAuthor(int authorId)
        {
            using var conn = CreateConnection();
            return conn.Execute("DELETE FROM Authors WHERE AuthorId = @Id", new { Id = authorId }) > 0;
        }

        // Книги без авторів (для перевірки каскадного видалення)
        public IEnumerable<Book> GetBooksWithoutAuthors()
        {
            using var conn = CreateConnection();
            var sql = @"
                SELECT b.* FROM Books b
                LEFT JOIN BookAuthors ba ON b.BookId = ba.BookId
                WHERE ba.BookAuthorId IS NULL";
            
            return conn.Query<Book>(sql);
        }
    }
}