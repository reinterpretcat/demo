namespace Assets.Scripts.Console.Watchers
{
    /// <summary>
    ///
    /// </summary>
    public class WatchVar<T> : WatchVarBase
    {
        public T Value
        {
            get { return (T)_value; }
            set { _value = value; }
        }

        public WatchVar(string name)
            : base(name)
        {

        }

        public WatchVar(string name, T val)
            : base(name, val)
        {

        }
    }
}