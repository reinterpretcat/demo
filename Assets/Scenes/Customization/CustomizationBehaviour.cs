using System;
using ActionStreetMap.Core;
using ActionStreetMap.Core.Geometry;
using ActionStreetMap.Explorer;
using ActionStreetMap.Explorer.Infrastructure;
using ActionStreetMap.Infrastructure.Dependencies;
using ActionStreetMap.Infrastructure.Diagnostic;
using Assets.Scripts.Character;

namespace Assets.Scenes.Customization
{
    public class CustomizationBehaviour: ActionStreetMapBehaviour
    {
        protected override ConfigBuilder GetConfigBuilder()
        {
            // use customization mapcss file and render only one scene tile
            return base.GetConfigBuilder()
                .SetRenderOptions(RenderMode.Scene, new Rectangle2d(0, 0, TileSize, TileSize))
                .SetMapCss("Config/customization.mapcss");
        }

        protected override Action<IContainer, IMessageBus, ITrace, GameRunner> GetBootInitAction()
        {
            // use customization bootstrapper plugin
            return (container, messageBus, trace, gameRunner) =>
            {
                gameRunner.RegisterPlugin<CustomizationBootstrapper>("customization");
            };
        }
    }
}
