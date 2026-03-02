using System.ComponentModel.DataAnnotations;

namespace PushNotification.API.Models.PushNotification;

public class DeviceInstallation
{
    [Required]
    public string InstallationId { get; set; }

    [Required]
    public string Platform { get; set; }

    [Required]
    public string PushChannel { get; set; }

    public IList<string> Tags { get; set; } = Array.Empty<string>();
}

