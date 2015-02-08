using System;
using ActionStreetMap.Core;
using ActionStreetMap.Core.Tiling;
using ActionStreetMap.Infrastructure.Dependencies;
using ActionStreetMap.Infrastructure.Diagnostic;
using Assets.Scripts.Demo;
using Assets.Scripts.Map;
using ActionStreetMap.Explorer.Interactions;
using ActionStreetMap.Infrastructure.Bootstrap;

namespace Assets.Scripts.Character
{
    /// <summary> Adds some specific services to default bootstrapper pipeline. </summary>
    public class DemoBootstrapper: BootstrapperPlugin
    {
        private IMessageBus _messageBus;
        private readonly ITrace _trace;
        private CompositeModelBehaviour _solidModelBehavior;
        private CompositeModelBehaviour _waterModelBehavior;
        private DemoTileListener _messageListener;

        public override string Name { get { return "demo"; } }

        [Dependency]
        public DemoBootstrapper(IMessageBus messageBus, ITrace trace)
        {
            // NOTE inject these types through constructor instead of calling resolve method
            _messageBus = messageBus;
            _trace = trace;
        }

        public override bool Run()
        {
            // NOTE we should keep reference to prevent GC as RegisterInstance uses WeakReference
            // TODO add ability to register object without this trick
            _solidModelBehavior = new CompositeModelBehaviour("solid", new [] 
                { typeof (DestroyableObject), typeof (LocationInfoHolder) });

            Container.RegisterInstance<IModelBehaviour>(_solidModelBehavior, "solid");

            _waterModelBehavior = new CompositeModelBehaviour("water", new [] { typeof(WaterSimple) });

            Container.RegisterInstance<IModelBehaviour>(_waterModelBehavior, "water");

            // this class will listen messages about tile processing from ASM engine
            _messageListener = new DemoTileListener(_messageBus, _trace);

            return true;
        }
    }
}
