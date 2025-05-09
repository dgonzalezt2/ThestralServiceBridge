using System.Text.Json.Serialization;

namespace ThestralServiceBridge.Domain.GovCarpeta.Dtos;

public class OperatorDto
{
    [JsonPropertyName("_id")] public string OperatorId { get; set; }

    [JsonPropertyName("operatorName")] public string OperatorName { get; set; }

    [JsonPropertyName("transferAPIURL")] public string? TransferAPIURL { get; set; }
}