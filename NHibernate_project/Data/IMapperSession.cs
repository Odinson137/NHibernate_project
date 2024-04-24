using NHibernate_project.Models;

namespace NHibernate_project.Data;

public interface IMapperSession
{
    void BeginTransaction();
    Task Commit();
    Task Rollback();
    void CloseTransaction();
    Task Save(Book entity);
    Task Update(Book entity);
    Task Delete(Book entity);
    Task RunInTransaction(Func<Task> func);
    IQueryable<Book> Books { get; }
}