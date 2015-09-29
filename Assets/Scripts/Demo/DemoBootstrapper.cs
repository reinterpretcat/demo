using ActionStreetMap.Core;
using ActionStreetMap.Explorer.Bootstrappers;
using ActionStreetMap.Explorer.Customization;
using ActionStreetMap.Infrastructure.Dependencies;
using ActionStreetMap.Infrastructure.Diagnostic;
using Assets.Scripts.MapEditor.Behaviors;

namespace Assets.Scripts.Demo
{
    /// <summary> Adds some specific services to default bootstrapper pipeline. </summary>
    public class DemoBootstrapper: BootstrapperPlugin
    {
        private IMessageBus _messageBus;
        private readonly ITrace _trace;
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
            // this class will listen messages about tile processing from ASM engine
            _messageListener = new DemoTileListener(_messageBus, _trace);

            // extensions
            CustomizationService
                 .RegisterBehaviour("terrain_draw", typeof(TerrainDrawBehaviour))
                 .RegisterAtlas("main", TextureAtlasHelper.GeTextureAtlas());

            return true;
        }
    }
}
