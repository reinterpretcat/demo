using System;
using System.Text;
using Assets.Scripts.Console.Utils;
using ActionStreetMap.Infrastructure.Diagnostic;
using UnityEngine;

namespace Assets.Scripts.Console
{
    public class DebugConsoleTrace: DefaultTrace
    {
        private DebugConsole _console;

        public void SetConsole(DebugConsole console)
        {
            if (_console != null)
               GameObject.Destroy(_console);

            _console = console;
        }

        protected override void WriteRecord(RecordType type, string category, string message, Exception exception)
        {
            var logMessage = ToLogMessage(type, category, message, exception);
            _console.LogMessage(logMessage);
            switch (type)
            {
                 case RecordType.Error:
                    UnityEngine.Debug.LogError(logMessage.Text);
                    break;
                 case RecordType.Warn:
                    UnityEngine.Debug.LogWarning(logMessage.Text);
                    break;
                default:
                    UnityEngine.Debug.Log(logMessage.Text);
                    break;
            }
        }

        private ConsoleMessage ToLogMessage(RecordType type, string category, string text, Exception exception)
        {
            switch (type)
            {
                case RecordType.Error:
                    return ConsoleMessage.Error(String.Format("[{0}] {1}:{2}. Exception: {3}", type, category, text, exception));
                case RecordType.Warn:
                    return ConsoleMessage.Warning(String.Format("[{0}] {1}:{2}", type, category, text));
                case RecordType.Info:
                    var lines = text.Trim('\n').Split('\n');
                    var output = new StringBuilder();
                    output.Append(String.Format("[{0}] {1}:", type, category));
                    for(int i=0; i < lines.Length; i++)
                        output.AppendFormat("{0}{1}", lines[i], i != lines.Length - 1 ? "\n" : "");
                    return ConsoleMessage.Info(output.ToString());
                default:
                    return ConsoleMessage.Debug(String.Format("[{0}] {1}: {2}", type, category, text));
            }
        }
    }
}
