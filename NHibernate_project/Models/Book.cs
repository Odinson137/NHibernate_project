namespace NHibernate_project.Models;

public class Book
{
    public virtual Guid Id { get; set; }
    public virtual string Title { get; set; } = null!;
    // public virtual ICollection<Chapter>? Chapters { get; set; }
}