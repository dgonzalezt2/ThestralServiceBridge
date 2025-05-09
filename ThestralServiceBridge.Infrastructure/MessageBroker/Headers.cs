namespace ThestralServiceBridge.Infrastructure.MessageBroker;

public class Headers(string messageType, string userId)
{
    public string UserId { get; set; } = userId;
    public Guid MessageId { get; set; } = Guid.NewGuid();
    public string EventType { get; set; } = messageType;
    public string Timestamp { get; set; } = DateTimeOffset.UtcNow.ToString("o");
    public int Priority { get; set; } = 1;
    public string ServiceName { get; set; } = "ThestralServiceBridge";

    public Dictionary<string, object?> GetAttributesAsDictionary()
    {
        var dictionary = new Dictionary<string, object?>();

        foreach (var property in GetType().GetProperties())
        {
            var value = property.PropertyType == typeof(Guid)
                ? property.GetValue(this)?.ToString()
                : property.GetValue(this);
            if (value != null) dictionary[property.Name] = value;
        }

        return dictionary;
    }
}