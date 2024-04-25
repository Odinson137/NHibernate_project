namespace NHibernate_project.Models;

public class Chapter : BaseEntity
{
    public virtual string? Title { get; set; }
    public virtual Book Book { get; set; } = null!;
    public virtual long BookId { get; set; }
}