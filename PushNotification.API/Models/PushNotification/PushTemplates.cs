namespace PushNotification.API.Models.PushNotification;

public class PushTemplates
{
    public class QuickNotificationWithData
    {
        public const string Android = "{ \"message\" : { \"data\" : { \"title\" : \"$(titlePlaceholder)\", \"body\" : \"$(messagePlaceholder)\", \"url\" : \"$(urlPlaceholder)\" } } }";
    }

    // TODO: to fix format
    public class DataOnly
    {
        public const string Android = "{ \"message\" : { \"data\" : { \"title\" : \"$(titlePlaceholder)\", \"body\" : \"$(messagePlaceholder)\", \"url\" : \"$(urlPlaceholder)\" } } }";
    }
}
