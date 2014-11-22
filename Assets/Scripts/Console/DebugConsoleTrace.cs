using System;
using System.Text;
using Assets.Scripts.Console.Utils;
using ActionStreetMap.Infrastructure.Diagnostic;

namespace Assets.Scripts.Console
{
    public class DebugConsoleTrace: DefaultTrace
    {
        private readonly DebugConsole _console;

        public DebugConsoleTrace(DebugConsole console)
        {
            _console = console;
        }

        protected override void WriteRecord(RecordType type, string category, string message, Exception exception)
        {
            var logMessage = ToLogMessage(type, category, message, exception);
            _console.LogMessage(logMessage);
            UnityEngine.Debug.Log(logMessage.Text);
        }

        private ConsoleMessage ToLogMessage(RecordType type, string category, string text, Exception exception)
        {
            switch (type)
            {
                case RecordType.Error:
                    return ConsoleMessage.Error(String.Format("[{0}] {1}:{2}. Exception: {3}", type, category, text, exception));
                case RecordType.Warning:
                    return ConsoleMessage.Warning(String.Format("[{0}] {1}:{2}", type, category, text));
                case RecordType.Output:
                    var lines = text.Trim('\n').Split('\n');
                    var output = new StringBuilder();
                    foreach (var line in lines)
                    {
                        output.AppendFormat("= {0}\n", line);
                    }
                    return ConsoleMessage.Output(output.ToString());
                case RecordType.Input:
                    return ConsoleMessage.Input(String.Format(">>> {0}", text));
                case RecordType.System:
                    return ConsoleMessage.System(String.Format("# {0}", text));
                default:
                    return ConsoleMessage.Normal(String.Format("[{0}] {1}:{2}", type, category, text));
            }
        }
    }
}
