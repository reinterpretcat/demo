using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ActionStreetMap.Explorer.Commands;
using ActionStreetMap.Infrastructure.Reactive;
using ActionStreetMap.Infrastructure.Utilities;
using Assets.Scripts.Console.Utils;

namespace Assets.Scripts.Console.Commands
{
    /// <summary>
    ///     Simple grep implementation. Ported from http://www.codeproject.com/Articles/1485/A-C-Grep-Application.
    /// </summary>
    public class GrepCommand : ICommand
    {
        /// <inheritdoc />
        public string Name
        {
            get { return "grep"; }
        }

        /// <inheritdoc />
        public string Description
        {
            get { return "simple grep implementation"; }
        }

        /// <summary>
        ///     Messages to search.
        /// </summary>
        public List<ConsoleMessage> Messages { get; set; }

        /// <summary>
        ///     Creates instance of <see cref="GrepCommand" />
        /// </summary>
        /// <param name="messages">Debug console messages.</param>
        public GrepCommand(List<ConsoleMessage> messages)
        {
            Messages = messages;
        }

        /// <inheritdoc />
        public IObservable<string> Execute(params string[] args)
        {
            return Observable.Create<string>(o =>
            {
                var response = new StringBuilder();
                var grep = new ConsoleGrep(response);
                var commandLine = new Arguments(args);
                if (commandLine["h"] != null || commandLine["H"] != null)
                {
                    grep.PrintHelp();
                    o.OnNext(response.ToString());
                    o.OnCompleted();
                    return Disposable.Empty;
                }
                // The arguments /e and /f are mandatory
                if (commandLine["e"] != null)
                    grep.RegEx = commandLine["e"];
                else
                {
                    response.AppendLine("Error: No Regular Expression specified!");
                    response.AppendLine();
                    grep.PrintHelp();
                    o.OnNext(response.ToString());
                    o.OnCompleted();
                    return Disposable.Empty;
                }

                grep.IgnoreCase = (commandLine["i"] != null);
                grep.LineNumbers = (commandLine["n"] != null);
                grep.CountLines = (commandLine["c"] != null);

                // Do the search
                grep.Search(Messages.Take(Messages.Count - 1).ToList());

                o.OnNext(response.ToString());
                o.OnCompleted();
                return Disposable.Empty;
            });
        }

        #region Nested classes

        private class ConsoleGrep
        {
            private readonly StringBuilder _response;

            public bool IgnoreCase { get; set; }
            public bool LineNumbers { get; set; }
            public bool CountLines { get; set; }
            public string RegEx { get; set; }

            public ConsoleGrep(StringBuilder response)
            {
                _response = response;
            }

            /// <summary>
            ///     Search Function.
            /// </summary>
            /// <param name="log">Input content.</param>
            public void Search(List<ConsoleMessage> log)
            {
                String strResults = "Grep Results:\r\n\r\n";
                int iLine = 0, iCount = 0;
                bool bEmpty = true;

                var enumerator = log.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var strLine = enumerator.Current.Text;
                    var lines = strLine.Split('\n');
                    foreach (var text in lines)
                    {
                        iLine++;
                        //Using Regular Expressions as a real Grep
                        Match mtch;
                        if (IgnoreCase)
                            mtch = Regex.Match(text, RegEx, RegexOptions.IgnoreCase);
                        else
                            mtch = Regex.Match(text, RegEx);
                        if (mtch.Success)
                        {
                            bEmpty = false;
                            iCount++;
                            //Add the Line to Results string
                            if (LineNumbers)
                                strResults += "  " + iLine + ": " + text + "\r\n";
                            else
                                strResults += "  " + text + "\r\n";
                        }
                    }
                }

                if (CountLines)
                    strResults += "  " + iCount + " Lines Matched\r\n";
                strResults += "\r\n";

                _response.AppendLine(bEmpty ? "No matches found!" : strResults);
            }

            //Print Help
            public void PrintHelp()
            {
                _response.AppendLine("Usage: grep [/h|/H]");
                _response.AppendLine("       grep [/c] [/i] [/n] /e:reg_exp");
            }
        }

        #endregion
    }
}
