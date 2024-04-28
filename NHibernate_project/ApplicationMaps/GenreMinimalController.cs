using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NHibernate_project.Data;
using NHibernate_project.Models;
using NHibernate.Linq;

namespace NHibernate_project.ApplicationMaps;

public static class GenreMinimalController
{
    public static void InitGenreController(this WebApplication app)
    {
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
    }
}