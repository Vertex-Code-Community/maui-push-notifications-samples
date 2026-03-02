namespace PushNotification.API.Models.PushNotification;

public class PushTemplates
{
    public class QuickNotificationWithData
    {
        public const string Android = "{ \"message\" : { \"data\" : { \"title\" : \"$(titlePlaceholder)\", \"body\" : \"$(messagePlaceholder)\", \"url\" : \"$(urlPlaceholder)\" } } }";
        public const string iOS = "{ \"aps\" : {\"alert\" : { \"title\" : \"$(titlePlaceholder)\", \"body\" : \"$(messagePlaceholder)\" } }, \"title\" : \"$(titlePlaceholder)\", \"body\" : \"$(messagePlaceholder)\", \"url\" : \"$(urlPlaceholder)\" }";
    }

    // TODO: to fix format
    public class DataOnly
    {
        public const string Android = "{ \"message\" : { \"data\" : { \"title\" : \"$(titlePlaceholder)\", \"body\" : \"$(messagePlaceholder)\", \"url\" : \"$(urlPlaceholder)\" } } }";
        public const string iOS = "{ \"aps\" : {\"content-available\" : 1, \"apns-priority\": 5, \"sound\" : \"\", \"badge\" : 0}, \"title\" : \"$(titlePlaceholder)\", \"body\" : \"$(messagePlaceholder)\", \"url\" : \"$(urlPlaceholder)\" }";
    }
}
