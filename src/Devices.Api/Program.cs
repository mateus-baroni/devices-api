using Devices.Infrastructure.Persistence;
using Devices.Application.Devices;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Devices.Api.Exceptions;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
    });

//Services
builder.Services.AddScoped<DeviceService>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DevicesDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DevicesDb"));
});

builder.Services.AddHealthChecks()
    .AddDbContextCheck<DevicesDbContext>();

var app = builder.Build();

app.UseExceptionHandler();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<DevicesDbContext>();

    dbContext.Database.Migrate();
}

// Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

public partial class Program
{
}