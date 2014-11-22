using System.Collections.Generic;

namespace Assets.Scripts.Console.Watchers
{
    public class VarWatcher
    {
        Dictionary<string, WatchVarBase> _watchVarTable = new Dictionary<string, WatchVarBase>();

        public void AddWatchVarToTable(WatchVarBase watchVar)
        {
            _watchVarTable[watchVar.Name] = watchVar;
        }

        public void RemoveWatchVarFromTable(string name)
        {
            _watchVarTable.Remove(name);
        }

        public IEnumerable<WatchVarBase> GetVariables
        {
            get
            {
                return _watchVarTable.Values;
            }
        }
    }
}
