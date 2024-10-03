using Justwish.Notifications.Application;
using Justwish.Notifications.Contracts;
using Justwish.Notifications.Infrastructure;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication(builder.Configuration, builder.Environment)
    .AddInfrastructure(builder.Configuration, builder.Environment);

var app = builder.Build();

app.MapGet("/", async (IRequestClient<SendEmailVerificationRequest> client) =>
{
    var response = await client.GetResponse<SendEmailVerificationResponse>(
        new SendEmailVerificationRequest("blit228@mail.ru", 1234));
    return response.Message.Success ? "OK" : "FAIL";
});

app.Run();
