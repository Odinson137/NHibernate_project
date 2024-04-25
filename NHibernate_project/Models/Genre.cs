namespace NHibernate_project.Models;

public class Genre : BaseEntity
{
    public virtual string Title { get; set; } = null!;
    public virtual ISet<Book>? Books { get; set; }
}