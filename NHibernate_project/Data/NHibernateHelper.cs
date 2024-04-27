using System.Reflection;
using FluentNHibernate;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Dialect;
using NHibernate.Driver;
using ISession = NHibernate.ISession;

namespace NHibernate_project.Data;

public static class NHibernateHelper
{

    public static IServiceCollection AddNHibernate(this IServiceCollection services, string connectionString)
    {
        var configuration = CreateConfiguration(connectionString);
        
        var mappings = Assembly.GetExecutingAssembly();
        
        configuration.AddMappingsFromAssembly(mappings);
        
        var sessionFactory = configuration.BuildSessionFactory();

        services.AddSingleton(sessionFactory);
        services.AddScoped<ISession>(_ => sessionFactory.OpenSession());
        
        services.AddScoped<IMapperSession, NHibernateMapperSession>();
        
        return services;
    }

    public static Configuration CreateConfiguration(string connectionString)
    {
        var configuration = new Configuration();
        
        configuration.DataBaseIntegration(db =>
        {
            db.Dialect<MySQL57Dialect>();
            db.Driver<MySqlDataDriver>();
            db.ConnectionProvider<DriverConnectionProvider>();
            db.ConnectionString = connectionString;
            db.LogSqlInConsole = true;
            db.LogFormattedSql = true;
            db.SchemaAction = SchemaAutoAction.Create;
        });

        return configuration;
    }

}