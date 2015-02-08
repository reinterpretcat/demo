using System;
using ActionStreetMap.Infrastructure.IO;
using ActionStreetMap.Infrastructure.Reactive;
using Assets.Scripts.Console;
using Assets.Scripts.Console.Utils;
using Assets.Scripts.Demo;
using ActionStreetMap.Core;
using ActionStreetMap.Explorer;
using ActionStreetMap.Infrastructure.Dependencies;
using ActionStreetMap.Infrastructure.Diagnostic;
using UnityEngine;

using RecordType = ActionStreetMap.Infrastructure.Diagnostic.DefaultTrace.RecordType;

namespace Assets.Scripts.Character
{
    public class ActionStreetMapBehavior : MonoBehaviour
    {
        public float Delta = 10;

        private IPositionObserver<MapPoint> _positionObserver;

        private ITrace _trace;

        private bool _isInitialized = false;

        private Vector3 _position = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        
        private DebugConsole _console;

        // Use this for initialization
        private void Start()
        {
            Initialize();
        }

        // Update is called once per frame
        private void Update()
        {
            if (_isInitialized && _position != transform.position)
            {
                _position = transform.position;
                Scheduler.ThreadPool.Schedule(() => 
                    _positionObserver.OnNext(new MapPoint(_position.x, _position.z, _position.y)));
            }
        }

        #region Initialization

        private void Initialize()
        {
            Scheduler.MainThread = new UnityMainThreadScheduler();
            // create and register DebugConsole inside Container
            var container = new Container();
            
            InitializeConsole(container);

            Scheduler.ThreadPool.Schedule(() =>
            {
                try
                {
                    IMessageBus messageBus = new MessageBus();
                    // these services should be registered inside container before GameRunner is constructed.
                    container.RegisterInstance(_trace);
                    container.RegisterInstance<IPathResolver>(new WinPathResolver());
                    container.RegisterInstance(messageBus);

                    var gameRunner = new GameRunner(container, @"Config/settings.json")
                        .RegisterPlugin<DemoBootstrapper>("demo", messageBus, _trace);
                    _positionObserver = gameRunner;
                    gameRunner.RunGame(new GeoCoordinate(52.52033, 13.38748));

                    _isInitialized = true;
                }
                catch (Exception ex)
                {
                    _console.LogMessage(new ConsoleMessage("Error running game:" + ex, RecordType.Error, Color.red));
                    throw;
                }
            });
        }

        private void InitializeConsole(IContainer container)
        {
            var consoleGameObject = new GameObject("_DebugConsole_");
            _console = consoleGameObject.AddComponent<DebugConsole>();
            container.RegisterInstance(_console);
            // that is not nice, but we need to use commands registered in DI with their dependencies
            _console.Container = container; 
            _trace = new DebugConsoleTrace(_console);
            _console.IsOpen = true;

            UnityMainThreadDispatcher.RegisterUnhandledExceptionCallback(ex => 
                _trace.Error("fatal", ex, "Unhandled exception"));
        }

        #endregion
    }
}
