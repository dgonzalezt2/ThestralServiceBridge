using Microsoft.AspNetCore.Mvc;
using ThestralServiceBridge.Domain.GovCarpeta.Dtos;
using ThestralServiceBridge.Infrastructure.GovCarpeta;

namespace ThestralServiceBridge.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OperatorController(IGetOperatorProcess getOperatorProcess) : ControllerBase
{
    [HttpGet("{operatorId}")]
    [ProducesResponseType(typeof(OperatorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OperatorDto>> GetOperator(string operatorId)
    {
        var requestedOperator = await getOperatorProcess.GetOperatorAsync(operatorId);
        if (requestedOperator == null) return NotFound();
        return Ok(requestedOperator);
    }

    [HttpGet]
    [ProducesResponseType(typeof(OperatorDto[]), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OperatorDto[]>> GetOperators()
    {
        var operators = await getOperatorProcess.GetOperatorsAsync();
        if (operators == null || operators.Length == 0) return NotFound();
        Response.Headers.Append("X-Total-Count", operators.Length.ToString());
        return Ok(operators);
    }
}