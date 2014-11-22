using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Assets.Scripts.Console.Utils;

namespace Assets.Scripts.Console.Grep
{
    class ConsoleGrep
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

        //Search Function
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
                    if (IgnoreCase == true)
                        mtch = Regex.Match(text, RegEx, RegexOptions.IgnoreCase);
                    else
                        mtch = Regex.Match(text, RegEx);
                    if (mtch.Success == true)
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

            if (CountLines == true)
                strResults += "  " + iCount + " Lines Matched\r\n";
            strResults += "\r\n";

            _response.AppendLine(bEmpty == true ? "No matches found!" : strResults);
        }

        //Print Help
        public void PrintHelp()
        {
            _response.AppendLine("Usage: grep [/h|/H]");
            _response.AppendLine("       grep [/c] [/i] [/n] /e:reg_exp");
        }
    }
}
