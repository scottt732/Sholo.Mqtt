using System;
using Microsoft.Extensions.Logging;

namespace Sholo.Mqtt.Old.Test.Helpers
{
    public class LogEntry
    {
        public LogLevel LogLevel { get; }
        public EventId EventId { get; }
        public string Message { get; }
        public Exception Exception { get; }
        public object[] Scopes { get; }

        public LogEntry(LogLevel logLevel, EventId eventId, string message, Exception exception, object[] scopes)
        {
            LogLevel = logLevel;
            EventId = eventId;
            Message = message;
            Exception = exception;
            Scopes = scopes;
        }
    }
}
