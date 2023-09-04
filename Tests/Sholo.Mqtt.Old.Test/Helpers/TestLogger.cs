using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Sholo.Mqtt.Old.Test.Helpers
{
    public class TestLogger<T> : TestLogger, ILogger<T>
    {
    }

    public class TestLogger : ILogger
    {
        public LogEntry[] LogEntries => LogEntriesList.ToArray();

        private List<LogEntry> LogEntriesList { get; } = new List<LogEntry>();
        private Stack<object> Scopes { get; } = new Stack<object>();

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var message = formatter.Invoke(state, exception);
            LogEntriesList.Add(new LogEntry(logLevel, eventId, message, exception, Scopes.ToArray()));
        }

        public bool IsEnabled(LogLevel logLevel) => true;
        public IDisposable BeginScope<TState>(TState state) => new ScopeContainer(Scopes, state);

        private sealed class ScopeContainer : IDisposable
        {
            private Stack<object> States { get; }

            public ScopeContainer(Stack<object> states, object state)
            {
                States = states;
                States.Push(state);
            }

            public void Dispose()
            {
                States.Pop();
            }
        }
    }
}
