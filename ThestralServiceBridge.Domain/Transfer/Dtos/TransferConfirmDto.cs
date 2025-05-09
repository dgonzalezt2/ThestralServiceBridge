using System.Text.Json.Serialization;

namespace ThestralServiceBridge.Domain.Transfer.Dtos;

public class TransferConfirmDto
{
    [JsonPropertyName("id")] public uint Id { get; set; }

    [JsonPropertyName("req_status")] public int ReqStatus { get; set; }
}