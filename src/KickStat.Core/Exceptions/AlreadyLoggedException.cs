namespace KickStat;

public class AlreadyLoggedException : ApplicationException, IAlreadyLoggedException
{
    public string? LoggedMessage { get; set; }

    public object? ErrorObject { get; set; }


    public AlreadyLoggedException()
    {
    }

    public AlreadyLoggedException(string message) : base(message)
    {
    }

    public AlreadyLoggedException(string message, Exception inner) : base(message, inner)
    {
    }
}