// MicroORMLibraryApp/Program.cs
using MicroORMLibraryApp.Repository;
using MicroORMLibraryApp.Services;
using Microsoft.Extensions.Configuration;
using System.Globalization;

class Program
{
    static void Main()
    {
        // Встановлюємо інваріантну культуру для коректного парсингу
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

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
            var menuService = new ConsoleMenuService(repository, connectionString);
            
            // Запуск програми
            menuService.Run();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Критична помилка: {ex.Message}");
            Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
            Console.ResetColor();
            Console.WriteLine("\nНатисніть будь-яку клавішу для виходу...");
            Console.ReadKey();
        }
    }
}