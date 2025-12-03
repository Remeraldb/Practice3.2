// MicroORMLibraryApp/Repository/LibraryRepository.cs
using System.Data;
using Dapper;
using MicroORMLibraryApp.Models;
using Npgsql;
using System.Globalization;

namespace MicroORMLibraryApp.Repository
{
    public class LibraryRepository
    {
        private readonly string _connectionString;

        static LibraryRepository()
        {
            // Ініціалізуємо Dapper конфігурацію
            DapperConfig.Configure();
        }

        public LibraryRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);

        // Метод для конвертації дат
        private DateOnly? ParseDateOnly(object value)
        {
            if (value == null || value is DBNull)
                return null;

            if (value is DateOnly dateOnly)
                return dateOnly;

            if (value is DateTime dateTime)
                return DateOnly.FromDateTime(dateTime);

            if (value is string str)
            {
                var formats = new[] { "dd.MM.yyyy", "dd/MM/yyyy", "yyyy-MM-dd", "MM/dd/yyyy", "dd-MM-yyyy" };
                foreach (var format in formats)
                {
                    if (DateOnly.TryParseExact(str, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                        return result;
                }
                
                if (DateOnly.TryParse(str, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
                    return parsed;
            }

            return null;
        }

        private DateTime? ParseDateTime(object value)
        {
            var dateOnly = ParseDateOnly(value);
            return dateOnly?.ToDateTime(TimeOnly.MinValue);
        }

        // Метод для маппінгу рядка в об'єкт
        private T MapRowToObject<T>(dynamic row) where T : new()
        {
            var obj = new T();
            var properties = typeof(T).GetProperties();
            var rowDict = (IDictionary<string, object>)row;

            foreach (var prop in properties)
            {
                var key = prop.Name.ToLower();
                if (rowDict.TryGetValue(key, out var value))
                {
                    if (value is DBNull)
                        continue;

                    // Особлива обробка для дат
                    if (prop.PropertyType == typeof(DateOnly) || prop.PropertyType == typeof(DateOnly?))
                    {
                        var dateOnlyValue = ParseDateOnly(value);
                        if (dateOnlyValue.HasValue || prop.PropertyType == typeof(DateOnly?))
                            prop.SetValue(obj, dateOnlyValue);
                        else if (prop.PropertyType == typeof(DateOnly))
                            prop.SetValue(obj, DateOnly.MinValue);
                    }
                    else if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
                    {
                        var dateTimeValue = ParseDateTime(value);
                        if (dateTimeValue.HasValue || prop.PropertyType == typeof(DateTime?))
                            prop.SetValue(obj, dateTimeValue);
                        else if (prop.PropertyType == typeof(DateTime))
                            prop.SetValue(obj, DateTime.MinValue);
                    }
                    // Особлива обробка для нульових значень int та decimal
                    else if (prop.PropertyType == typeof(int) && value != null)
                    {
                        try
                        {
                            prop.SetValue(obj, Convert.ToInt32(value));
                        }
                        catch
                        {
                            // Залишаємо значення за замовчуванням (0)
                        }
                    }
                    else if (prop.PropertyType == typeof(int?) && value != null)
                    {
                        try
                        {
                            prop.SetValue(obj, Convert.ToInt32(value));
                        }
                        catch
                        {
                            // Встановлюємо null
                            prop.SetValue(obj, null);
                        }
                    }
                    else if (prop.PropertyType == typeof(decimal) && value != null)
                    {
                        try
                        {
                            prop.SetValue(obj, Convert.ToDecimal(value));
                        }
                        catch
                        {
                            // Залишаємо значення за замовчуванням (0)
                        }
                    }
                    else if (prop.PropertyType == typeof(decimal?) && value != null)
                    {
                        try
                        {
                            prop.SetValue(obj, Convert.ToDecimal(value));
                        }
                        catch
                        {
                            // Встановлюємо null
                            prop.SetValue(obj, null);
                        }
                    }
                    else
                    {
                        try
                        {
                            prop.SetValue(obj, Convert.ChangeType(value, prop.PropertyType));
                        }
                        catch
                        {
                            // Якщо не вдається конвертувати, залишаємо значення за замовчуванням
                        }
                    }
                }
            }

            return obj;
        }

        // Допоміжний метод для перетворення IEnumerable<dynamic> в IEnumerable<T>
        private IEnumerable<T> MapDynamicToType<T>(IEnumerable<dynamic> rows) where T : new()
        {
            var result = new List<T>();
            foreach (var row in rows)
            {
                result.Add(MapRowToObject<T>(row));
            }
            return result;
        }

        // 1. CRUD для Authors
        public IEnumerable<Author> GetAllAuthors()
        {
            using var conn = CreateConnection();
            var rows = conn.Query("SELECT * FROM Authors ORDER BY LastName, FirstName");
            return MapDynamicToType<Author>(rows);
        }

        public Author? GetAuthorById(int id)
        {
            using var conn = CreateConnection();
            var row = conn.QueryFirstOrDefault("SELECT * FROM Authors WHERE AuthorId = @Id", new { Id = id });
            return row == null ? null : MapRowToObject<Author>(row);
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
            var rows = conn.Query("SELECT * FROM Books ORDER BY Title");
            return MapDynamicToType<Book>(rows);
        }

        public Book? GetBookById(int id)
        {
            using var conn = CreateConnection();
            var row = conn.QueryFirstOrDefault("SELECT * FROM Books WHERE BookId = @Id", new { Id = id });
            return row == null ? null : MapRowToObject<Book>(row);
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
            var rows = conn.Query("SELECT * FROM Readers ORDER BY LastName, FirstName");
            return MapDynamicToType<Reader>(rows);
        }

        public Reader? GetReaderById(int id)
        {
            using var conn = CreateConnection();
            var row = conn.QueryFirstOrDefault("SELECT * FROM Readers WHERE ReaderId = @Id", new { Id = id });
            return row == null ? null : MapRowToObject<Reader>(row);
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
            var rows = conn.Query("SELECT * FROM Borrowings ORDER BY BorrowDate DESC");
            return MapDynamicToType<Borrowing>(rows);
        }

        public Borrowing? GetBorrowingById(int id)
        {
            using var conn = CreateConnection();
            var row = conn.QueryFirstOrDefault("SELECT * FROM Borrowings WHERE BorrowingId = @Id", new { Id = id });
            return row == null ? null : MapRowToObject<Borrowing>(row);
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
                    b.BookId, 
                    b.Title, 
                    b.Genre, 
                    b.PublicationYear, 
                    b.Price,
                    STRING_AGG(CONCAT(a.FirstName, ' ', a.LastName), ', ' ORDER BY ba.AuthorOrder) as AuthorNames
                FROM Books b
                LEFT JOIN BookAuthors ba ON b.BookId = ba.BookId
                LEFT JOIN Authors a ON ba.AuthorId = a.AuthorId
                GROUP BY b.BookId, b.Title, b.Genre, b.PublicationYear, b.Price
                ORDER BY b.Title";
    
            var rows = conn.Query(sql);
    
            return rows.Select(row => new BookWithAuthor
            {
                BookId = row.bookid is int bookId ? bookId : 0,
                Title = row.title?.ToString() ?? string.Empty,
                Genre = row.genre?.ToString(),
                PublicationYear = row.publicationyear as int?,
                Price = row.price as decimal?,
                FirstName = row.authornames?.ToString() ?? "Без автора",
                LastName = "", // Clear last name
                AuthorOrder = 1 // Default
            });
        }

        // 2.2.2. Запит з фільтрацією: Книги за жанром (WHERE)
        public IEnumerable<Book> GetBooksByGenre(string genre)
        {
            using var conn = CreateConnection();
            var rows = conn.Query("SELECT * FROM Books WHERE Genre = @Genre ORDER BY Title", new { Genre = genre });
            return MapDynamicToType<Book>(rows);
        }

        // 2.2.3. Запит з агрегатними функціями: Статистика по книгах (COUNT, AVG, MIN, MAX, SUM)
        public BooksStatistics GetBooksStatistics()
        {
            using var conn = CreateConnection();
            var sql = @"
            SELECT 
                COUNT(*) as TotalBooks,
                COALESCE(AVG(Price), 0) as AveragePrice,
                MIN(PublicationYear) as OldestYear,
                MAX(PublicationYear) as NewestYear,
                COALESCE(SUM(PageCount), 0) as TotalPages,
                COUNT(DISTINCT Genre) as GenreCount
            FROM Books
            WHERE Genre IS NOT NULL";
    
            var row = conn.QueryFirstOrDefault(sql);
    
            if (row == null)
                return new BooksStatistics();

            return new BooksStatistics
            {
                TotalBooks = Convert.ToInt32(row.totalbooks),
                AveragePrice = row.averageprice as decimal?,
                OldestYear = row.oldestyear as int?,
                NewestYear = row.newestyear as int?,
                TotalPages = Convert.ToInt32(row.totalpages), // Fixed: convert to int
                GenreCount = Convert.ToInt32(row.genrecount)
            };
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
            
            var rows = conn.Query(sql);
            
            return rows.Select(row => new AuthorWithBookCount
            {
                AuthorId = row.authorid is int authorId ? authorId : 0,
                FirstName = row.firstname?.ToString() ?? string.Empty,
                LastName = row.lastname?.ToString() ?? string.Empty,
                Country = row.country?.ToString(),
                BookCount = row.bookcount != null ? Convert.ToInt32(row.bookcount) : 0
            });
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
            
            var rows = conn.Query(sql);
            
            return rows.Select(row => new BookWithAuthorCount
            {
                BookId = row.bookid is int bookId ? bookId : 0,
                Title = row.title?.ToString() ?? string.Empty,
                Genre = row.genre?.ToString(),
                AuthorCount = row.authorcount != null ? Convert.ToInt32(row.authorcount) : 0
            });
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
            
            var rows = conn.Query(sql);
            
            return rows.Select(row => new BorrowingWithDetails
            {
                BorrowingId = row.borrowingid is int borrowingId ? borrowingId : 0,
                BorrowDate = ParseDateTime(row.borrowdate) ?? DateTime.MinValue,
                DueDate = ParseDateTime(row.duedate) ?? DateTime.MinValue,
                Status = row.status?.ToString() ?? string.Empty,
                Title = row.title?.ToString() ?? string.Empty,
                ISBN = row.isbn?.ToString(),
                FirstName = row.firstname?.ToString() ?? string.Empty,
                LastName = row.lastname?.ToString() ?? string.Empty,
                Email = row.email?.ToString()
            });
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
            
            var rows = conn.Query(sql);
            
            return rows.Select(row => new ReaderWithOverdueCount
            {
                ReaderId = row.readerid is int readerId ? readerId : 0,
                FirstName = row.firstname?.ToString() ?? string.Empty,
                LastName = row.lastname?.ToString() ?? string.Empty,
                Email = row.email?.ToString(),
                OverdueCount = row.overduecount != null ? Convert.ToInt32(row.overduecount) : 0,
                OldestOverdue = ParseDateTime(row.oldestoverdue) ?? DateTime.MinValue
            });
        }

        // Пошук книг за назвою (LIKE)
        public IEnumerable<Book> SearchBooks(string searchTerm)
        {
            using var conn = CreateConnection();
            var rows = conn.Query(
                "SELECT * FROM Books WHERE Title ILIKE @SearchTerm OR ISBN ILIKE @SearchTerm ORDER BY Title",
                new { SearchTerm = $"%{searchTerm}%" });
            return MapDynamicToType<Book>(rows);
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
            
            var rows = conn.Query(sql, new { AuthorId = authorId });
            return MapDynamicToType<Book>(rows);
        }

        // Метод для скидання бази даних
        public bool ResetDatabase()
        {
            try
            {
                using var conn = CreateConnection();
        
                // Просто виконаємо весь init.sql скрипт
                var initSqlPath = "init.sql";
                if (File.Exists(initSqlPath))
                {
                    var sql = File.ReadAllText(initSqlPath);
                    conn.Execute(sql);
                    return true;
                }
        
                return false;
            }
            catch
            {
                return false;
            }
        }

        // Книги без авторів (для перевірки каскадного видалення)
        public IEnumerable<Book> GetBooksWithoutAuthors()
        {
            using var conn = CreateConnection();
            var sql = @"
                SELECT b.* FROM Books b
                LEFT JOIN BookAuthors ba ON b.BookId = ba.BookId
                WHERE ba.BookAuthorId IS NULL";
            
            var rows = conn.Query(sql);
            return MapDynamicToType<Book>(rows);
        }

        // Додаткові методи для зручності

        // Отримати всі жанри книг
        public IEnumerable<string> GetAllGenres()
        {
            using var conn = CreateConnection();
            return conn.Query<string>("SELECT DISTINCT Genre FROM Books WHERE Genre IS NOT NULL ORDER BY Genre");
        }

        // Отримати книги за роком видання
        public IEnumerable<Book> GetBooksByPublicationYear(int year)
        {
            using var conn = CreateConnection();
            var rows = conn.Query("SELECT * FROM Books WHERE PublicationYear = @Year ORDER BY Title", 
                new { Year = year });
            return MapDynamicToType<Book>(rows);
        }

        // Отримати позичення за читачем
        public IEnumerable<Borrowing> GetBorrowingsByReader(int readerId)
        {
            using var conn = CreateConnection();
            var rows = conn.Query("SELECT * FROM Borrowings WHERE ReaderId = @ReaderId ORDER BY BorrowDate DESC", 
                new { ReaderId = readerId });
            return MapDynamicToType<Borrowing>(rows);
        }

        // Отримати позичення за книгою
        public IEnumerable<Borrowing> GetBorrowingsByBook(int bookId)
        {
            using var conn = CreateConnection();
            var rows = conn.Query("SELECT * FROM Borrowings WHERE BookId = @BookId ORDER BY BorrowDate DESC", 
                new { BookId = bookId });
            return MapDynamicToType<Borrowing>(rows);
        }

        // Отримати активних читачів
        public IEnumerable<Reader> GetActiveReaders()
        {
            using var conn = CreateConnection();
            var rows = conn.Query("SELECT * FROM Readers WHERE IsActive = true ORDER BY LastName, FirstName");
            return MapDynamicToType<Reader>(rows);
        }

        // Отримати книги з найвищою ціною (TOP 10)
        public IEnumerable<Book> GetMostExpensiveBooks(int limit = 10)
        {
            using var conn = CreateConnection();
            var rows = conn.Query("SELECT * FROM Books WHERE Price IS NOT NULL ORDER BY Price DESC LIMIT @Limit", 
                new { Limit = limit });
            return MapDynamicToType<Book>(rows);
        }

        // Отримати книги з найнижчою ціною (TOP 10)
        public IEnumerable<Book> GetCheapestBooks(int limit = 10)
        {
            using var conn = CreateConnection();
            var rows = conn.Query("SELECT * FROM Books WHERE Price IS NOT NULL ORDER BY Price ASC LIMIT @Limit", 
                new { Limit = limit });
            return MapDynamicToType<Book>(rows);
        }

        // Отримати найновіші книги (за роком видання)
        public IEnumerable<Book> GetNewestBooks(int limit = 10)
        {
            using var conn = CreateConnection();
            var rows = conn.Query("SELECT * FROM Books WHERE PublicationYear IS NOT NULL ORDER BY PublicationYear DESC LIMIT @Limit", 
                new { Limit = limit });
            return MapDynamicToType<Book>(rows);
        }

        // Отримати найстаріші книги (за роком видання)
        public IEnumerable<Book> GetOldestBooks(int limit = 10)
        {
            using var conn = CreateConnection();
            var rows = conn.Query("SELECT * FROM Books WHERE PublicationYear IS NOT NULL ORDER BY PublicationYear ASC LIMIT @Limit", 
                new { Limit = limit });
            return MapDynamicToType<Book>(rows);
        }

        // Отримати книги з найбільшою кількістю сторінок
        public IEnumerable<Book> GetLongestBooks(int limit = 10)
        {
            using var conn = CreateConnection();
            var rows = conn.Query("SELECT * FROM Books WHERE PageCount IS NOT NULL ORDER BY PageCount DESC LIMIT @Limit", 
                new { Limit = limit });
            return MapDynamicToType<Book>(rows);
        }

        // Отримати книги з найменшою кількістю сторінок
        public IEnumerable<Book> GetShortestBooks(int limit = 10)
        {
            using var conn = CreateConnection();
            var rows = conn.Query("SELECT * FROM Books WHERE PageCount IS NOT NULL ORDER BY PageCount ASC LIMIT @Limit", 
                new { Limit = limit });
            return MapDynamicToType<Book>(rows);
        }

        // Отримати статистику по позиченнях
        public dynamic GetBorrowingsStatistics()
        {
            using var conn = CreateConnection();
            var sql = @"
                SELECT 
                    COUNT(*) as TotalBorrowings,
                    COUNT(CASE WHEN ReturnDate IS NULL THEN 1 END) as ActiveBorrowings,
                    COUNT(CASE WHEN Status = 'Overdue' THEN 1 END) as OverdueBorrowings,
                    COUNT(CASE WHEN Status = 'Returned' THEN 1 END) as ReturnedBorrowings,
                    MIN(BorrowDate) as FirstBorrowDate,
                    MAX(BorrowDate) as LastBorrowDate
                FROM Borrowings";
            
            return conn.QueryFirstOrDefault<dynamic>(sql) ?? new { };
        }

        // Отримати статистику по читачах
        public dynamic GetReadersStatistics()
        {
            using var conn = CreateConnection();
            var sql = @"
                SELECT 
                    COUNT(*) as TotalReaders,
                    COUNT(CASE WHEN IsActive = true THEN 1 END) as ActiveReaders,
                    COUNT(CASE WHEN IsActive = false THEN 1 END) as InactiveReaders,
                    MIN(RegistrationDate) as FirstRegistration,
                    MAX(RegistrationDate) as LastRegistration
                FROM Readers";
            
            return conn.QueryFirstOrDefault<dynamic>(sql) ?? new { };
        }

        // Пошук авторів за іменем або прізвищем
        public IEnumerable<Author> SearchAuthors(string searchTerm)
        {
            using var conn = CreateConnection();
            var rows = conn.Query(
                "SELECT * FROM Authors WHERE FirstName ILIKE @SearchTerm OR LastName ILIKE @SearchTerm OR Country ILIKE @SearchTerm ORDER BY LastName, FirstName",
                new { SearchTerm = $"%{searchTerm}%" });
            return MapDynamicToType<Author>(rows);
        }

        // Пошук читачів за іменем, прізвищем або email
        public IEnumerable<Reader> SearchReaders(string searchTerm)
        {
            using var conn = CreateConnection();
            var rows = conn.Query(
                "SELECT * FROM Readers WHERE FirstName ILIKE @SearchTerm OR LastName ILIKE @SearchTerm OR Email ILIKE @SearchTerm ORDER BY LastName, FirstName",
                new { SearchTerm = $"%{searchTerm}%" });
            return MapDynamicToType<Reader>(rows);
        }

        // Отримати книги за видавництвом
        public IEnumerable<Book> GetBooksByPublisher(string publisher)
        {
            using var conn = CreateConnection();
            var rows = conn.Query(
                "SELECT * FROM Books WHERE Publisher ILIKE @Publisher ORDER BY Title",
                new { Publisher = $"%{publisher}%" });
            return MapDynamicToType<Book>(rows);
        }

        // Отримати всіх авторів певної книги
        public IEnumerable<Author> GetAuthorsOfBook(int bookId)
        {
            using var conn = CreateConnection();
            var sql = @"
                SELECT a.* FROM Authors a
                JOIN BookAuthors ba ON a.AuthorId = ba.AuthorId
                WHERE ba.BookId = @BookId
                ORDER BY ba.AuthorOrder";
            
            var rows = conn.Query(sql, new { BookId = bookId });
            return MapDynamicToType<Author>(rows);
        }

        // Отримати всіх авторів з певної країни
        public IEnumerable<Author> GetAuthorsByCountry(string country)
        {
            using var conn = CreateConnection();
            var rows = conn.Query(
                "SELECT * FROM Authors WHERE Country ILIKE @Country ORDER BY LastName, FirstName",
                new { Country = $"%{country}%" });
            return MapDynamicToType<Author>(rows);
        }

        // Отримати книги, що ще не повернуті (позичені або прострочені)
        public IEnumerable<BorrowingWithDetails> GetNotReturnedBorrowings()
        {
            return GetCurrentBorrowings(); // Це той самий запит
        }

        // Отримати книги, які були повернуті вчасно
        public IEnumerable<BorrowingWithDetails> GetReturnedOnTimeBorrowings()
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
                WHERE br.Status = 'Returned' 
                  AND br.ReturnDate <= br.DueDate
                ORDER BY br.ReturnDate DESC";
            
            var rows = conn.Query(sql);
            
            return rows.Select(row => new BorrowingWithDetails
            {
                BorrowingId = row.borrowingid is int borrowingId ? borrowingId : 0,
                BorrowDate = ParseDateTime(row.borrowdate) ?? DateTime.MinValue,
                DueDate = ParseDateTime(row.duedate) ?? DateTime.MinValue,
                Status = row.status?.ToString() ?? string.Empty,
                Title = row.title?.ToString() ?? string.Empty,
                ISBN = row.isbn?.ToString(),
                FirstName = row.firstname?.ToString() ?? string.Empty,
                LastName = row.lastname?.ToString() ?? string.Empty,
                Email = row.email?.ToString()
            });
        }

        // Отримати книги, які були повернуті з запізненням
        public IEnumerable<BorrowingWithDetails> GetReturnedLateBorrowings()
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
                WHERE br.Status = 'Returned' 
                  AND br.ReturnDate > br.DueDate
                ORDER BY br.ReturnDate DESC";
            
            var rows = conn.Query(sql);
            
            return rows.Select(row => new BorrowingWithDetails
            {
                BorrowingId = row.borrowingid is int borrowingId ? borrowingId : 0,
                BorrowDate = ParseDateTime(row.borrowdate) ?? DateTime.MinValue,
                DueDate = ParseDateTime(row.duedate) ?? DateTime.MinValue,
                Status = row.status?.ToString() ?? string.Empty,
                Title = row.title?.ToString() ?? string.Empty,
                ISBN = row.isbn?.ToString(),
                FirstName = row.firstname?.ToString() ?? string.Empty,
                LastName = row.lastname?.ToString() ?? string.Empty,
                Email = row.email?.ToString()
            });
        }

        // Отримати топ читачів за кількістю позичень
        public IEnumerable<dynamic> GetTopReadersByBorrowings(int limit = 10)
        {
            using var conn = CreateConnection();
            var sql = @"
                SELECT 
                    r.ReaderId, r.FirstName, r.LastName, r.Email,
                    COUNT(br.BorrowingId) as BorrowingsCount
                FROM Readers r
                LEFT JOIN Borrowings br ON r.ReaderId = br.ReaderId
                GROUP BY r.ReaderId, r.FirstName, r.LastName, r.Email
                ORDER BY BorrowingsCount DESC
                LIMIT @Limit";
            
            return conn.Query<dynamic>(sql, new { Limit = limit });
        }

        // Отримати топ книг за кількістю позичень
        public IEnumerable<dynamic> GetTopBooksByBorrowings(int limit = 10)
        {
            using var conn = CreateConnection();
            var sql = @"
                SELECT 
                    b.BookId, b.Title, b.Genre,
                    COUNT(br.BorrowingId) as BorrowingsCount
                FROM Books b
                LEFT JOIN Borrowings br ON b.BookId = br.BookId
                GROUP BY b.BookId, b.Title, b.Genre
                ORDER BY BorrowingsCount DESC
                LIMIT @Limit";
            
            return conn.Query<dynamic>(sql, new { Limit = limit });
        }

        // Отримати книги, які ніколи не позичалися
        public IEnumerable<Book> GetNeverBorrowedBooks()
        {
            using var conn = CreateConnection();
            var sql = @"
                SELECT b.* FROM Books b
                LEFT JOIN Borrowings br ON b.BookId = br.BookId
                WHERE br.BorrowingId IS NULL
                ORDER BY b.Title";
            
            var rows = conn.Query(sql);
            return MapDynamicToType<Book>(rows);
        }

        // Отримати читачів, які ніколи не позичали книги
        public IEnumerable<Reader> GetNeverBorrowedReaders()
        {
            using var conn = CreateConnection();
            var sql = @"
                SELECT r.* FROM Readers r
                LEFT JOIN Borrowings br ON r.ReaderId = br.ReaderId
                WHERE br.BorrowingId IS NULL
                ORDER BY r.LastName, r.FirstName";
            
            var rows = conn.Query(sql);
            return MapDynamicToType<Reader>(rows);
        }
    }
}