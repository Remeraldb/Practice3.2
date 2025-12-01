// MicroORMLibraryApp/Program.cs
using MicroORMLibraryApp.Repository;
using MicroORMLibraryApp.Services;
using Microsoft.Extensions.Configuration;

class Program
{
    static void Main()
    {
        try
        {
            // Налаштування конфігурації
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Створення репозиторію
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var repository = new LibraryRepository(connectionString);
            
            // Створення сервісу меню
            var menuService = new ConsoleMenuService(repository);
            
            // Запуск програми
            menuService.Run();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Критична помилка: {ex.Message}");
            Console.ResetColor();
            Console.WriteLine("\nНатисніть будь-яку клавішу для виходу...");
            Console.ReadKey();
        }
    }
}