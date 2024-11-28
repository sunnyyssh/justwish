using Justwish.Notifications.Application;
using Justwish.Notifications.Contracts;
using Justwish.Notifications.Infrastructure;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication(builder.Configuration, builder.Environment)
    .AddInfrastructure(builder.Configuration, builder.Environment);

var app = builder.Build();

app.Run();
