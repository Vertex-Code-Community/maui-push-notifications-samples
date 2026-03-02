using PushNotification.API.Models;

namespace PushNotification.API.Utilites;

public static class NotificationTagExpressionBuilder
{
    private static readonly Dictionary<NotificationTagsType, string> Prefix = new()
    {
        { NotificationTagsType.User, "user_shopper_id:" },
        { NotificationTagsType.VersionOwner, "version:" },
    };

    public static string? Build(NotificationModel model)
    {
        var andParts = new List<string>();

        foreach (var tagPair in model.Tags)
        {
            if (!Prefix.TryGetValue(tagPair.Key, out var prefix))
                continue;
            
            if (tagPair.Value is null || tagPair.Value.Count == 0)
                continue;

            var values = tagPair.Value
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => Prefix[tagPair.Key] + x.Trim())
                .Distinct()
                .ToList();

            if (values.Count == 0)
                continue;
            

            andParts.Add(values.Count == 1 ? values[0] : $"({string.Join(" || ", values)})");
        }

        if (andParts.Count == 0)
            return null;

        return string.Join(" && ", andParts);
    }
}