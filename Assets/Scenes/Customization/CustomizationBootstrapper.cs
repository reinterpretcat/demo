using ActionStreetMap.Core;
using ActionStreetMap.Explorer.Bootstrappers;
using ActionStreetMap.Infrastructure.Dependencies;
using Assets.Scripts.Demo;

namespace Assets.Scenes.Customization
{
    public class CustomizationBootstrapper: BootstrapperPlugin
    {
        public override string Name { get { return "customization"; } }

        public override bool Run()
        {
            // Replace default elevation provider with custom one
            Container.Register(Component.For<IElevationProvider>().Use<FlatElevationProvider>().Singleton());

            // Register model extensions. Name should match with mapCSS 
            // rule builders/behaviours declaration.
            CustomizationService
                .RegisterAtlas("main", TextureAtlasHelper.GeTextureAtlas())
                .RegisterBuilder("waypoint", typeof (WaypointModelBuilder))
                .RegisterBehaviour("hide", typeof(HideModelBehaviour));

            return true;
        }
    }
}
