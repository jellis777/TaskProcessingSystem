using Microsoft.EntityFrameworkCore;
using TaskProcessing.Api.Data;
using System.Text.Json.Serialization;
using TaskProcessing.Api.Interfaces;
using TaskProcessing.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    })
;
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



builder.Services.AddScoped<ITaskService, TaskService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
{
    policy.WithOrigins(
        "http://localhost:5173",
        "http://localhost:5174",
        "http://localhost:5175",
       "https://wonderful-tree-0b6cc6d10.4.azurestaticapps.net"
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
});
});

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.UseHttpsRedirection();

app.UseCors("Frontend");

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program
{

}
