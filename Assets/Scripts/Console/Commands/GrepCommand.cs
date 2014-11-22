using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Console.Grep;
using Assets.Scripts.Console.Utils;

namespace Assets.Scripts.Console.Commands
{
    /// <summary>
    /// Grep implementation. Ported from http://www.codeproject.com/Articles/1485/A-C-Grep-Application
    /// </summary>
    public class GrepCommand: ICommand
    {
        private List<ConsoleMessage> _message;

        public string Description { get { return "simple grep implementation"; } }

        public GrepCommand(List<ConsoleMessage> message)
        {
            _message = message;
        }

        public string Execute(params string[] args)
        {
            var response = new StringBuilder();
            var grep = new ConsoleGrep(response);
            var commandLine = new Arguments(args);
            if (commandLine["h"] != null || commandLine["H"] != null)
            {
                grep.PrintHelp();
                return response.ToString();
            }
            // The arguments /e and /f are mandatory
            if (commandLine["e"] != null)
                grep.RegEx = (string)commandLine["e"];
            else
            {
                response.AppendLine("Error: No Regular Expression specified!");
                response.AppendLine();
                grep.PrintHelp();
                return response.ToString();
            }

            grep.IgnoreCase = (commandLine["i"] != null);
            grep.LineNumbers = (commandLine["n"] != null);
            grep.CountLines = (commandLine["c"] != null);
            // Do the search
            grep.Search(_message.Take(_message.Count-1).ToList());

            return response.ToString();
        }
    }
}
