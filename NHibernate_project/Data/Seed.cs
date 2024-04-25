using NHibernate;
using NHibernate_project.Models;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using ISession = NHibernate.ISession;

namespace NHibernate_project.Data;

public static class Seed
{
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    public static bool IsEnabled = true;
    
    public static void Seeding(this WebApplication webApplication)
    {
        if (!IsEnabled) return;

        using var serviceScope = webApplication.Services.CreateAsyncScope();
        using var session = serviceScope.ServiceProvider.GetRequiredService<ISession>();
        using var transaction = session.BeginTransaction();

        if (session.Query<Book>().Any())
        {
            Console.WriteLine("Данные уже есть");
            return;
        }
        
        session.SaveAsync(new Book
        {
            Id = Guid.NewGuid(),
            Title = "Привет из сида",
        });

        transaction.Commit();
    }
    static bool TableExists(ISessionFactory sessionFactory, string tableName)
    {
        using var session = sessionFactory.OpenSession();
        var connection = session.Connection;
        var cmd = connection.CreateCommand();
        cmd.CommandText = $"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{tableName}'";
        var result = Convert.ToInt32(cmd.ExecuteScalar());
        return result > 0;
    }
}