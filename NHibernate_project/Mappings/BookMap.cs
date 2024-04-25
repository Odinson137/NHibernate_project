using FluentNHibernate.Mapping;
using NHibernate;
using NHibernate_project.Models;
using NHibernate.Mapping.ByCode;

namespace NHibernate_project.Mappings;

public class BookMap : ClassMap<Book>
{
    public BookMap()
    {
        Id(x => x.Id, "Id");

        Map(c => c.Title)
            .Length(50)
            .Not.Nullable();

        HasMany(c => c.Chapters)
            .Cascade.All();
        
        HasManyToMany(c => c.Genres);
        
        Table("Books");
    }
}