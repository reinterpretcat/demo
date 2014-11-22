using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Console.Commands
{
    public class CommandManager
    {
        private Dictionary<string, ICommand> _cmdTable = new Dictionary<string, ICommand>();

        public void Register(string commandString, ICommand command)
        {
            _cmdTable[commandString.ToLower()] = command;
        }

        public void Unregister(string commandString)
        {
            _cmdTable.Remove(commandString.ToLower());
        }

        public bool Contains(string command)
        {
            return _cmdTable.ContainsKey(command);
        }

        public ICommand this[string name] 
        {
            get
            {
                return _cmdTable[name];
            }
        }

        public IEnumerable<string> CommandNames
        {
            get
            {
                return _cmdTable.Keys;
            }
        }

        public void RegisterDefaults()
        {
            Register("/?", new Command("prints help", CmdHelp));
            Register("sys", new SysCommand());
        }

        private string CmdHelp(params string[] args)
        {
            var output = new StringBuilder();
            output.AppendLine(":: Command List ::");

            foreach (string name in CommandNames)
            {
                var command = _cmdTable[name];
                output.AppendFormat("{0}: {1}\n", name, command.Description);
            }

            output.AppendLine(" ");
            return output.ToString();
        } 
    }
}
