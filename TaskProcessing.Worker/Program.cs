using Microsoft.EntityFrameworkCore;
using TaskProcessing.Api.Data;
using TaskProcessing.Worker;
using TaskProcessing.Worker.Interfaces;
using TaskProcessing.Worker.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ITaskProcessor, TaskProcessor>();

var host = builder.Build();
host.Run();
