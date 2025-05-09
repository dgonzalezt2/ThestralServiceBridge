using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace ThestralServiceBridge.Infrastructure.MessageBroker;

public class RabbitMQPublisher(ConnectionFactory connectionFactory, ILogger<RabbitMQPublisher> logger)
    : IMessagePublisher
{
    public async Task SendMessageAsync<T>(T message, string queueName, IDictionary<string, object?>? headers = null)
    {
        logger.LogInformation("Sending message to RabbitMQ queue: {QueueName}", queueName);
        await using var connection = await connectionFactory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();
        var serializedMessage = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(serializedMessage);
        var props = new BasicProperties();
        if (headers is not null) props.Headers = headers;

        await channel.BasicPublishAsync("",
            queueName,
            true,
            props,
            body);
        logger.LogInformation("Message sent to RabbitMQ queue: {QueueName}", queueName);
    }
}