using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NHibernate_project.Data;
using NHibernate_project.Models;
using NHibernate.Linq;

namespace NHibernate_project.ApplicationMaps;

public static class BookMinimalController
{
    public static void InitBookController(this WebApplication app)
    {
        app.MapGet("/GetBooks", async (IMapperSession session) =>
            {
                var books = await session.Books
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
                return books;
            })
            .WithName("GetBooks")
            .WithOpenApi();
        
        app.MapGet("/GetBook", async (
                IMapperSession session, 
                ILogger<Program> logger,
                [FromQuery] long bookId) =>
            {
                logger.LogInformation($"Get book by id - {bookId}");
                var book = await session.Books
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
        
                return book;
            })
            .WithName("GetBook")
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

    }
}