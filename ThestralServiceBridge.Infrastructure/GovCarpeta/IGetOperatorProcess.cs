using ThestralServiceBridge.Domain.GovCarpeta.Dtos;

namespace ThestralServiceBridge.Infrastructure.GovCarpeta;

public interface IGetOperatorProcess
{
    Task<OperatorDto?> GetOperatorAsync(string operatorId);
    Task<OperatorDto[]?> GetOperatorsAsync();
}