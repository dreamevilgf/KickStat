namespace KickStat;

public static class ProgressExtensions
{
    public static void ReportFormat(this IProgress<string> progress, string formatString, params object[] args)
    {
        progress.Report(string.Format(formatString, args));
    }
}