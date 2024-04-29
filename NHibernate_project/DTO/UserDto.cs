using NHibernate_project.Models;

namespace NHibernate_project.DTO;

public class UserDto
{
    public virtual string UserName { get; set; } = null!;
    public virtual string? Name { get; set; }
    public virtual string? LastName { get; set; }
    public virtual string Password { get; set; } = null!;
    public virtual Address Address { get; set; } = null!;
}