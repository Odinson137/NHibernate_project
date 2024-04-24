using NHibernate;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Mapping.ByCode;
namespace NHibernate_project.Data;

public static class NHibernateHelper
{
    private static ISessionFactory _sessionFactory;

    public static IServiceCollection AddNHibernate(this IServiceCollection services, string connectionString)
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
        });
        
        var mapper = new ModelMapper();
        
        mapper.AddMappings(typeof(NHibernateHelper).Assembly.ExportedTypes);
        var domainMapping = mapper.CompileMappingForAllExplicitlyAddedEntities();
        configuration.AddMapping(domainMapping);
        
        _sessionFactory = configuration.BuildSessionFactory();

        // Создание таблиц если их нет и заполнение данными
        Seed.Seeding(_sessionFactory, configuration);
        
        services.AddSingleton(_sessionFactory);
        services.AddScoped(factory => _sessionFactory.OpenSession());
        
        services.AddScoped<IMapperSession, NHibernateMapperSession>();
        
        return services;
    }


}