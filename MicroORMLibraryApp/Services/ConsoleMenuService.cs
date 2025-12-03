// MicroORMLibraryApp/Services/ConsoleMenuService.cs
using MicroORMLibraryApp.Models;
using MicroORMLibraryApp.Repository;
using Spectre.Console;
using System.Globalization;
using Npgsql;
using Dapper;

namespace MicroORMLibraryApp.Services
{
    public class ConsoleMenuService
    {
        private readonly LibraryRepository _repository;
        private readonly string _connectionString;

        public ConsoleMenuService(LibraryRepository repository, string connectionString)
        {
            _repository = repository;
            _connectionString = connectionString;
        }

        public void Run()
        {
            while (true)
            {
                Console.Clear();
                AnsiConsole.Write(
                    new FigletText("MicroORM Library")
                        .Centered()
                        .Color(Color.Blue));

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Оберіть розділ:")
                        .PageSize(10)
                        .AddChoices(new[]
                        {
                            "📚 Книги (CRUD)",
                            "👥 Автори (CRUD)",
                            "👤 Читачі (CRUD)",
                            "📖 Позичення (CRUD)",
                            "🔍 Запити з об'єднанням таблиць",
                            "🔍 Запити з фільтрацією",
                            "🔍 Запити з агрегатними функціями",
                            "🔄 Скинути базу даних",
                            "❌ Вихід"
                        }));

                switch (choice)
                {
                    case "📚 Книги (CRUD)":
                        ShowBooksMenu();
                        break;
                    case "👥 Автори (CRUD)":
                        ShowAuthorsMenu();
                        break;
                    case "👤 Читачі (CRUD)":
                        ShowReadersMenu();
                        break;
                    case "📖 Позичення (CRUD)":
                        ShowBorrowingsMenu();
                        break;
                    case "🔍 Запити з об'єднанням таблиць":
                        ShowJoinQueriesMenu();
                        break;
                    case "🔍 Запити з фільтрацією":
                        ShowFilterQueriesMenu();
                        break;
                    case "🔍 Запити з агрегатними функціями":
                        ShowAggregateQueriesMenu();
                        break;
                    case "🔄 Скинути базу даних":
                        ResetDatabase();
                        break;
                    case "❌ Вихід":
                        return;
                }
            }
        }

        private void ShowBooksMenu()
        {
            while (true)
            {
                Console.Clear();
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("📚 CRUD операції для книг:")
                        .PageSize(10)
                        .AddChoices(new[]
                        {
                            "📖 Список всіх книг",
                            "➕ Додати книгу",
                            "✏️ Редагувати книгу",
                            "🗑️ Видалити книгу",
                            "🔍 Пошук книги",
                            "⬅️ Назад"
                        }));

                switch (choice)
                {
                    case "📖 Список всіх книг":
                        DisplayBooks();
                        break;
                    case "➕ Додати книгу":
                        AddBook();
                        break;
                    case "✏️ Редагувати книгу":
                        EditBook();
                        break;
                    case "🗑️ Видалити книгу":
                        DeleteBook();
                        break;
                    case "🔍 Пошук книги":
                        SearchBooks();
                        break;
                    case "⬅️ Назад":
                        return;
                }
            }
        }

        private void ShowAuthorsMenu()
        {
            while (true)
            {
                Console.Clear();
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("👥 CRUD операції для авторів:")
                        .PageSize(10)
                        .AddChoices(new[]
                        {
                            "📝 Список авторів",
                            "➕ Додати автора",
                            "✏️ Редагувати автора",
                            "🗑️ Видалити автора",
                            "⬅️ Назад"
                        }));

                switch (choice)
                {
                    case "📝 Список авторів":
                        DisplayAuthors();
                        break;
                    case "➕ Додати автора":
                        AddAuthor();
                        break;
                    case "✏️ Редагувати автора":
                        EditAuthor();
                        break;
                    case "🗑️ Видалити автора":
                        DeleteAuthor();
                        break;
                    case "⬅️ Назад":
                        return;
                }
            }
        }

        private void ShowReadersMenu()
        {
            while (true)
            {
                Console.Clear();
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("👤 CRUD операції для читачів:")
                        .PageSize(10)
                        .AddChoices(new[]
                        {
                            "📋 Список читачів",
                            "➕ Додати читача",
                            "✏️ Редагувати читача",
                            "🗑️ Видалити читача",
                            "⬅️ Назад"
                        }));

                switch (choice)
                {
                    case "📋 Список читачів":
                        DisplayReaders();
                        break;
                    case "➕ Додати читача":
                        AddReader();
                        break;
                    case "✏️ Редагувати читача":
                        EditReader();
                        break;
                    case "🗑️ Видалити читача":
                        DeleteReader();
                        break;
                    case "⬅️ Назад":
                        return;
                }
            }
        }

        private void ShowBorrowingsMenu()
        {
            while (true)
            {
                Console.Clear();
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("📖 CRUD операції для позичень:")
                        .PageSize(10)
                        .AddChoices(new[]
                        {
                            "📚 Список позичень",
                            "➕ Додати позичення",
                            "✏️ Редагувати позичення",
                            "🗑️ Видалити позичення",
                            "⬅️ Назад"
                        }));

                switch (choice)
                {
                    case "📚 Список позичень":
                        DisplayBorrowings();
                        break;
                    case "➕ Додати позичення":
                        AddBorrowing();
                        break;
                    case "✏️ Редагувати позичення":
                        EditBorrowing();
                        break;
                    case "🗑️ Видалити позичення":
                        DeleteBorrowing();
                        break;
                    case "⬅️ Назад":
                        return;
                }
            }
        }

        private void ShowJoinQueriesMenu()
        {
            while (true)
            {
                Console.Clear();
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("🔍 Запити з об'єднанням таблиць:")
                        .PageSize(10)
                        .AddChoices(new[]
                        {
                            "📚 Книги з авторами (JOIN)",
                            "👥 Автори з кількістю книг (LEFT JOIN + GROUP BY)",
                            "📖 Поточні позичення (JOIN кількох таблиць)",
                            "⬅️ Назад"
                        }));

                switch (choice)
                {
                    case "📚 Книги з авторами (JOIN)":
                        DisplayBooksWithAuthors();
                        break;
                    case "👥 Автори з кількістю книг (LEFT JOIN + GROUP BY)":
                        DisplayAuthorsWithBookCount();
                        break;
                    case "📖 Поточні позичення (JOIN кількох таблиць)":
                        DisplayCurrentBorrowings();
                        break;
                    case "⬅️ Назад":
                        return;
                }
            }
        }

        private void ShowFilterQueriesMenu()
        {
            while (true)
            {
                Console.Clear();
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("🔍 Запити з фільтрацією:")
                        .PageSize(10)
                        .AddChoices(new[]
                        {
                            "📚 Книги за жанром (WHERE)",
                            "🔍 Пошук книги (LIKE)",
                            "📖 Книги автора",
                            "⏰ Прострочені позичення (WHERE з датою)",
                            "⬅️ Назад"
                        }));

                switch (choice)
                {
                    case "📚 Книги за жанром (WHERE)":
                        DisplayBooksByGenre();
                        break;
                    case "🔍 Пошук книги (LIKE)":
                        SearchBooks();
                        break;
                    case "📖 Книги автора":
                        DisplayBooksByAuthor();
                        break;
                    case "⏰ Прострочені позичення (WHERE з датою)":
                        DisplayOverdueBorrowings();
                        break;
                    case "⬅️ Назад":
                        return;
                }
            }
        }

        private void ShowAggregateQueriesMenu()
        {
            while (true)
            {
                Console.Clear();
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("🔍 Запити з агрегатними функціями:")
                        .PageSize(10)
                        .AddChoices(new[]
                        {
                            "📊 Статистика книг (COUNT, AVG, MIN, MAX, SUM)",
                            "👥 Автори з кількістю книг (COUNT + GROUP BY)",
                            "📚 Книги з кількома авторами (HAVING)",
                            "👤 Читачі з простроченими книгами (COUNT + GROUP BY)",
                            "⬅️ Назад"
                        }));

                switch (choice)
                {
                    case "📊 Статистика книг (COUNT, AVG, MIN, MAX, SUM)":
                        DisplayBooksStatistics();
                        break;
                    case "👥 Автори з кількістю книг (COUNT + GROUP BY)":
                        DisplayAuthorsWithBookCount();
                        break;
                    case "📚 Книги з кількома авторами (HAVING)":
                        DisplayBooksWithMultipleAuthors();
                        break;
                    case "👤 Читачі з простроченими книгами (COUNT + GROUP BY)":
                        DisplayReadersWithOverdueBooks();
                        break;
                    case "⬅️ Назад":
                        return;
                }
            }
        }

        #region Display Methods
        private void DisplayBooks()
        {
            var books = _repository.GetAllBooks();
            
            var table = new Table();
            table.AddColumn("ID");
            table.AddColumn("Назва");
            table.AddColumn("Жанр");
            table.AddColumn("Рік");
            table.AddColumn("Ціна");
            table.AddColumn("ISBN");

            foreach (var book in books)
            {
                table.AddRow(
                    book.BookId.ToString(),
                    book.Title,
                    book.Genre ?? "Н/Д",
                    book.PublicationYear?.ToString() ?? "Н/Д",
                    book.Price?.ToString("C") ?? "Н/Д",
                    book.ISBN ?? "Н/Д"
                );
            }

            AnsiConsole.Write(table);
            WaitForContinue();
        }

        private void DisplayAuthors()
        {
            var authors = _repository.GetAllAuthors();
            
            var table = new Table();
            table.AddColumn("ID");
            table.AddColumn("Ім'я");
            table.AddColumn("Прізвище");
            table.AddColumn("Країна");
            table.AddColumn("Дата народження");

            foreach (var author in authors)
            {
                table.AddRow(
                    author.AuthorId.ToString(),
                    author.FirstName,
                    author.LastName,
                    author.Country ?? "Н/Д",
                    author.BirthDate?.ToString("yyyy-MM-dd") ?? "Н/Д"
                );
            }

            AnsiConsole.Write(table);
            WaitForContinue();
        }

        private void DisplayBooksWithAuthors()
        {
            var books = _repository.GetBooksWithAuthors();
    
            var table = new Table();
            table.AddColumn("Назва книги");
            table.AddColumn("Жанр");
            table.AddColumn("Автори");
            table.AddColumn("Рік");
            table.AddColumn("Ціна");

            foreach (var book in books)
            {
                table.AddRow(
                    book.Title,
                    book.Genre ?? "Н/Д",
                    book.FirstName ?? "Без автора", // Now contains all authors
                    book.PublicationYear?.ToString() ?? "Н/Д",
                    book.Price?.ToString("C") ?? "Н/Д"
                );
            }

            AnsiConsole.Write(table);
            AnsiConsole.WriteLine("\n[green]Це запит з об'єднанням таблиць (JOIN)[/]");
            WaitForContinue();
        }

        private void DisplayBooksStatistics()
        {
            var stats = _repository.GetBooksStatistics();
            
            var panel = new Panel($"""
                📚 Загальна кількість книг: {stats.TotalBooks}
                💰 Середня ціна: {stats.AveragePrice:F2} грн
                📅 Рік найстарішої книги: {stats.OldestYear}
                📅 Рік найновішої книги: {stats.NewestYear}
                📄 Загальна кількість сторінок: {stats.TotalPages}
                🏷️ Кількість жанрів: {stats.GenreCount}
                """)
                .Header("Статистика книг (агрегатні функції)")
                .BorderColor(Color.Green);

            AnsiConsole.Write(panel);
            AnsiConsole.WriteLine("\n[green]Це запит з агрегатними функціями: COUNT, AVG, MIN, MAX, SUM[/]");
            WaitForContinue();
        }

        private void DisplayBooksByGenre()
        {
            var genre = AnsiConsole.Ask<string>("Введіть жанр для пошуку:");
            var books = _repository.GetBooksByGenre(genre);

            if (!books.Any())
            {
                AnsiConsole.MarkupLine($"[yellow]Книги жанру '{genre}' не знайдені[/]");
            }
            else
            {
                var table = new Table();
                table.AddColumn("ID");
                table.AddColumn("Назва");
                table.AddColumn("Рік");
                table.AddColumn("Ціна");
                table.AddColumn("Видавництво");

                foreach (var book in books)
                {
                    table.AddRow(
                        book.BookId.ToString(),
                        book.Title,
                        book.PublicationYear?.ToString() ?? "Н/Д",
                        book.Price?.ToString("C") ?? "Н/Д",
                        book.Publisher ?? "Н/Д"
                    );
                }

                AnsiConsole.Write(table);
                AnsiConsole.WriteLine("\n[green]Це запит з фільтрацією (WHERE)[/]");
            }

            WaitForContinue();
        }

        private void DisplayAuthorsWithBookCount()
        {
            var authors = _repository.GetAuthorsWithBookCount();
            
            var table = new Table();
            table.AddColumn("Автор");
            table.AddColumn("Країна");
            table.AddColumn("Кількість книг");

            foreach (var author in authors)
            {
                table.AddRow(
                    $"{author.FirstName} {author.LastName}",
                    author.Country ?? "Н/Д",
                    author.BookCount.ToString()
                );
            }

            AnsiConsole.Write(table);
            AnsiConsole.WriteLine("\n[green]Це запит з об'єднанням (LEFT JOIN) та агрегатною функцією (COUNT)[/]");
            WaitForContinue();
        }

        private void DisplayBooksWithMultipleAuthors()
        {
            var books = _repository.GetBooksWithMultipleAuthors();
            
            var table = new Table();
            table.AddColumn("Назва книги");
            table.AddColumn("Жанр");
            table.AddColumn("Кількість авторів");

            foreach (var book in books)
            {
                table.AddRow(
                    book.Title,
                    book.Genre ?? "Н/Д",
                    book.AuthorCount.ToString()
                );
            }

            AnsiConsole.Write(table);
            AnsiConsole.WriteLine("\n[green]Це запит з агрегатною функцією (COUNT) та фільтрацією групування (HAVING)[/]");
            WaitForContinue();
        }

        private void DisplayCurrentBorrowings()
        {
            var borrowings = _repository.GetCurrentBorrowings();
            
            var table = new Table();
            table.AddColumn("Читач");
            table.AddColumn("Книга");
            table.AddColumn("Дата позичення");
            table.AddColumn("Термін повернення");
            table.AddColumn("Статус");

            foreach (var borrowing in borrowings)
            {
                table.AddRow(
                    $"{borrowing.FirstName} {borrowing.LastName}",
                    borrowing.Title,
                    borrowing.BorrowDate.ToString("yyyy-MM-dd"),
                    borrowing.DueDate.ToString("yyyy-MM-dd"),
                    borrowing.Status
                );
            }

            AnsiConsole.Write(table);
            AnsiConsole.WriteLine("\n[green]Це запит з об'єднанням кількох таблиць (JOIN)[/]");
            WaitForContinue();
        }

        private void DisplayReadersWithOverdueBooks()
        {
            var readers = _repository.GetReadersWithOverdueBooks();
            
            if (!readers.Any())
            {
                AnsiConsole.MarkupLine("[green]Читачів з простроченими книгами немає![/]");
            }
            else
            {
                var table = new Table();
                table.AddColumn("Читач");
                table.AddColumn("Email");
                table.AddColumn("Кількість прострочених");
                table.AddColumn("Найстаріше прострочення");

                foreach (var reader in readers)
                {
                    table.AddRow(
                        $"{reader.FirstName} {reader.LastName}",
                        reader.Email,
                        reader.OverdueCount.ToString(),
                        reader.OldestOverdue.ToString("yyyy-MM-dd")
                    );
                }

                AnsiConsole.Write(table);
            }

            AnsiConsole.WriteLine("\n[green]Це запит з фільтрацією (WHERE) та агрегатною функцією (COUNT)[/]");
            WaitForContinue();
        }

        private void DisplayOverdueBorrowings()
        {
            // Використовуємо той же метод, але фільтруємо тільки прострочені
            var borrowings = _repository.GetCurrentBorrowings();
            var overdue = borrowings.Where(b => b.Status == "Overdue").ToList();
            
            if (!overdue.Any())
            {
                AnsiConsole.MarkupLine("[green]Прострочених позичень немає![/]");
            }
            else
            {
                var table = new Table();
                table.AddColumn("Читач");
                table.AddColumn("Книга");
                table.AddColumn("Дата позичення");
                table.AddColumn("Термін повернення");
                table.AddColumn("Днів прострочення");

                foreach (var borrowing in overdue)
                {
                    var daysOverdue = (DateTime.Now - borrowing.DueDate).Days;
                    table.AddRow(
                        $"{borrowing.FirstName} {borrowing.LastName}",
                        borrowing.Title,
                        borrowing.BorrowDate.ToString("yyyy-MM-dd"),
                        borrowing.DueDate.ToString("yyyy-MM-dd"),
                        $"{daysOverdue} днів"
                    );
                }

                AnsiConsole.Write(table);
            }

            AnsiConsole.WriteLine("\n[green]Це запит з фільтрацією за датою (WHERE)[/]");
            WaitForContinue();
        }
        #endregion

        #region CRUD Operations
        private void AddBook()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold blue]Додавання нової книги[/]");

            var book = new Book
            {
                Title = AnsiConsole.Ask<string>("Назва книги:"),
                ISBN = AnsiConsole.Ask<string>("ISBN:"),
                Genre = AnsiConsole.Ask<string>("Жанр:"),
                PublicationYear = AnsiConsole.Ask<int?>("Рік видання (Enter для пропуску):"),
                Publisher = AnsiConsole.Ask<string>("Видавництво:"),
                PageCount = AnsiConsole.Ask<int?>("Кількість сторінок (Enter для пропуску):"),
                Price = AnsiConsole.Ask<decimal?>("Ціна (Enter для пропуску):")
            };

            try
            {
                var bookId = _repository.CreateBook(book);
                AnsiConsole.MarkupLine($"[green]Книга успішно додана з ID: {bookId}[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Помилка: {ex.Message}[/]");
            }

            WaitForContinue();
        }

        private void AddAuthor()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold blue]Додавання нового автора[/]");

            var author = new Author
            {
                FirstName = AnsiConsole.Ask<string>("Ім'я:"),
                LastName = AnsiConsole.Ask<string>("Прізвище:"),
                Country = AnsiConsole.Ask<string>("Країна:"),
                BirthDate = ParseDateOnlyFromUser("Дата народження (yyyy-MM-dd, Enter для пропуску):")
            };

            try
            {
                var authorId = _repository.CreateAuthor(author);
                AnsiConsole.MarkupLine($"[green]Автор успішно доданий з ID: {authorId}[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Помилка: {ex.Message}[/]");
            }

            WaitForContinue();
        }

        private void AddReader()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold blue]Додавання нового читача[/]");

            var reader = new Reader
            {
                FirstName = AnsiConsole.Ask<string>("Ім'я:"),
                LastName = AnsiConsole.Ask<string>("Прізвище:"),
                Email = AnsiConsole.Ask<string>("Email:"),
                Phone = AnsiConsole.Ask<string>("Телефон:"),
                RegistrationDate = DateOnly.FromDateTime(DateTime.Now),
                IsActive = true
            };

            try
            {
                var readerId = _repository.CreateReader(reader);
                AnsiConsole.MarkupLine($"[green]Читач успішно доданий з ID: {readerId}[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Помилка: {ex.Message}[/]");
            }

            WaitForContinue();
        }

        private void AddBorrowing()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold blue]Додавання нового позичення[/]");

            var borrowing = new Borrowing
            {
                BookId = AnsiConsole.Ask<int>("ID книги:"),
                ReaderId = AnsiConsole.Ask<int>("ID читача:"),
                BorrowDate = DateOnly.FromDateTime(DateTime.Now),
                DueDate = ParseDateOnlyFromUser("Термін повернення (yyyy-MM-dd):") ?? DateOnly.FromDateTime(DateTime.Now.AddDays(30)),
                Status = "Borrowed"
            };

            try
            {
                var borrowingId = _repository.CreateBorrowing(borrowing);
                AnsiConsole.MarkupLine($"[green]Позичення успішно додане з ID: {borrowingId}[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Помилка: {ex.Message}[/]");
            }

            WaitForContinue();
        }

        private void EditBook()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold yellow]Редагування книги[/]");

            var bookId = AnsiConsole.Ask<int>("ID книги для редагування:");
            var book = _repository.GetBookById(bookId);

            if (book == null)
            {
                AnsiConsole.MarkupLine("[red]Книга не знайдена![/]");
                WaitForContinue();
                return;
            }

            AnsiConsole.MarkupLine($"Редагуємо книгу: [yellow]{book.Title}[/]");

            var newTitle = AnsiConsole.Ask<string>("Назва книги:", book.Title);
            var newISBN = AnsiConsole.Ask<string>("ISBN:", book.ISBN ?? "");
            var newGenre = AnsiConsole.Ask<string>("Жанр:", book.Genre ?? "");
            var newPrice = AnsiConsole.Ask<decimal?>("Ціна (Enter для пропуску):", book.Price);

            book.Title = newTitle;
            book.ISBN = newISBN;
            book.Genre = newGenre;
            book.Price = newPrice;

            try
            {
                var result = _repository.UpdateBook(book);
                if (result)
                    AnsiConsole.MarkupLine("[green]Книга успішно оновлена[/]");
                else
                    AnsiConsole.MarkupLine("[yellow]Не вдалося оновити книгу[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Помилка: {ex.Message}[/]");
            }

            WaitForContinue();
        }

        private void EditAuthor()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold yellow]Редагування автора[/]");

            var authorId = AnsiConsole.Ask<int>("ID автора для редагування:");
            var author = _repository.GetAuthorById(authorId);

            if (author == null)
            {
                AnsiConsole.MarkupLine("[red]Автор не знайдений![/]");
                WaitForContinue();
                return;
            }

            AnsiConsole.MarkupLine($"Редагуємо автора: [yellow]{author.FirstName} {author.LastName}[/]");

            var newFirstName = AnsiConsole.Ask<string>("Ім'я:", author.FirstName);
            var newLastName = AnsiConsole.Ask<string>("Прізвище:", author.LastName);
            var newCountry = AnsiConsole.Ask<string>("Країна:", author.Country ?? "");
            
            // Конвертуємо DateOnly? в DateTime? для AnsiConsole, потім назад
            var birthDateInput = AnsiConsole.Ask<string>("Дата народження (yyyy-MM-dd, Enter для пропуску):", 
                author.BirthDate?.ToString("yyyy-MM-dd") ?? "");
            author.BirthDate = string.IsNullOrEmpty(birthDateInput) ? 
                null : 
                DateOnly.ParseExact(birthDateInput, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            author.FirstName = newFirstName;
            author.LastName = newLastName;
            author.Country = newCountry;

            try
            {
                var result = _repository.UpdateAuthor(author);
                if (result)
                    AnsiConsole.MarkupLine("[green]Автор успішно оновлений[/]");
                else
                    AnsiConsole.MarkupLine("[yellow]Не вдалося оновити автора[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Помилка: {ex.Message}[/]");
            }

            WaitForContinue();
        }

        private void EditReader()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold yellow]Редагування читача[/]");

            var readerId = AnsiConsole.Ask<int>("ID читача для редагування:");
            var reader = _repository.GetReaderById(readerId);

            if (reader == null)
            {
                AnsiConsole.MarkupLine("[red]Читач не знайдений![/]");
                WaitForContinue();
                return;
            }

            AnsiConsole.MarkupLine($"Редагуємо читача: [yellow]{reader.FirstName} {reader.LastName}[/]");

            var newFirstName = AnsiConsole.Ask<string>("Ім'я:", reader.FirstName);
            var newLastName = AnsiConsole.Ask<string>("Прізвище:", reader.LastName);
            var newEmail = AnsiConsole.Ask<string>("Email:", reader.Email ?? "");
            var isActive = AnsiConsole.Confirm("Активний?", reader.IsActive);

            reader.FirstName = newFirstName;
            reader.LastName = newLastName;
            reader.Email = newEmail;
            reader.IsActive = isActive;

            try
            {
                var result = _repository.UpdateReader(reader);
                if (result)
                    AnsiConsole.MarkupLine("[green]Читач успішно оновлений[/]");
                else
                    AnsiConsole.MarkupLine("[yellow]Не вдалося оновити читача[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Помилка: {ex.Message}[/]");
            }

            WaitForContinue();
        }

        private void EditBorrowing()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold yellow]Редагування позичення[/]");

            var borrowingId = AnsiConsole.Ask<int>("ID позичення для редагування:");
            var borrowing = _repository.GetBorrowingById(borrowingId);

            if (borrowing == null)
            {
                AnsiConsole.MarkupLine("[red]Позичення не знайдено![/]");
                WaitForContinue();
                return;
            }

            // Конвертуємо DateOnly? в string для вводу, потім назад
            var returnDateInput = AnsiConsole.Ask<string>("Дата повернення (yyyy-MM-dd, Enter для NULL):", 
                borrowing.ReturnDate?.ToString("yyyy-MM-dd") ?? "");
            borrowing.ReturnDate = string.IsNullOrEmpty(returnDateInput) ? 
                null : 
                DateOnly.ParseExact(returnDateInput, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            borrowing.Status = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Статус:")
                    .AddChoices("Borrowed", "Returned", "Overdue"));

            try
            {
                var result = _repository.UpdateBorrowing(borrowing);
                if (result)
                    AnsiConsole.MarkupLine("[green]Позичення успішно оновлено[/]");
                else
                    AnsiConsole.MarkupLine("[yellow]Не вдалося оновити позичення[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Помилка: {ex.Message}[/]");
            }

            WaitForContinue();
        }

        private void DeleteBook()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold red]Видалення книги[/]");

            var bookId = AnsiConsole.Ask<int>("ID книги для видалення:");
            var book = _repository.GetBookById(bookId);

            if (book == null)
            {
                AnsiConsole.MarkupLine("[red]Книга не знайдена![/]");
                WaitForContinue();
                return;
            }

            AnsiConsole.MarkupLine($"Ви дійсно хочете видалити книгу: [yellow]{book.Title}[/]?");
            if (AnsiConsole.Confirm("Підтвердити видалення?"))
            {
                try
                {
                    var result = _repository.DeleteBook(bookId);
                    if (result)
                        AnsiConsole.MarkupLine("[green]Книга успішно видалена[/]");
                    else
                        AnsiConsole.MarkupLine("[yellow]Книга не була видалена[/]");
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]Помилка: {ex.Message}[/]");
                }
            }

            WaitForContinue();
        }

        private void DeleteAuthor()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold red]Видалення автора[/]");

            var authorId = AnsiConsole.Ask<int>("ID автора для видалення:");
            var author = _repository.GetAuthorById(authorId);

            if (author == null)
            {
                AnsiConsole.MarkupLine("[red]Автор не знайдений![/]");
                WaitForContinue();
                return;
            }

            AnsiConsole.MarkupLine($"Ви дійсно хочете видалити автора: [yellow]{author.FirstName} {author.LastName}[/]?");
            AnsiConsole.MarkupLine("[yellow]Увага: Це може призвести до каскадного видалення книг без авторів![/]");
            
            if (AnsiConsole.Confirm("Підтвердити видалення?"))
            {
                try
                {
                    var result = _repository.DeleteAuthor(authorId);
                    if (result)
                        AnsiConsole.MarkupLine("[green]Автор успішно видалений[/]");
                    else
                        AnsiConsole.MarkupLine("[yellow]Автор не був видалений[/]");
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]Помилка: {ex.Message}[/]");
                }
            }

            WaitForContinue();
        }

        private void DeleteReader()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold red]Видалення читача[/]");

            var readerId = AnsiConsole.Ask<int>("ID читача для видалення:");
            var reader = _repository.GetReaderById(readerId);

            if (reader == null)
            {
                AnsiConsole.MarkupLine("[red]Читач не знайдений![/]");
                WaitForContinue();
                return;
            }

            AnsiConsole.MarkupLine($"Ви дійсно хочете видалити читача: [yellow]{reader.FirstName} {reader.LastName}[/]?");
            AnsiConsole.MarkupLine("[yellow]Увага: Це також видалить всі його позичення![/]");
            
            if (AnsiConsole.Confirm("Підтвердити видалення?"))
            {
                try
                {
                    var result = _repository.DeleteReader(readerId);
                    if (result)
                        AnsiConsole.MarkupLine("[green]Читач успішно видалений[/]");
                    else
                        AnsiConsole.MarkupLine("[yellow]Читач не був видалений[/]");
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]Помилка: {ex.Message}[/]");
                }
            }

            WaitForContinue();
        }

        private void DeleteBorrowing()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold red]Видалення позичення[/]");

            var borrowingId = AnsiConsole.Ask<int>("ID позичення для видалення:");
            var borrowing = _repository.GetBorrowingById(borrowingId);

            if (borrowing == null)
            {
                AnsiConsole.MarkupLine("[red]Позичення не знайдено![/]");
                WaitForContinue();
                return;
            }

            AnsiConsole.MarkupLine($"Ви дійсно хочете видалити позичення ID: [yellow]{borrowingId}[/]?");
            
            if (AnsiConsole.Confirm("Підтвердити видалення?"))
            {
                try
                {
                    var result = _repository.DeleteBorrowing(borrowingId);
                    if (result)
                        AnsiConsole.MarkupLine("[green]Позичення успішно видалено[/]");
                    else
                        AnsiConsole.MarkupLine("[yellow]Позичення не було видалено[/]");
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]Помилка: {ex.Message}[/]");
                }
            }

            WaitForContinue();
        }
        #endregion

        #region Helper Methods
        private DateOnly? ParseDateOnlyFromUser(string prompt)
        {
            var input = AnsiConsole.Ask<string>(prompt);
            if (string.IsNullOrEmpty(input))
                return null;

            try
            {
                return DateOnly.ParseExact(input, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            catch
            {
                try
                {
                    return DateOnly.Parse(input, CultureInfo.InvariantCulture);
                }
                catch
                {
                    AnsiConsole.MarkupLine("[red]Неправильний формат дати! Використовуйте yyyy-MM-dd[/]");
                    return null;
                }
            }
        }

        private void SearchBooks()
        {
            Console.Clear();
            var searchTerm = AnsiConsole.Ask<string>("Пошуковий запит:");
            var books = _repository.SearchBooks(searchTerm);

            if (!books.Any())
            {
                AnsiConsole.MarkupLine("[yellow]Книги не знайдені[/]");
            }
            else
            {
                var table = new Table();
                table.AddColumn("ID");
                table.AddColumn("Назва");
                table.AddColumn("Жанр");
                table.AddColumn("Рік");
                table.AddColumn("ISBN");

                foreach (var book in books)
                {
                    table.AddRow(
                        book.BookId.ToString(),
                        book.Title,
                        book.Genre ?? "Н/Д",
                        book.PublicationYear?.ToString() ?? "Н/Д",
                        book.ISBN ?? "Н/Д"
                    );
                }

                AnsiConsole.Write(table);
            }

            AnsiConsole.WriteLine("\n[green]Це запит з фільтрацією (LIKE)[/]");
            WaitForContinue();
        }

        private void DisplayBooksByAuthor()
        {
            Console.Clear();
            var authorId = AnsiConsole.Ask<int>("ID автора:");
            var books = _repository.GetBooksByAuthor(authorId);

            if (!books.Any())
            {
                AnsiConsole.MarkupLine("[yellow]Книги цього автора не знайдені[/]");
            }
            else
            {
                var author = _repository.GetAuthorById(authorId);
                AnsiConsole.MarkupLine($"[bold]Книги автора: {author?.FirstName} {author?.LastName}[/]\n");

                var table = new Table();
                table.AddColumn("ID");
                table.AddColumn("Назва");
                table.AddColumn("Жанр");
                table.AddColumn("Рік");

                foreach (var book in books)
                {
                    table.AddRow(
                        book.BookId.ToString(),
                        book.Title,
                        book.Genre ?? "Н/Д",
                        book.PublicationYear?.ToString() ?? "Н/Д"
                    );
                }

                AnsiConsole.Write(table);
            }

            WaitForContinue();
        }

        private void DisplayReaders()
        {
            var readers = _repository.GetAllReaders();
            
            var table = new Table();
            table.AddColumn("ID");
            table.AddColumn("Ім'я");
            table.AddColumn("Прізвище");
            table.AddColumn("Email");
            table.AddColumn("Телефон");
            table.AddColumn("Активний");

            foreach (var reader in readers)
            {
                table.AddRow(
                    reader.ReaderId.ToString(),
                    reader.FirstName,
                    reader.LastName,
                    reader.Email ?? "Н/Д",
                    reader.Phone ?? "Н/Д",
                    reader.IsActive ? "✅" : "❌"
                );
            }

            AnsiConsole.Write(table);
            WaitForContinue();
        }

        private void DisplayBorrowings()
        {
            var borrowings = _repository.GetAllBorrowings();
            
            var table = new Table();
            table.AddColumn("ID");
            table.AddColumn("ID книги");
            table.AddColumn("ID читача");
            table.AddColumn("Дата позичення");
            table.AddColumn("Термін повернення");
            table.AddColumn("Дата повернення");
            table.AddColumn("Статус");

            foreach (var borrowing in borrowings)
            {
                table.AddRow(
                    borrowing.BorrowingId.ToString(),
                    borrowing.BookId.ToString(),
                    borrowing.ReaderId.ToString(),
                    borrowing.BorrowDate.ToString("yyyy-MM-dd"),
                    borrowing.DueDate.ToString("yyyy-MM-dd"),
                    borrowing.ReturnDate?.ToString("yyyy-MM-dd") ?? "Не повернено",
                    borrowing.Status
                );
            }

            AnsiConsole.Write(table);
            WaitForContinue();
        }

        private string FindInitSqlFile()
        {
            // Шукаємо файл init.sql в різних місцях
            var possiblePaths = new[]
            {
                "init.sql",
                Path.Combine(Directory.GetCurrentDirectory(), "init.sql"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "init.sql"),
                Path.Combine(Directory.GetCurrentDirectory(), "..", "init.sql"),
                Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "init.sql"),
                Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "init.sql"),
                Path.Combine(Environment.CurrentDirectory, "init.sql")
            };
    
            foreach (var path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    return path;
                }
            }
    
            return null;
        }
        
        private void ResetDatabase()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold yellow]Скидання бази даних[/]");
    
            try
            {
                // Знаходимо файл init.sql
                var initSqlPath = FindInitSqlFile();
                if (string.IsNullOrEmpty(initSqlPath))
                {
                    AnsiConsole.MarkupLine("[red]Файл init.sql не знайдено![/]");
                    WaitForContinue();
                    return;
                }
        
                AnsiConsole.MarkupLine($"Використовую файл: {initSqlPath}");
        
                // Виконуємо SQL
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();
        
                var initSql = File.ReadAllText(initSqlPath);
                conn.Execute(initSql);
        
                AnsiConsole.MarkupLine("[green]✅ Базу даних успішно скинуто![/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Помилка: {ex.Message}[/]");
            }
    
            WaitForContinue();
        }

        private void WaitForContinue()
        {
            AnsiConsole.WriteLine("\nНатисніть будь-яку клавішу для продовження...");
            Console.ReadKey();
        }
        #endregion
    }
}