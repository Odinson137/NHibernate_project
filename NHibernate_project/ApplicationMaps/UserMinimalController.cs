using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NHibernate_project.Data;
using NHibernate_project.DTO;
using NHibernate_project.Models;
using NHibernate.Linq;

namespace NHibernate_project.ApplicationMaps;

public static class UserMinimalController
{
    public static void InitUserController(this WebApplication app)
    {
        app.MapGet("/GetUsers", async (
                IMapperSession session,
                IMapper mapper) =>
            {
                var users = await session.Users
                    .ProjectTo<UserDto>(mapper.ConfigurationProvider)
                    .ToListAsync();
                return users;
            })
            .WithName("GetUsers")
            .WithOpenApi();
        
        app.MapGet("/GetUser", async (
                IMapperSession session, 
                ILogger<Program> logger,
                IMapper mapper,
                [FromQuery] long userId) =>
            {
                logger.LogInformation($"Get user by id - {userId}");
                var book = await session.Users
                    .ProjectTo<UserDto>(mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync();
                
                logger.LogInformation($"Got user by id - {userId}");
        
                return book;
            })
            .WithName("GetUser")
            .WithOpenApi();
        
        app.MapPost("/AddUser", async (
                IMapperSession session, 
                ILogger<Program> logger,
                IMapper mapper,
                [FromBody] UserDto userDto) =>
            {
                logger.LogInformation($"Add user\n{JsonConvert.SerializeObject(userDto)}");

                var user = mapper.Map<User>(userDto);
                await session.Save(user);
                
                logger.LogInformation("Added user");
        
                return user;
            })
            .WithName("AddUser")
            .WithOpenApi();
    }
}