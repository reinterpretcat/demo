using ActionStreetMap.Infrastructure.Diagnostic;
using UnityEngine;

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

        private static Color DefaultColor = Color.white;
        private static Color WarningColor = Color.yellow;
        private static Color ErrorColor = Color.red;
        private static Color SystemColor = Color.green;
        private static Color InputColor = Color.green;
        private static Color OutputColor = Color.cyan;

        public static ConsoleMessage Normal(string message)
        {
            return new ConsoleMessage(message, RecordType.Normal, DefaultColor);
        }

        public static ConsoleMessage System(string message)
        {
            return new ConsoleMessage(message, RecordType.System, SystemColor);
        }

        public static ConsoleMessage Warning(string message)
        {
            return new ConsoleMessage(message, RecordType.Warning,  WarningColor);
        }

        public static ConsoleMessage Error(string message)
        {
            return new ConsoleMessage(message, RecordType.Error,  ErrorColor);
        }

        public static ConsoleMessage Output(string message)
        {
            return new ConsoleMessage(message, RecordType.Output, OutputColor);
        }

        public static ConsoleMessage Input(string message)
        {
            return new ConsoleMessage(message, RecordType.Input,  InputColor);
        }
    }
}
