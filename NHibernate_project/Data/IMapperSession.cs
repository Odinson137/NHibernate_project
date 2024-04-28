using NHibernate_project.Models;

namespace NHibernate_project.Data;

public interface IMapperSession
{
    void BeginTransaction();
    Task Commit();
    Task Rollback();
    void CloseTransaction();
    Task Save<T>(T entity);
    Task Update<T>(T entity);
    Task Delete<T>(T entity);
    Task RunInTransaction(Func<Task> func);
    IQueryable<User> Users { get; }
    IQueryable<Book> Books { get; }
    IQueryable<Genre> Genres { get; }
    IQueryable<Chapter> Chapters { get; }
}