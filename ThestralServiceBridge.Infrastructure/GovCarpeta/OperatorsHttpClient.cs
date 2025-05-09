using System.Text.Json;
using ThestralServiceBridge.Domain.GovCarpeta.Dtos;

namespace ThestralServiceBridge.Infrastructure.GovCarpeta;

public class OperatorsHttpClient(IHttpClientFactory httpClientFactory) : IOperatorsHttpClient
{
    public async Task<OperatorDto[]?> ExecuteAsync()
    {
        var client = httpClientFactory.CreateClient("GovCarpeta");
        var request = new HttpRequestMessage(HttpMethod.Get, "/apis/getOperators");
        var response = await client.SendAsync(request);
        var responseContent = await response.Content.ReadAsStringAsync();
        var operators = JsonSerializer.Deserialize<OperatorDto[]>(responseContent);
        if (operators != null)
            foreach (var op in operators)
                if (op.TransferAPIURL != null)
                    op.TransferAPIURL = op.TransferAPIURL.Replace(" ", "");

        return operators;
    }
}