namespace KickStat;

public static class AppSecurityInfo
{
    public const string USER_ADMINISTRATOR = "Admin";

    public const string ROLE_ADMINISTRATORS = "Administrators";
    public const string ROLE_USERS = "Users";

    public const string ROLE_ALL = ROLE_ADMINISTRATORS + "," + ROLE_USERS;

    public static readonly string[] AllRoles = { ROLE_ADMINISTRATORS, ROLE_USERS };
}