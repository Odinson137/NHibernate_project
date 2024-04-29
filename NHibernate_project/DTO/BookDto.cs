namespace NHibernate_project.DTO;

public class BookDto
{
    public virtual string Title { get; set; } = null!;
    public virtual ICollection<ChapterDto>? Chapters { get; set; }
    public virtual ICollection<GenreDto>? Genres { get; set; }
}