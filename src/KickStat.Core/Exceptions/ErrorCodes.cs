namespace KickStat;

public class ErrorCodes
{
    public const int NoError = 0;

    public const int AppConfig = 1;

    public const int NoFarPlanDbConnect = 1100;

    public const int StartJobEpgServiceUpdate = 4000;
    public const int EpgUpdateJobDownloadImages = 4002;
    public const int UnhandledException = 5000;
}