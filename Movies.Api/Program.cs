using Microsoft.AspNetCore.Localization;
using Movies.Api.Middlewares;
using Movies.Application;
using Movies.Application.Database.Initializers;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();

var connectionString = configuration.GetConnectionString("DefaultConnection");

ArgumentException.ThrowIfNullOrWhiteSpace(connectionString, nameof(connectionString));

builder.Services.AddDatabase(connectionString);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en-US")
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware(typeof(ValidationErrorMiddleware));

app.MapControllers();

var dbInitializer = app.Services.GetRequiredService<DatabaseInitializer>();

await dbInitializer.InitializeAsync();

app.Run();
