namespace NHibernate_project.Models;

public class Address
{
    public virtual string Street { get; set; } = null!;
    public virtual string City { get; set; } = null!;
    public virtual int Number { get; set; }
}