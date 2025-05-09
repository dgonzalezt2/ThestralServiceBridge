namespace ThestralServiceBridge.Infrastructure.MessageBroker;

public interface IMessagePublisher
{
    Task SendMessageAsync<T>(T message, string queueName, IDictionary<string, object?>? headers = null);
}