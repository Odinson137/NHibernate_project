using System.Data;
using NHibernate_project.Models;
using NHibernate.Linq;
using ISession = NHibernate.ISession;

namespace NHibernate_project.Data;

public static class Seed
{
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    public static bool IsEnabled = true;
    
    public static async Task Seeding(this WebApplication webApplication)
    {
        var logger = webApplication.Services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation($"Разрешено ли сидирование: {IsEnabled}");
        
        if (!IsEnabled) return;

        await using var serviceScope = webApplication.Services.CreateAsyncScope();
        using var session = serviceScope.ServiceProvider.GetRequiredService<ISession>();

        if (await session.Query<Book>().CountAsync() != 0)
        {
            logger.LogInformation("Данные уже есть");
            return;
        }

        logger.LogInformation("Запись данных");

        using var transaction = session.BeginTransaction();
        
        var user1 = new User
        {
            UserName = "Admin",
            Name = "Yuriy",
            LastName = "Buryy",
            Password = "12345678",
            Address = new Address
            {
                Street = "Острожских",
                City = "Минск",
                Number = 8,
            }
        };
        
        var user2 = new User
        {
            UserName = "Kactus25",
            Name = "Baget",
            LastName = "Baget",
            Password = "12345678",
            Address = new Address
            {
                Street = "Багетная",
                City = "Минск",
                Number = 25,
            }
        };

        await session.SaveCollectionAsync(user1, user2);
        
        var book = new Book
        {
            Title = "Hello from seed",
            Chapters = new List<Chapter>(),
            Genres = new HashSet<Genre>(),
        };
        
        var chapter1 = new Chapter {
            Title = "Chapter 1",
            Book = book,
        };
        
        var chapter2 = new Chapter {
            Title = "Chapter 2",
            Book = book,
        };
        
        var chapter3 = new Chapter {
            Title = "Chapter 3",
            Book = book,
        };

        await session.SaveCollectionAsync(chapter1, chapter2, chapter3);

        var genre1 = new Genre()
        {
            Title = "Drama",
        };
        
        var genre2 = new Genre()
        {
            Title = "Action",
        };
        
        var genre3 = new Genre()
        {
            Title = "Comedy",
        };

        await session.SaveCollectionAsync(genre1, genre2, genre3);
        
        book.Genres.Add(genre1);
        book.Genres.Add(genre2);
        book.Genres.Add(genre3);

        await session.SaveAsync(book);
        
        await transaction.CommitAsync();
        
        logger.LogInformation("Данные добавлены");
    }
    
    private static async Task SaveCollectionAsync<T>(this ISession session, params T[] args)
    {
        foreach (var arg in args)
        {
            await session.SaveAsync(arg);
        }
    }
    
    public async static Task<WebApplication> AlterSchemaCollation(this WebApplication webApplication)
    {
        var logger = webApplication.Services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Change schema");
        await using var serviceScope = webApplication.Services.CreateAsyncScope();
        using var session = serviceScope.ServiceProvider.GetRequiredService<ISession>();
        
        if (await session.Query<Book>().CountAsync() != 0)
        {
            logger.LogInformation("Данные уже есть");
            return webApplication;
        }
        
        await using var connection = session.Connection;
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = "alter schema NHibernateDb collate utf8_general_ci;";
        await command.ExecuteNonQueryAsync();

        return webApplication;
    }
}