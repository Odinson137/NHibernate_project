using FluentNHibernate.Mapping;
using NHibernate_project.Models;

namespace NHibernate_project.Mappings;

public class GenreMap : ClassMap<Genre>
{
    public GenreMap()
    {
        Id(c => c.Id);

        Map(c => c.Title)
            .Length(50)
            .Not.Nullable();

        HasManyToMany(c => c.Books);
    }
}