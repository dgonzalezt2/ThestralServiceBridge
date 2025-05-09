using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using RabbitMQ.Client;
using ThestralServiceBridge.Infrastructure.Cache;
using ThestralServiceBridge.Infrastructure.GovCarpeta;
using ThestralServiceBridge.Infrastructure.MessageBroker;
using ThestralServiceBridge.Infrastructure.MessageBroker.Options;

namespace ThestralServiceBridge.Infrastructure.Configuration;

public static class ConfigureServices
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IGetOperatorProcess, GetOperatorProcess>();
        services.AddSingleton<IOperatorsHttpClient, OperatorsHttpClient>();
        services.AddSingleton<IMessagePublisher, RabbitMQPublisher>();
        services.AddSingleton<IAsyncPolicy<HttpResponseMessage>>(provider => GetRetryPolicy());
        services.AddHttpClient("GovCarpeta",
            client =>
            {
                client.BaseAddress = new Uri(configuration["GovCarpeta:BaseUrl"] ??
                                             "https://govcarpeta-apis-4905ff3c005b.herokuapp.com/");
            });

        services.AddStackExchangeRedisCache(options => { configuration.Bind("Cache:Redis", options); });
        services.AddSingleton<ICacheStore, CacheStore>();
        services.Configure<PublisherConfiguration>(options =>
            configuration.GetSection("RabbitMQ:Queues:Publisher").Bind(options)
        );
        services.AddSingleton<ConnectionFactory>(sp =>
        {
            var factory = new ConnectionFactory();
            configuration.GetSection("RabbitMQ:Connection").Bind(factory);
            return factory;
        });
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return Policy.Handle<HttpRequestException>()
            .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }
}