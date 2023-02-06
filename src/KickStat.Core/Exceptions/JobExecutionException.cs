using System.Runtime.Serialization;


namespace KickStat;

public class JobExecutionException : ApplicationException
{
    public JobExecutionException()
    {
    }

    public JobExecutionException(string message) : base(message)
    {
    }

    public JobExecutionException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected JobExecutionException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}