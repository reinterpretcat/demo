namespace Assets.Scripts.Console.Watchers
{
    /// <summary>
    /// Base class for WatchVars. Provides base functionality.
    /// </summary>
    public abstract class WatchVarBase
    {
        /// <summary>
        /// Name of the WatchVar.
        /// </summary>
        public string Name { get; private set; }

        protected object _value;

        public WatchVarBase(string name, object val)
            : this(name)
        {
            _value = val;
        }

        public WatchVarBase(string name)
        {
            Name = name;
        }

        public object ObjValue
        {
            get { return _value; }
        }

        public override string ToString()
        {
            if (_value == null)
            {
                return "<null>";
            }

            return _value.ToString();
        }
    }
}