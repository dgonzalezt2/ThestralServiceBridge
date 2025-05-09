using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ThestralServiceBridge.Domain.Transfer.Dtos;

public class TransferDto
{
    [JsonPropertyName("externalOperatorId")]
    public required string ExternalOperatorId { get; set; }

    [JsonPropertyName("userEmail")] 
    [EmailAddress]
    public required string UserEmail { get; set; }
}