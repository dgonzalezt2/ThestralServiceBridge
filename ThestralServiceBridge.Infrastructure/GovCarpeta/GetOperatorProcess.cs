using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ThestralServiceBridge.Domain.GovCarpeta.Dtos;
using ThestralServiceBridge.Infrastructure.Cache;

namespace ThestralServiceBridge.Infrastructure.GovCarpeta;

public class GetOperatorProcess(
    IOperatorsHttpClient operatorsHttpClient,
    ICacheStore cacheStore,
    ILogger<GetOperatorProcess> logger,
    IConfiguration configuration
) : IGetOperatorProcess
{
    private readonly string _defaultOperatorId =
        configuration.GetValue<string>("DefaultOperatorId") ?? "681bf6787cc39f0015383651";

    public async Task<OperatorDto?> GetOperatorAsync(string operatorId)
    {
        var operators = await GetOperatorsFromGovCarpetaAsync();
        return operators?.SingleOrDefault(op => op.OperatorId == operatorId);
    }

    public async Task<OperatorDto[]?> GetOperatorsAsync()
    {
        var operators = await GetOperatorsFromGovCarpetaAsync();
        return operators;
    }

    private async Task<OperatorDto[]?> GetOperatorsFromGovCarpetaAsync()
    {
        var operators = await TryGetOperatorsFromCacheAsync();

        if (operators == null || operators.Length == 0)
        {
            operators = await FetchOperatorsAsync();
            if (operators is { Length: > 0 }) await SaveOperatorsToCacheAsync(operators);
        }

        operators = operators?
            .Where(op => op.TransferAPIURL is not null)
            .Where(op => !op.OperatorId.Equals(_defaultOperatorId, StringComparison.InvariantCultureIgnoreCase))
            .ToArray();

        return operators ?? [];
    }

    private async Task<OperatorDto[]?> TryGetOperatorsFromCacheAsync()
    {
        try
        {
            return await cacheStore.GetAsync<OperatorDto[]>("transferOperators", CancellationToken.None);
        }
        catch (Exception ex)
        {
            logger.LogWarning("Error retrieving operators from cache: {Message}", ex.Message);
            return null;
        }
    }

    private async Task SaveOperatorsToCacheAsync(OperatorDto[] operators)
    {
        try
        {
            await cacheStore.SaveAsync("transferOperators", operators, CancellationToken.None, TimeSpan.FromHours(10));
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error saving operators to cache: {message}", ex.Message);
        }
    }

    private async Task<OperatorDto[]?> FetchOperatorsAsync()
    {
        return await operatorsHttpClient.ExecuteAsync();
    }
}