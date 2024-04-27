using Microsoft.AspNetCore.Mvc;
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

app.MapGet("/GetBook", async (
        IMapperSession session, 
        ILogger<Program> logger,
        [FromQuery] long bookId) =>
    {
        logger.LogInformation($"Get book by id - {bookId}");
        var products = await session.Books
            .Select(c => new
            {
                c.Id, 
                c.Title, 
                Chapters = c.Chapters!.Select(x => new
                {
                    x.Id,
                    x.Title,
                }), 
                Genres = c.Genres!.Select(x => new {x.Id, x.Title}),
            })
            .FirstOrDefaultAsync();
        
        logger.LogInformation($"Got book by id - {bookId}");

        return products;
    })
    .WithName("GetBook")
    .WithOpenApi();

app.MapGet("/GetGenres", async (
        IMapperSession session,
        ILogger<Program> logger) =>
    {
        logger.LogInformation("Get genres");

        var genres = await session.Genres.Select(c => new {c.Id, c.Title}).ToListAsync();
        logger.LogInformation("Got genres");

        return genres;
    })
    .WithName("GetGenres")
    .WithOpenApi();

app.MapGet("/GetGenre", async (
        IMapperSession session, 
        ILogger<Program> logger,
        [FromQuery] long genreId) =>
    {
        logger.LogInformation($"Get genre by id - {genreId}");
        var products = await session.Genres
            .Select(c => new
            {
                c.Id, 
                c.Title, 
                BookCount = c.Books!.Count,
            })
            .FirstOrDefaultAsync();
        
        logger.LogInformation("Got genre");

        return products;
    })
    .WithName("GetGenre")
    .WithOpenApi();

app.MapPost("/AddBook", async (
        IMapperSession session, 
        ILogger<Program> logger,
        [FromQuery] string bookTitle) =>
    {
        logger.LogInformation($"Add book with bookTitle - {bookTitle}");

        var newBook = new Book
        {
            Title = bookTitle,
        };

        await session.RunInTransaction(async () =>
        {
            await session.Save(newBook);
        });

        logger.LogInformation("Added book");
        
        return newBook;
    })
    .WithName("AddBook")
    .WithOpenApi();


app.MapPut("/UpdateBook", async (
        IMapperSession session, 
        ILogger<Program> logger,
        [FromQuery] long bookId,
        [FromQuery] string newBookTitle) =>
    {
        logger.LogInformation($"Update book bookId - {bookId} & newBookTitle - {newBookTitle}");

        var book = await session.Books.Where(c => c.Id == bookId).FirstOrDefaultAsync();
        if (book == null)
        {
            return Results.Ok("The book is not found");
        }

        book.Title = newBookTitle;

        session.BeginTransaction();
        
        await session.Update(book);
        await session.Commit();
        
        session.CloseTransaction();

        logger.LogInformation($"Updated book bookId");

        return Results.Ok("The book is successfully changed title");

    })
    .WithName("UpdateBook")
    .WithOpenApi();

app.MapDelete("/DeleteBook", async (
        IMapperSession session, 
        ILogger<Program> logger,
        [FromQuery] long bookId) =>
    {
        logger.LogInformation($"Delete book bookId - {bookId}");

        var book = await session.Books.Where(c => c.Id == bookId).FirstOrDefaultAsync();
        if (book == null)
        {
            return Results.Ok("The book is not found");
        }

        session.BeginTransaction();

        await session.Delete(book);

        await session.Commit();
        
        session.CloseTransaction();
        
        
        logger.LogInformation($"Deleted book");

        return Results.Ok("The book is successfully deleted");
    })
    .WithName("DeleteBook")
    .WithOpenApi();

app.Run();

