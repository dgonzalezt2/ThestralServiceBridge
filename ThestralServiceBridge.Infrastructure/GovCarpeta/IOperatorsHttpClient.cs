using ThestralServiceBridge.Domain.GovCarpeta.Dtos;

namespace ThestralServiceBridge.Infrastructure.GovCarpeta;

public interface IOperatorsHttpClient
{
    Task<OperatorDto[]?> ExecuteAsync();
}