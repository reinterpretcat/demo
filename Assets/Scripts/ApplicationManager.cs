using System;
using System.Collections.Generic;
using ActionStreetMap.Core;
using ActionStreetMap.Core.Geometry;
using ActionStreetMap.Core.Tiling;
using ActionStreetMap.Explorer;
using ActionStreetMap.Explorer.Infrastructure;
using ActionStreetMap.Infrastructure.Dependencies;
using ActionStreetMap.Infrastructure.Diagnostic;
using ActionStreetMap.Infrastructure.IO;
using ActionStreetMap.Infrastructure.Reactive;
using ActionStreetMap.Unity.IO;
using Assets.Scripts.Console;
using Assets.Scripts.Demo;
using UnityEngine;
using Component = ActionStreetMap.Infrastructure.Dependencies.Component;
using RenderMode = ActionStreetMap.Core.RenderMode;

namespace Assets.Scripts
{
    /// <summary>
    ///     Maintains application flow, provides service location logic.
    ///     This class should be only one singleton in application.
    /// </summary>
    public class ApplicationManager
    {
        private const string FatalCategoryName = "Fatal";

        private IContainer _container;
        private IMessageBus _messageBus;
        private DebugConsoleTrace _trace;
        private GameRunner _gameRunner;

        private ITileController _tileController;
        private IPositionObserver<Vector2d> _positionObserver;

        #region Singleton implementation

        private ApplicationManager()
        {
            InitializeFramework();

            Coordinate = new GeoCoordinate(52.52090, 13.40793);
            //Coordinate = new GeoCoordinate(55.75282, 37.62259);
        }

        public static ApplicationManager Instance { get { return Nested.__instance; } }

        private class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested() { }

            internal static readonly ApplicationManager __instance = new ApplicationManager();
        }

        #endregion

        #region Initialization logic

        private void InitializeFramework()
        {
            // Setup main thread scheduler
            Scheduler.MainThread = UnityMainThreadScheduler.MainThread;

            // Create and register DebugConsole inside Container
            _container = new Container();

            // Create message bus class which is way to listen for ASM events
            _messageBus = new MessageBus();

            // Create trace to log important messages
            _trace = new DebugConsoleTrace();

            // Subscribe to unhandled exceptions in RX
            UnityMainThreadDispatcher.RegisterUnhandledExceptionCallback(ex =>
                _trace.Error(FatalCategoryName, ex, "Unhandled exception"));

            // Console is way to debug/investigate app behavior on real devices when
            // regular debugger is not applicable
            CreateConsole(false);

            try
            {
                // NOTE These services should be registered inside container before GameRunner is constructed.
                // Trace implementation
                _container.RegisterInstance<ITrace>(_trace);
                // Path resolver which knows about current platform
                _container.RegisterInstance<IPathResolver>(new PathResolver());
                // Message bus
                _container.RegisterInstance(_messageBus);
                // File system service
                _container.Register(Component.For<IFileSystemService>()
#if UNITY_WEBPLAYER
                    .Use<WebFileSystemService>().Singleton());
#else
                    .Use<FileSystemService>().Singleton());
#endif
                // Build config with default settings
                var config = ConfigBuilder.GetDefault()
#if UNITY_WEBPLAYER
                    .SetSandbox(true)
#endif
                    .Build();

                // Create ASM entry point with settings provided, register custom plugin which adds
                // custom logic or replaces default one. Then run bootstrapping process which populates container
                // with defined implementations.
                _gameRunner = new GameRunner(_container, config)
                    .RegisterPlugin<DemoBootstrapper>("demo", _messageBus, _trace)
                    .Bootstrap();

            }
            catch (Exception ex)
            {
                _trace.Error(FatalCategoryName, ex, "Cannot initialize ASM framework");
                throw;
            }
        }

        /// <summary> Creates debug console in scene. </summary>
        public void CreateConsole(bool isOpen = true)
        {
            // NOTE DebugConsole is based on some adapted solution found in Internet
            var consoleGameObject = new GameObject("_DebugConsole_");
            var console = consoleGameObject.AddComponent<DebugConsole>();
            _trace.SetConsole(console);
            // that is not nice, but we need to use commands registered in DI with their dependencies
            console.SetContainer(_container);
            console.IsOpen = isOpen;
        }

        #endregion

        #region Service locator

        /// <summary> Gets service of T from container. </summary>
        public T GetService<T>()
        {
            return _container.Resolve<T>();
        }

        /// <summary> Gets services of T from container. sS</summary>
        public IEnumerable<T> GetServices<T>()
        {
            return _container.ResolveAll<T>();
        }

        #endregion

        #region Public members

        public GeoCoordinate Coordinate { get; set; }

        public bool IsInitialized { get; private set; }

        public void RunGame()
        {
            try
            {
                _tileController = GetService<ITileController>();
                _positionObserver = _tileController;
                _gameRunner.RunGame(Coordinate);
                IsInitialized = true;
            }
            catch (Exception ex)
            {
                _trace.Error("FATAL", ex, "Error running game:");
                throw;
            }
        }

        public void Move(Vector2d point)
        {
            Scheduler.ThreadPool.Schedule(() => _positionObserver.OnNext(point));
        }

        public void SwitchMode(RenderMode renderMode, Rectangle2d rectange)
        {
            _tileController.Mode = renderMode;
            _tileController.Viewport = rectange;
        }

        #endregion
    }
}
