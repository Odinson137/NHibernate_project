namespace NHibernate_project.Models;

public class Book : BaseEntity
{
    public virtual string Title { get; set; } = null!;
    public virtual ICollection<Chapter>? Chapters { get; set; }
    public virtual ICollection<Genre>? Genres { get; set; }
}