using Microsoft.EntityFrameworkCore;
using Notification;
using Notification.Data;

var builder = Host.CreateApplicationBuilder(args);

var conn = builder.Configuration["ConnectionStrings:DefaultConnection"];

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(conn));

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
