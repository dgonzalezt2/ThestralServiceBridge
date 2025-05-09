using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ThestralServiceBridge.Domain.Transfer.Dtos;

public class TransferReceivedPackageDto
{
    [JsonPropertyName("id")]
    public uint Id { get; set; }
    [JsonPropertyName("citizenName")]
    public required string Name { get; set; }
    [JsonPropertyName("citizenEmail")]
    [EmailAddress]
    public required string Email { get; set; }
    [JsonPropertyName("confirmAPI")]
    public required string CallbackUrl { get; set; }
    [JsonPropertyName("urlDocuments")]
    public required Dictionary<string, string[]> Files { get; set; }
}