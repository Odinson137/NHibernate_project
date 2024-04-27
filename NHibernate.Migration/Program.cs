using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using NHibernate;
using NHibernate_project.Data;

var path = GetParent(4);

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile($"{path}/appsettings.json", optional: false)
    .AddJsonFile($"{path}/appsettings.Development.json", optional: false)
    .Build();

var connectionString = configuration.GetConnectionString("MariaDb")!;

var cfg = NHibernateHelper.CreateConfiguration(connectionString);

var sessionFactory = cfg.BuildSessionFactory();

using (var session = sessionFactory.OpenSession())
using (var transaction = session.BeginTransaction())
{
    CreateMigrationTable(session);

    var dbMigrations = GetMigrationNames(session);
    
    var migrationFiles =
        Directory.GetFiles($"{GetParent(4, false)}/Migrations", "*.sql").ToList();
    
    migrationFiles.Sort();

    if (dbMigrations.Count == migrationFiles.Count)
    {
        Console.WriteLine("Количество миграций осталось прежним - новых миграций нет");
        return;
    }
    
    foreach (var migrationFile in migrationFiles)
    {
        var migrationTitle = migrationFile.Split("\\").Last();
        if (dbMigrations.Contains(migrationTitle)) continue;
        
        var migrationScript = File.ReadAllText(migrationFile);
        session.CreateSQLQuery(migrationScript).ExecuteUpdate();
        
        AddMigration(session, GetQuery("AddMigration"), migrationTitle);
    }

    transaction.Commit();
}

Console.WriteLine("Migrations applied successfully.");

string GetParent(int upCount = 3, bool isMainProject = true)
{
    var currentDirectory = Directory.GetCurrentDirectory();
    for (var i = 0; i < upCount; i++)
    {
        currentDirectory = Directory.GetParent(currentDirectory)!.ToString();
    }

    var projectDirectory = isMainProject ? "NHibernate_project" : "NHibernate.Migration";
    return @$"{currentDirectory}/{projectDirectory}";
}

void CreateMigrationTable(ISession session)
{
    var checkQueryScript = GetQuery("CheckMigrationTableExist");
    var result = session.CreateSQLQuery(checkQueryScript).UniqueResult<long>();
    if (result != 0) return;
    
    var createQueryScript = GetQuery("CreateMigrationTable");
    session.CreateSQLQuery(createQueryScript).ExecuteUpdate();
}

ICollection<string> GetMigrationNames(ISession session)
{
    var result = session.CreateSQLQuery(GetQuery("GetMigrations")).List<string>();

    return result;
}

string GetQuery(string title) => File.ReadAllText(@$"{GetParent(4, false)}/Queries/{title}.sql");

void AddMigration(ISession session, string query, params string[] args)
{
    var sqlQuery = session.CreateSQLQuery(query);
    var num = 0;
    foreach (var arg in args)
    {
        sqlQuery.SetParameter(num++, arg);
    }

    sqlQuery.ExecuteUpdate();
}