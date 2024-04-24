using NHibernate;
using NHibernate_project.Models;
using ISession = NHibernate.ISession;

namespace NHibernate_project.Data;

public class NHibernateMapperSession : IMapperSession
{
    private readonly ISession _session;
    private ITransaction _transaction = null!;
 
    public NHibernateMapperSession(ISession session)
    {
        _session = session;
    }
 
    public IQueryable<Book> Books => _session.Query<Book>();
 
    public void BeginTransaction()
    {
        _transaction = _session.BeginTransaction();
    }
 
    public async Task Commit()
    {
        await _transaction.CommitAsync();
    }
 
    public async Task Rollback()
    {
        await _transaction.RollbackAsync();
    }
 
    public void CloseTransaction()
    {
        if(_transaction != null)
        {
            _transaction.Dispose();
            _transaction = null;
        }
    }
 
    public async Task Save(Book entity)
    {
        await _session.SaveAsync(entity);
    }
 
    public async Task Update(Book entity)
    {
        await _session.UpdateAsync(entity);
    }
    
    public async Task Delete(Book entity)
    {
        await _session.DeleteAsync(entity);
    }
    
    public async Task RunInTransaction(Func<Task> func)
    {
        try
        {
            BeginTransaction();
 
            await func();
 
            await Commit();
        }
        catch
        {
            await Rollback();
 
            throw;
        }
        finally
        {
            CloseTransaction();
        }
    }
}

