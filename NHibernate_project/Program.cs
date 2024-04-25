using Microsoft.AspNetCore.Http.HttpResults;
using NHibernate_project.Data;
using NHibernate_project.Models;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Linq;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddNHibernate(configuration.GetConnectionString("MariaDb")!);

var app = builder.Build();

// Заполнение таблиц данными
app.Seeding();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/GetBooks", async (IMapperSession session) =>
    {
        var products = await session.Books.ToListAsync();
        return products;
    })
    .WithName("GetBooks")
    .WithOpenApi();

app.MapPost("/AddBook", async (IMapperSession session, string bookTitle) =>
    {
        var newBook = new Book
        {
            Id = Guid.NewGuid(),
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
        string bookId,
        string newBookTitle) =>
    {
        var book = await session.Books.Where(c => c.Id == Guid.Parse(bookId)).FirstOrDefaultAsync();
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
        string bookId) =>
    {
        var book = await session.Books.Where(c => c.Id == Guid.Parse(bookId)).FirstOrDefaultAsync();
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

