using FluentNHibernate.Mapping;
using NHibernate_project.Models;

namespace NHibernate_project.Mappings;

public class AddressComponentMap : ComponentMap<Address>
{
    public AddressComponentMap()
    {
        Map(c => c.City);
        Map(c => c.Street);
        Map(c => c.Number);
    }
}