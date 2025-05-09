using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ThestralServiceBridge.Domain.Transfer;
using ThestralServiceBridge.Domain.Transfer.Dtos;
using ThestralServiceBridge.Infrastructure.MessageBroker;
using ThestralServiceBridge.Infrastructure.MessageBroker.Options;

namespace ThestralServiceBridge.Controllers;

[ApiController]
[Route("[controller]")]
public class TransferController(IMessagePublisher messagePublisher, IOptions<PublisherConfiguration> exchangeOptions)
    : ControllerBase
{
    private readonly PublisherConfiguration _exchangeOptions = exchangeOptions.Value;

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> TransferRequest([FromBody] TransferDto transferRequestDto,
        [FromHeader(Name = "X-User-Id")] string userId)
    {
        /*
        if (string.IsNullOrEmpty(userInfoHeader))
        {
            return BadRequest("User Id not found");
        }
         */
        if (string.IsNullOrEmpty(userId)) return BadRequest("User Id not found");
        if (!uint.TryParse(userId, out _)) return BadRequest("User Id not valid");
        var headers = new Headers(nameof(EventTypes.TRANSFER_USER), userId);
        await messagePublisher.SendMessageAsync(transferRequestDto, _exchangeOptions.UserTransferRequestQueue,
            headers.GetAttributesAsDictionary());

        return Ok();
    }

    [HttpPost("confirm")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> TransferConfirm([FromBody] TransferConfirmDto transferRequestDto)
    {
        var headers = new Headers(nameof(EventTypes.TRANSFER_USER_COMPLETED_FROM_EXTERNAL), transferRequestDto.Id.ToString());
        await messagePublisher.SendMessageAsync(transferRequestDto, _exchangeOptions.UserTransferRequestQueue,
            headers.GetAttributesAsDictionary());
        return Ok(transferRequestDto);
    }
    
    [HttpPost("newUser")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UserReceived([FromBody] TransferReceivedPackageDto transferReceived)
    {
        var headers = new Headers(nameof(EventTypes.NEW_TRANSFER_USER_FROM_EXTERNAL), transferReceived.Id.ToString());
        await messagePublisher.SendMessageAsync(transferReceived, _exchangeOptions.UserTransferRequestQueue,
            headers.GetAttributesAsDictionary());
        return Ok();
    }
}