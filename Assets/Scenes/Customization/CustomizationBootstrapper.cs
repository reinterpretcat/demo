using ActionStreetMap.Explorer.Bootstrappers;
using Assets.Scripts.Demo;

namespace Assets.Scenes.Customization
{
    public class CustomizationBootstrapper: BootstrapperPlugin
    {
        public override string Name { get { return "customization"; } }

        public override bool Run()
        {
            // Register model extensions. Name should match with mapCSS 
            // rule builders/behaviours declaration.
            CustomizationService
                .RegisterBuilder("waypoint", typeof (WaypointModelBuilder))
                .RegisterBehaviour("hide", typeof(HideModelBehaviour));

            return true;
        }
    }
}
