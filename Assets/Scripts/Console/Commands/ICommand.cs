using System;

namespace Assets.Scripts.Console.Commands
{
    public interface ICommand
    {
        string Description { get; }
        string Execute(params string[] args);
    }

    public class Command : ICommand
    {
        private readonly Func<string[], string> _functor;
        public Command(string description, Func<string[], string> functor)
        {
            Description = description;
            _functor = functor;
        }

        public string Description { get; private set; }

        public string Execute(params string[] args)
        {
            return _functor.Invoke(args);
        }
    }
}
