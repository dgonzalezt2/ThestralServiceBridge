using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Scalar.AspNetCore;
using ThestralServiceBridge.Infrastructure.Configuration;
using ThestralServiceBridge.Infrastructure.MessageBroker.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddInfrastructure(builder.Configuration);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Servers = [];
        options.WithTitle("ThestralBridge API")
            .WithFavicon("/favicon.ico")
            .WithTheme(ScalarTheme.DeepSpace)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
            .WithSidebar(true);
    });
    app.UseStaticFiles();
}

app.UseCors(corsPolicyBuilder =>
{
    corsPolicyBuilder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
        .WithExposedHeaders("X-User-Id")
        .WithExposedHeaders("X-Pagination-Total-Pages")
        .WithExposedHeaders("X-Pagination-Next-Page")
        .WithExposedHeaders("X-Pagination-Has-Next-Page")
        .WithExposedHeaders("X-Pagination-Total-Pages");
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var connectionFactory = scope.ServiceProvider.GetRequiredService<ConnectionFactory>();
    await using var connection = await connectionFactory.CreateConnectionAsync();
    await using var channel = await connection.CreateChannelAsync();
    var queues = scope.ServiceProvider.GetRequiredService<IOptions<PublisherConfiguration>>().Value;
    var userTransferNotificationQueue = queues.UserTransferRequestQueue;
    await channel.QueueDeclareAsync(queue: userTransferNotificationQueue,
        durable: true,
        exclusive: false,
        autoDelete: false,
        arguments: null);
}

app.Run();