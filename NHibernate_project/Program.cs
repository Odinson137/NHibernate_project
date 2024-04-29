using System.Reflection;
using NHibernate_project.ApplicationMaps;
using NHibernate_project.Data;
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

services.AddAutoMapper(typeof(MappingProfile));

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

// Minimal API
app.InitUserController();
app.InitBookController();
app.InitGenreController();

app.Run();

