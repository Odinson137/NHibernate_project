using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NHibernate_project.Data;
using NHibernate_project.Models;
using NHibernate.Linq;

namespace NHibernate_project.ApplicationMaps;

public static class UserMinimalController
{
    public static void InitUserController(this WebApplication app)
    {
        app.MapGet("/GetUsers", async (IMapperSession session) =>
            {
                var users = await session.Users
                    .Select(c => new
                    {
                        c.Id, 
                        c.UserName, 
                        c.Name,
                        c.LastName,
                        c.Address,
                    })
                    .ToListAsync();
                return users;
            })
            .WithName("GetUsers")
            .WithOpenApi();
        
        app.MapGet("/GetUser", async (
                IMapperSession session, 
                ILogger<Program> logger,
                [FromQuery] long userId) =>
            {
                logger.LogInformation($"Get user by id - {userId}");
                var book = await session.Users
                    .Select(c => new
                    {
                        c.Id, 
                        c.UserName, 
                        c.Name,
                        c.LastName,
                        c.Address,
                    })
                    .FirstOrDefaultAsync();
                
                logger.LogInformation($"Got user by id - {userId}");
        
                return book;
            })
            .WithName("GetUser")
            .WithOpenApi();
        
        app.MapPost("/AddUser", async (
                IMapperSession session, 
                ILogger<Program> logger,
                [FromBody] User user) =>
            {
                logger.LogInformation($"Add user\n{JsonConvert.SerializeObject(user)}");
                
                user.Id = 0;
                await session.Save(user);
                
                logger.LogInformation("Added user");
        
                return user;
            })
            .WithName("AddUser")
            .WithOpenApi();
    }
}