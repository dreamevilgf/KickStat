using Microsoft.Extensions.Logging;


namespace FarPlan.Logging;

public class EfCoreLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) => new EfCoreLogger();

    public void Dispose() => throw new NotImplementedException();

    private class EfCoreLogger : ILogger
    {
#pragma warning disable CS8633
        public IDisposable BeginScope<TState>(TState state) => default!;
#pragma warning restore CS8633

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            File.AppendAllText("logs/efCoreLog.txt", formatter(state, exception) + "\n-----------------\n");
            Console.WriteLine(formatter(state, exception));
        }
    }
}