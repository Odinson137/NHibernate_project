namespace NHibernate_project.Models;

public class Chapter
{
    public virtual Guid Id { get; set; }
    public virtual string? Title { get; set; }
    public virtual Guid BookId { get; set; }
}