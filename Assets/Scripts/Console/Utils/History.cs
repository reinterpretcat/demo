using System.Collections.Generic;

namespace Assets.Scripts.Console.Utils
{
    class History
    {
        List<string> history = new List<string>();
        int index = 0;

        public void Add(string item)
        {
            history.Add(item);
            index = 0;
        }

        string current;

        public string Fetch(string current, bool next)
        {
            if (index == 0)
            {
                this.current = current;
            }

            if (history.Count == 0)
            {
                return current;
            }

            index += next ? -1 : 1;

            if (history.Count + index < 0 || history.Count + index > history.Count - 1)
            {
                index = 0;
                return this.current;
            }

            var result = history[history.Count + index];

            return result;
        }
    }
}