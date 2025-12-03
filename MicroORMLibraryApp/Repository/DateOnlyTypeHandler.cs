// MicroORMLibraryApp/Repository/DateOnlyTypeHandler.cs
using Dapper;
using System.Data;
using System.Globalization;

namespace MicroORMLibraryApp.Repository
{
    public class DateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
    {
        public override void SetValue(IDbDataParameter parameter, DateOnly value)
        {
            parameter.Value = value.ToDateTime(TimeOnly.MinValue);
        }

        public override DateOnly Parse(object value)
        {
            if (value == null || value is DBNull)
                return default;

            if (value is DateOnly dateOnly)
                return dateOnly;

            if (value is DateTime dateTime)
                return DateOnly.FromDateTime(dateTime);

            if (value is string str)
            {
                // Спробуємо різні формати дат
                var formats = new[] 
                { 
                    "dd.MM.yyyy",       // 10.01.2024
                    "dd/MM/yyyy",       // 10/01/2024
                    "yyyy-MM-dd",       // 2024-01-10
                    "MM/dd/yyyy",       // 01/10/2024
                    "dd-MM-yyyy",       // 10-01-2024
                    "d.M.yyyy",         // 10.1.2024
                    "d/M/yyyy"          // 10/1/2024
                };
                
                foreach (var format in formats)
                {
                    if (DateOnly.TryParseExact(str, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                        return result;
                }
                
                // Стандартний парсинг
                if (DateOnly.TryParse(str, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
                    return parsed;
            }

            return default;
        }
    }

    public class NullableDateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly?>
    {
        public override void SetValue(IDbDataParameter parameter, DateOnly? value)
        {
            parameter.Value = value?.ToDateTime(TimeOnly.MinValue) ?? (object)DBNull.Value;
        }

        public override DateOnly? Parse(object value)
        {
            if (value == null || value is DBNull)
                return null;

            if (value is DateOnly dateOnly)
                return dateOnly;

            if (value is DateTime dateTime)
                return DateOnly.FromDateTime(dateTime);

            if (value is string str)
            {
                // Спробуємо різні формати дат
                var formats = new[] 
                { 
                    "dd.MM.yyyy",       // 10.01.2024
                    "dd/MM/yyyy",       // 10/01/2024
                    "yyyy-MM-dd",       // 2024-01-10
                    "MM/dd/yyyy",       // 01/10/2024
                    "dd-MM-yyyy",       // 10-01-2024
                    "d.M.yyyy",         // 10.1.2024
                    "d/M/yyyy"          // 10/1/2024
                };
                
                foreach (var format in formats)
                {
                    if (DateOnly.TryParseExact(str, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                        return result;
                }
                
                // Стандартний парсинг
                if (DateOnly.TryParse(str, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
                    return parsed;
            }

            return null;
        }
    }

    // Обробник для конвертації DateOnly в DateTime
    public class DateOnlyToDateTimeTypeHandler : SqlMapper.TypeHandler<DateTime>
    {
        public override void SetValue(IDbDataParameter parameter, DateTime value)
        {
            parameter.Value = value;
        }

        public override DateTime Parse(object value)
        {
            if (value == null || value is DBNull)
                return DateTime.MinValue;

            if (value is DateTime dateTime)
                return dateTime;

            if (value is DateOnly dateOnly)
                return dateOnly.ToDateTime(TimeOnly.MinValue);

            if (value is string str)
            {
                // Спочатку спробуємо як DateOnly
                var formats = new[] 
                { 
                    "dd.MM.yyyy",       // 10.01.2024
                    "dd/MM/yyyy",       // 10/01/2024
                    "yyyy-MM-dd",       // 2024-01-10
                    "MM/dd/yyyy",       // 01/10/2024
                    "dd-MM-yyyy",       // 10-01-2024
                };
                
                foreach (var format in formats)
                {
                    if (DateOnly.TryParseExact(str, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateOnlyResult))
                        return dateOnlyResult.ToDateTime(TimeOnly.MinValue);
                }
                
                // Якщо не вийшло, спробуємо як DateTime
                if (DateTime.TryParse(str, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTimeResult))
                    return dateTimeResult;
            }

            return DateTime.MinValue;
        }
    }

    public class DateOnlyToNullableDateTimeTypeHandler : SqlMapper.TypeHandler<DateTime?>
    {
        public override void SetValue(IDbDataParameter parameter, DateTime? value)
        {
            parameter.Value = value ?? (object)DBNull.Value;
        }

        public override DateTime? Parse(object value)
        {
            if (value == null || value is DBNull)
                return null;

            if (value is DateTime dateTime)
                return dateTime;

            if (value is DateOnly dateOnly)
                return dateOnly.ToDateTime(TimeOnly.MinValue);

            if (value is string str)
            {
                // Спочатку спробуємо як DateOnly
                var formats = new[] 
                { 
                    "dd.MM.yyyy",       // 10.01.2024
                    "dd/MM/yyyy",       // 10/01/2024
                    "yyyy-MM-dd",       // 2024-01-10
                    "MM/dd/yyyy",       // 01/10/2024
                    "dd-MM-yyyy",       // 10-01-2024
                };
                
                foreach (var format in formats)
                {
                    if (DateOnly.TryParseExact(str, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateOnlyResult))
                        return dateOnlyResult.ToDateTime(TimeOnly.MinValue);
                }
                
                // Якщо не вийшло, спробуємо як DateTime
                if (DateTime.TryParse(str, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTimeResult))
                    return dateTimeResult;
            }

            return null;
        }
    }

    public static class DapperConfig
    {
        public static void Configure()
        {
            // Реєструємо обробники для DateOnly
            SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
            SqlMapper.AddTypeHandler(new NullableDateOnlyTypeHandler());
            
            // Реєструємо обробники для конвертації DateOnly в DateTime
            SqlMapper.AddTypeHandler(new DateOnlyToDateTimeTypeHandler());
            SqlMapper.AddTypeHandler(new DateOnlyToNullableDateTimeTypeHandler());
            
            // Додаємо маппінг для типів
            SqlMapper.RemoveTypeMap(typeof(DateOnly));
            SqlMapper.RemoveTypeMap(typeof(DateOnly?));
            SqlMapper.RemoveTypeMap(typeof(DateTime));
            SqlMapper.RemoveTypeMap(typeof(DateTime?));
            
            SqlMapper.AddTypeMap(typeof(DateOnly), DbType.Date);
            SqlMapper.AddTypeMap(typeof(DateOnly?), DbType.Date);
            SqlMapper.AddTypeMap(typeof(DateTime), DbType.DateTime);
            SqlMapper.AddTypeMap(typeof(DateTime?), DbType.DateTime);
        }
    }
}