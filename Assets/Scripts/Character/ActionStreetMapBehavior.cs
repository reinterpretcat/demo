using System;
using Assets.Scripts.Console;
using Assets.Scripts.Console.Utils;
using Assets.Scripts.Demo;
using ActionStreetMap.Core;
using ActionStreetMap.Explorer;
using ActionStreetMap.Explorer.Bootstrappers;
using ActionStreetMap.Infrastructure.Bootstrap;
using ActionStreetMap.Infrastructure.Config;
using ActionStreetMap.Infrastructure.Dependencies;
using ActionStreetMap.Infrastructure.Diagnostic;
using ActionStreetMap.Infrastructure.IO;
using UnityEngine;
using Component = ActionStreetMap.Infrastructure.Dependencies.Component;

namespace Assets.Scripts.Character
{
    public class ActionStreetMapBehavior : MonoBehaviour
    {
        public float Delta = 10;

        private GameRunner _gameRunner;

        private DemoTileListener _messageListener;

        private ITrace _trace;

        private Vector2 _position2D;
        
        private DebugConsole _console;

        // Use this for initialization
        private void Start()
        {
            Initialize();
        }

        // Update is called once per frame
        private void Update()
        {
            // NOTE we do no want to call ASM logic on every small position change
            // However, Delta should be less than defined offset value in configuration
            if (Math.Abs(transform.position.x - _position2D.x) > Delta
                || Math.Abs(transform.position.z - _position2D.y) > Delta)
            {
                _position2D = new Vector2(transform.position.x, transform.position.z);
                _gameRunner.OnMapPositionChanged(new MapPoint(transform.position.x, transform.position.z));
            }
        }

        #region Initialization

        private void Initialize()
        {
            // create and register DebugConsole inside Container
            var container = new Container();
            var messageBus = new MessageBus();
            var pathResolver = new WinPathResolver();
            InitializeConsole(container);
            try
            {
                var fileSystemService = new FileSystemService(pathResolver);
                container.RegisterInstance(typeof(IPathResolver), pathResolver);
                container.RegisterInstance(typeof (IFileSystemService), fileSystemService);
                container.RegisterInstance<IConfigSection>(new ConfigSection(@"Config/settings.json", fileSystemService));

                // actual boot service
                container.Register(Component.For<IBootstrapperService>().Use<BootstrapperService>());

                // boot plugins
                container.Register(Component.For<IBootstrapperPlugin>().Use<InfrastructureBootstrapper>().Named("infrastructure"));
                container.Register(Component.For<IBootstrapperPlugin>().Use<OsmBootstrapper>().Named("osm"));
                container.Register(Component.For<IBootstrapperPlugin>().Use<TileBootstrapper>().Named("tile"));
                container.Register(Component.For<IBootstrapperPlugin>().Use<SceneBootstrapper>().Named("scene"));
                container.Register(Component.For<IBootstrapperPlugin>().Use<DemoBootstrapper>().Named("demo"));

                container.RegisterInstance(_trace);

                // this class will listen messages about tile processing from ASM engine
                _messageListener = new DemoTileListener(messageBus, _trace);

                // interception
                //container.AllowProxy = true;
                //container.AutoGenerateProxy = true;
                //container.AddGlobalBehavior(new TraceBehavior(_trace));

                _gameRunner = new GameRunner(container, messageBus);
                _gameRunner.RunGame();
            }
            catch (Exception ex)
            {
                _console.LogMessage(new ConsoleMessage("Error running game:" + ex.ToString(), RecordType.Error, Color.red));
                throw;
            }
        }

        private void InitializeConsole(IContainer container)
        {
            var consoleGameObject = new GameObject("_DebugConsole_");
            _console = consoleGameObject.AddComponent<DebugConsole>();
            container.RegisterInstance(_console);
            _trace = new DebugConsoleTrace(_console);

            //_console.CommandManager.Register("scene", new SceneCommand(container));
        }

        #endregion
    }
}
