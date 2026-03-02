using System.Text;
using Foundation;

namespace PushNotification.MAUI;

internal static class NSDataExtensions
{
    internal static string? ToHexString(this NSData data)
    {
        var bytes = data.ToArray();
        if (bytes is null || bytes.Length == 0)return null;

        StringBuilder sb = new StringBuilder(bytes.Length * 2);

        foreach (byte b in bytes)
            sb.AppendFormat("{0:x2}", b);

        return sb.ToString().ToUpperInvariant();
    }
}