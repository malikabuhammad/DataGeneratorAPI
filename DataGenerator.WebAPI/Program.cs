using DataGenerator.Application.Interfaces;
using DataGenerator.Application.Services;
using DataGenerator.Domain.Interfaces;
using DataGenerator.Infrastructure.Repositories;
using DataGenerator.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<SqlDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IDataGeneratorRepository, DataGeneratorRepository>();
builder.Services.AddScoped<IDataGenerationService, DataGenerationService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Data Generator API v1");
    options.RoutePrefix = string.Empty; 
});


 app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.Run();
