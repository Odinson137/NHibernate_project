using FluentNHibernate.Mapping;
using NHibernate_project.Models;

namespace NHibernate_project.Mappings;

public class UserMap : ClassMap<User>
{
    public UserMap()
    {
        Id(c => c.Id);

        Map(c => c.UserName).Length(50).Not.Nullable();
        Map(c => c.Name).Length(50).Nullable();
        Map(c => c.LastName).Length(50).Nullable();
        Map(c => c.Password).Length(50).Not.Nullable();
        
        // Инициализация этого компонента идёт в AddressComponentMap
        Component(c => c.Address);
    }
}