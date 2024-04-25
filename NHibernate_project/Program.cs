using NHibernate_project.Data;
using NHibernate_project.Models;
using NHibernate.Linq;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddNHibernate(configuration.GetConnectionString("MariaDb")!);

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.Debug()
    .CreateLogger());

var app = builder.Build();

// await app.AlterSchemaCollation();
// Заполнение таблиц данными
await app.Seeding();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/GetBooks", async (IMapperSession session) =>
    {
        var products = await session.Books
            .Select(c => new
            {
                c.Id, 
                c.Title, 
                ChapterTitle = c.Chapters!.Select(x => new
                {
                    x.Id,
                    x.Title,
                }), 
            })
            .ToListAsync();
        return products;
    })
    .WithName("GetBooks")
    .WithOpenApi();

app.MapGet("/GetGenres", async (
        IMapperSession session) =>
    {
        var genres = await session.Genres.Select(c => new {c.Id, c.Title}).ToListAsync();

        return genres;
    })
    .WithName("GetGenres")
    .WithOpenApi();

app.MapPost("/AddBook", async (
        IMapperSession session, 
        string bookTitle) =>
    {
        var newBook = new Book
        {
            Title = bookTitle,
        };

        await session.RunInTransaction(async () =>
        {
            await session.Save(newBook);
        });

        return newBook;
    })
    .WithName("AddBook")
    .WithOpenApi();


app.MapPut("/UpdateBook", async (
        IMapperSession session, 
        long bookId,
        string newBookTitle) =>
    {
        var book = await session.Books.Where(c => c.Id == bookId).FirstOrDefaultAsync();
        if (book == null)
        {
            return Results.Ok("The book is not found");
        }

        book.Title = newBookTitle;
        
        await session.Update(book);
        
        return Results.Ok("The book is successfully changed title");

    })
    .WithName("UpdateBook")
    .WithOpenApi();

app.MapDelete("/DeleteBook", async (
        IMapperSession session, 
        long bookId) =>
    {
        var book = await session.Books.Where(c => c.Id == bookId).FirstOrDefaultAsync();
        if (book == null)
        {
            return Results.Ok("The book is not found");
        }

        await session.Delete(book);
        
        return Results.Ok("The book is successfully deleted");

    })
    .WithName("DeleteBook")
    .WithOpenApi();

app.Run();

