namespace PushNotification.API.Constants;

public static class Platform
{
    public const string Android = "Android";
}

public static class AuthenticationType
{
    public const string GiveX = "GiveX";
    public const string AppCart = "AppCart";
    public const string NoLoyalty = "NoLoyalty";
}

public static class TypeOfSend
{
    public const string Now = "Now";
    public const string SpecificDate = "SpecificDate";
}
public static class VersionApplication
{
    public const string Latest = "latest";
    public const string Old = "old";
}

public static class TypeOfRedirection
{
    public const string None = "None";
    public const string WithUrl = "Url";
}

public static class RecipientType
{
    public const string All = "All";
    public const string Target = "Target";
}

public static class TargetUserType
{
    public const string All = "All";
    public const string OldVersionOwner = "OldVersionOwner";
    public const string SpecificUser = "SpecificUser";
    public const string SpecificStoreLocation = "SpecificStoreLocation";
}

public static class NotificationStatus
{
    public const string Sent = "Sent";
    public const string Scheduled = "Scheduled";
    public const string Canceled = "Canceled";
}