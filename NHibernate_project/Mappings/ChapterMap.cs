using FluentNHibernate.Mapping;
using NHibernate_project.Models;

namespace NHibernate_project.Mappings;

public class ChapterMap : ClassMap<Chapter>
{
    public ChapterMap()
    {
        Id(x => x.Id);

        Map(c => c.Title).Nullable().Length(50);

        References(c => c.Book).Column("BookId").Cascade.All();
        
        Table("Chapters");
    }
}