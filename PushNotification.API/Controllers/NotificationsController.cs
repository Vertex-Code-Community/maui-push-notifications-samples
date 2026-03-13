using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using PushNotification.API.Models;
using PushNotification.API.Models.PushNotification;
using PushNotification.API.Services.Interfaces;

namespace PushNotification.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationHubService _notificationHubService;
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationHubService notificationHubService,
        INotificationService notificationService)
    {
        _notificationHubService = notificationHubService;
        _notificationService = notificationService;
    }

    [HttpPut]
    [Route("installations")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
    public async Task<IActionResult> UpdateInstallation(
        [Required] DeviceInstallation deviceInstallation)
    {
        var success = await _notificationHubService
            .CreateOrUpdateInstallationAsync(deviceInstallation, HttpContext.RequestAborted);

        if (!success)
            return new UnprocessableEntityResult(); 

        return new OkResult();
    }

    [HttpDelete()]
    [Route("installations/{installationId}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
    public async Task<ActionResult> DeleteInstallation([Required][FromRoute] string installationId)
    {
        // Probably want to ensure deletion even if the connection is broken
        var success = await _notificationHubService
            .DeleteInstallationByIdAsync(installationId, CancellationToken.None);

        if (!success)
            return new UnprocessableEntityResult();

        return new OkResult();
    }
    
    [HttpGet]
    [Route("send")]
    public async Task<ActionResult> Send()
    {
        // Probably want to ensure deletion even if the connection is broken
        var success = await _notificationService
            .RequestSendNotification(new NotificationModel()
            {
                Title = "Test",
                Message = "Test notification",
                Tags = new()
            });

        return new OkResult();
    }
}

