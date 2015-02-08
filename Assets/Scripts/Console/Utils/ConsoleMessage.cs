using UnityEngine;

using RecordType = ActionStreetMap.Infrastructure.Diagnostic.DefaultTrace.RecordType;

namespace Assets.Scripts.Console.Utils
{
    public class ConsoleMessage
    {
        public string Text { get; private set; }
        public RecordType Type { get; private set; }
        public Color Color { get; private set; }


        public ConsoleMessage(string text, RecordType type, Color color)
        {
            Text = text;
            Type = type;
            Color = color;
        }

        private static Color DebugColor = Color.white;
        private static Color WarningColor = Color.yellow;
        private static Color ErrorColor = Color.red;
        private static Color InfoColor = Color.cyan;

        public static ConsoleMessage Debug(string message)
        {
            return new ConsoleMessage(message, RecordType.Debug, DebugColor);
        }

        public static ConsoleMessage Info(string message)
        {
            return new ConsoleMessage(message, RecordType.Info, InfoColor);
        }

        public static ConsoleMessage Warning(string message)
        {
            return new ConsoleMessage(message, RecordType.Warn,  WarningColor);
        }

        public static ConsoleMessage Error(string message)
        {
            return new ConsoleMessage(message, RecordType.Error,  ErrorColor);
        }
    }
}
