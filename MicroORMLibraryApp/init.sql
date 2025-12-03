-- init.sql
-- Видаляємо існуючі таблиці якщо вони є
DROP TABLE IF EXISTS Borrowings CASCADE;
DROP TABLE IF EXISTS BookAuthors CASCADE;
DROP TABLE IF EXISTS Readers CASCADE;
DROP TABLE IF EXISTS Books CASCADE;
DROP TABLE IF EXISTS Authors CASCADE;

-- Таблиця авторів
CREATE TABLE Authors (
                         AuthorId SERIAL PRIMARY KEY,
                         FirstName VARCHAR(50) NOT NULL,
                         LastName VARCHAR(50) NOT NULL,
                         BirthDate DATE,
                         Country VARCHAR(50),
                         CreatedDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Таблиця книг
CREATE TABLE Books (
                       BookId SERIAL PRIMARY KEY,
                       Title VARCHAR(255) NOT NULL,
                       ISBN VARCHAR(20) UNIQUE,
                       PublicationYear INT,
                       Genre VARCHAR(50),
                       Publisher VARCHAR(100),
                       PageCount INT,
                       Price DECIMAL(10,2),
                       CreatedDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Таблиця для зв'язку багато-до-багатьох (книги-автори) з каскадним видаленням
CREATE TABLE BookAuthors (
                             BookAuthorId SERIAL PRIMARY KEY,
                             BookId INT NOT NULL REFERENCES Books(BookId) ON DELETE CASCADE,
                             AuthorId INT NOT NULL REFERENCES Authors(AuthorId) ON DELETE CASCADE,
                             AuthorOrder INT NOT NULL,
                             UNIQUE(BookId, AuthorId)
);

-- Таблиця читачів
CREATE TABLE Readers (
                         ReaderId SERIAL PRIMARY KEY,
                         FirstName VARCHAR(50) NOT NULL,
                         LastName VARCHAR(50) NOT NULL,
                         Email VARCHAR(100) UNIQUE,
                         Phone VARCHAR(20),
                         RegistrationDate DATE DEFAULT CURRENT_DATE,
                         IsActive BOOLEAN DEFAULT true
);

-- Таблиця позичень з каскадним видаленням
CREATE TABLE Borrowings (
                            BorrowingId SERIAL PRIMARY KEY,
                            BookId INT NOT NULL REFERENCES Books(BookId) ON DELETE CASCADE,
                            ReaderId INT NOT NULL REFERENCES Readers(ReaderId) ON DELETE CASCADE,
                            BorrowDate DATE NOT NULL,
                            DueDate DATE NOT NULL,
                            ReturnDate DATE NULL,
                            Status VARCHAR(20) DEFAULT 'Borrowed'
);

-- Заповнення даними авторів
INSERT INTO Authors (FirstName, LastName, BirthDate, Country) VALUES
                                                                  ('Джордж', 'Оруелл', '1903-06-25', 'Великобританія'),
                                                                  ('Ольга', 'Кобилянська', '1863-11-27', 'Україна'),
                                                                  ('Агата', 'Крісті', '1890-09-15', 'Великобританія'),
                                                                  ('Юрій', 'Винничук', '1952-03-30', 'Україна'),
                                                                  ('Стівен', 'Кінг', '1947-09-21', 'США'),
                                                                  ('Оксана', 'Забужко', '1960-09-19', 'Україна'),
                                                                  ('Ліна', 'Костенко', '1930-03-19', 'Україна'),
                                                                  ('Михайло', 'Коцюбинський', '1864-09-17', 'Україна'),
                                                                  ('Іван', 'Франко', '1856-08-27', 'Україна'),
                                                                  ('Леся', 'Українка', '1871-02-25', 'Україна'),
                                                                  ('Марк', 'Твен', '1835-11-30', 'США'),
                                                                  ('Джоан', 'Роулінг', '1965-07-31', 'Великобританія'),
                                                                  ('Ден', 'Браун', '1964-06-22', 'США'),
                                                                  ('Гаррі', 'Гаррісон', '1925-03-12', 'США'),
                                                                  ('Айзек', 'Азімов', '1920-01-02', 'США');

-- Заповнення даними книг
INSERT INTO Books (Title, ISBN, PublicationYear, Genre, Publisher, PageCount, Price) VALUES
                                                                                         ('1984', '978-617-679-145-2', 2020, 'Антиутопія', 'Видавництво Старого Лева', 328, 250.00),
                                                                                         ('Царівна', '978-966-14-9012-8', 2019, 'Роман', 'Книжковий клуб', 256, 180.00),
                                                                                         ('Вбивство у "Східному експресі"', '978-0-00-711931-8', 1934, 'Детектив', 'Фоліо', 256, 220.00),
                                                                                         ('Малиновий роман', '978-966-2355-85-8', 2019, 'Роман', 'Видавництво Старого Лева', 432, 300.00),
                                                                                         ('Воно', '978-0-670-81302-5', 1986, 'Жахи', 'Фоліо', 1138, 450.00),
                                                                                         ('Записки українського самашедшого', '978-966-2355-42-1', 2010, 'Роман', 'Видавництво Старого Лева', 320, 280.00),
                                                                                         ('Маруся Чурай', '978-966-7047-87-8', 1979, 'Історичний роман', 'Радянський письменник', 280, 200.00),
                                                                                         ('Тіні забутих предків', '978-617-679-234-3', 2018, 'Повість', 'А-ба-ба-га-ла-ма-га', 240, 190.00),
                                                                                         ('Камінний хрест', '978-617-679-567-2', 2021, 'Повість', 'Видавництво Старого Лева', 180, 160.00),
                                                                                         ('Лісова пісня', '978-966-2355-23-0', 2017, 'Драма', 'А-ба-ба-га-ла-ма-га', 150, 170.00),
                                                                                         ('Гаррі Поттер і філософський камінь', '978-966-7048-12-7', 2020, 'Фентезі', 'А-ба-ба-га-ла-ма-га', 320, 350.00),
                                                                                         ('Код да Вінчі', '978-617-679-345-6', 2019, 'Трилер', 'Книжковий клуб', 480, 320.00),
                                                                                         ('Пригоди Тома Сойєра', '978-617-679-789-8', 2018, 'Пригоди', 'Видавництво Старого Лева', 280, 210.00),
                                                                                         ('Я (Романтика)', '978-966-2355-67-4', 2015, 'Роман', 'Фоліо', 380, 270.00),
                                                                                         ('Фауст', '978-617-679-456-9', 2020, 'Драма', 'Книжковий клуб', 420, 290.00),
                                                                                         ('Сталеві печери', '978-966-2355-89-6', 2019, 'Наукова фантастика', 'Фоліо', 320, 240.00),
                                                                                         ('Дюна', '978-617-679-678-5', 2021, 'Наукова фантастика', 'Видавництво Старого Лева', 560, 380.00),
                                                                                         ('Володар перснів', '978-966-2355-78-0', 2020, 'Фентезі', 'А-ба-ба-га-ла-ма-га', 480, 400.00),
                                                                                         ('Зло під сонцем', '978-617-679-234-9', 2019, 'Детектив', 'Книжковий клуб', 290, 230.00),
                                                                                         ('Українська антологія', '978-966-2355-99-5', 2022, 'Антологія', 'Видавництво Старого Лева', 600, 500.00);

-- Заповнення даними авторів книг (багато-до-багатьох)
INSERT INTO BookAuthors (BookId, AuthorId, AuthorOrder) VALUES
                                                            (1, 1, 1),   -- 1984 - Джордж Оруелл
                                                            (2, 2, 1),   -- Царівна - Ольга Кобилянська
                                                            (3, 3, 1),   -- Вбивство... - Агата Крісті
                                                            (4, 4, 1),   -- Малиновий роман - Юрій Винничук
                                                            (5, 5, 1),   -- Воно - Стівен Кінг
                                                            (6, 6, 1),   -- Записки... - Оксана Забужко
                                                            (7, 7, 1),   -- Маруся Чурай - Ліна Костенко
                                                            (8, 8, 1),   -- Тіні... - Михайло Коцюбинський
                                                            (9, 9, 1),   -- Камінний хрест - Іван Франко
                                                            (10, 10, 1), -- Лісова пісня - Леся Українка
                                                            (11, 12, 1), -- Гаррі Поттер - Джоан Роулінг
                                                            (12, 13, 1), -- Код да Вінчі - Ден Браун
                                                            (13, 11, 1), -- Том Сойєр - Марк Твен
                                                            (14, 2, 1),  -- Я (Романтика) - Ольга Кобилянська
                                                            (15, 9, 1),  -- Фауст - Іван Франко
                                                            (16, 14, 1), -- Сталеві печери - Гаррі Гаррісон
                                                            (17, 15, 1), -- Дюна - Айзек Азімов
                                                            (18, 15, 1), -- Володар перснів - Айзек Азімов
                                                            (19, 3, 1),  -- Зло під сонцем - Агата Крісті
                                                            (20, 2, 1),  -- Українська антологія - Ольга Кобилянська (основний)
                                                            (20, 6, 2),  -- Українська антологія - Оксана Забужко
                                                            (20, 7, 3),  -- Українська антологія - Ліна Костенко
                                                            (20, 8, 4);  -- Українська антологія - Михайло Коцюбинський

-- Заповнення даними читачів
INSERT INTO Readers (FirstName, LastName, Email, Phone) VALUES
                                                            ('Іван', 'Петренко', 'ivan@example.com', '+380501234567'),
                                                            ('Марія', 'Коваленко', 'maria@example.com', '+380671234568'),
                                                            ('Олександр', 'Шевченко', 'oleksandr@example.com', '+380631234569'),
                                                            ('Наталія', 'Бондар', 'natalia@example.com', '+380661234570'),
                                                            ('Дмитро', 'Мельник', 'dmitro@example.com', '+380731234571');

-- Заповнення даними позичень
INSERT INTO Borrowings (BookId, ReaderId, BorrowDate, DueDate, ReturnDate, Status) VALUES
                                                                                       (1, 1, '2024-01-15', '2024-02-15', NULL, 'Borrowed'),
                                                                                       (2, 2, '2024-01-20', '2024-02-20', NULL, 'Borrowed'),
                                                                                       (3, 3, '2024-01-10', '2024-02-10', NULL, 'Overdue'),
                                                                                       (5, 1, '2024-01-25', '2024-02-25', NULL, 'Borrowed'),
                                                                                       (7, 4, '2024-02-01', '2024-03-01', '2024-02-28', 'Returned'),
                                                                                       (11, 5, '2024-02-05', '2024-03-05', NULL, 'Borrowed'),
                                                                                       (15, 2, '2024-02-10', '2024-03-10', NULL, 'Borrowed');

-- Створення індексів
CREATE INDEX idx_books_title ON Books(Title);
CREATE INDEX idx_books_year ON Books(PublicationYear);
CREATE INDEX idx_books_genre ON Books(Genre);
CREATE INDEX idx_authors_lastname ON Authors(LastName);
CREATE INDEX idx_readers_email ON Readers(Email);
CREATE INDEX idx_borrowings_status ON Borrowings(Status);
CREATE INDEX idx_borrowings_due_date ON Borrowings(DueDate);

-- Функція для автоматичного видалення книг без авторів
CREATE OR REPLACE FUNCTION delete_books_without_authors()
RETURNS TRIGGER AS $$
BEGIN
DELETE FROM Books
WHERE BookId NOT IN (SELECT DISTINCT BookId FROM BookAuthors);
RETURN NULL;
END;
$$ LANGUAGE plpgsql;

-- Тригер для видалення книг без авторів після видалення зв'язків
CREATE TRIGGER trigger_delete_orphaned_books
    AFTER DELETE ON BookAuthors
    FOR EACH STATEMENT
    EXECUTE FUNCTION delete_books_without_authors();