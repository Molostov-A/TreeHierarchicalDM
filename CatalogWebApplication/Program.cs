using CatalogWebApplication.Service;
using CatalogWebApplication.Repository;
using Microsoft.EntityFrameworkCore;
using CatalogWebApplication.Context;
using System;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var databaseType = builder.Configuration["Database:Type"];

if (databaseType == "MongoDB")
{
    builder.Services.AddSingleton<MongoDbContext>();
    builder.Services.AddScoped<ICatalogRepository, MongoCatalogRepository>();
}
else if (databaseType == "SqlServer")
{
    builder.Services.AddDbContext<SqlDbContext>(options =>
        options.UseSqlServer(builder.Configuration["Database:ConnectionStrings:SqlServer"]));
    builder.Services.AddScoped<ICatalogRepository, SqlCatalogRepository>();
}

builder.Services.AddScoped<CatalogService>();

var app = builder.Build();

if (databaseType == "SqlServer")
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<SqlDbContext>();
        dbContext.Database.EnsureCreated();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
