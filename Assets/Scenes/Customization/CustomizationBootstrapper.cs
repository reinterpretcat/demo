using ActionStreetMap.Explorer.Bootstrappers;

namespace Assets.Scenes.Customization
{
    public class CustomizationBootstrapper: BootstrapperPlugin
    {
        public override string Name { get { return "customization"; } }

        public override bool Run()
        {
            // Register model extensions. Name should match with mapCSS 
            // rule builders/behaviours declaration.
            ExtensionProvider
                .RegisterBuilder("waypoint", typeof (WaypointModelBuilder))
                .RegisterBehaviour("hide", typeof(HideModelBehaviour));

            return true;
        }
    }
}
