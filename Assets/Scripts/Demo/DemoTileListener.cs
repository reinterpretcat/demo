using System;
using System.Diagnostics;
using System.Reactive.Linq;
using ActionStreetMap.Core;
using ActionStreetMap.Core.Scene;
using ActionStreetMap.Core.Scene.Models;
using ActionStreetMap.Infrastructure.Diagnostic;

namespace Assets.Scripts.Demo
{
    /// <summary>
    ///     Listens for tile specific messages
    /// </summary>
    public class DemoTileListener
    {
        private const string LogTag = "Tile";

        private readonly ITrace _trace;

        private readonly Stopwatch _stopwatch = new Stopwatch();

        public DemoTileListener(IMessageBus messageBus, ITrace trace)
        {
            _trace = trace;

            messageBus.AsObservable<TileLoadStartMessage>().Do(m => OnTileBuildStarted(m.TileCenter)).Subscribe();
            messageBus.AsObservable<TileLoadFinishMessage>().Do(m => OnTileBuildFinished(m.Tile)).Subscribe();
            messageBus.AsObservable<TileActivateMessage>().Do(m => OnTileActivated(m.Tile)).Subscribe();
            messageBus.AsObservable<TileDeactivateMessage>().Do(m => OnTileDeactivated(m.Tile)).Subscribe();
            messageBus.AsObservable<TileDestroyMessage>().Do(m => OnTileDestroyed(m.Tile)).Subscribe();
        }

        private void OnTileDestroyed(Tile tile)
        {
            _trace.Normal(LogTag, String.Format("Tile destroyed: center:{0}", tile.MapCenter));
        }

        public void OnTileFound(Tile tile, MapPoint position)
        {
        }

        public void OnTileBuildStarted(MapPoint center)
        {
            _stopwatch.Start();
            _trace.Normal(LogTag, String.Format("Tile build begin: center:{0}", center));
        }

        public void OnTileBuildFinished(Tile tile)
        {
            _stopwatch.Stop();
            _trace.Normal(LogTag, String.Format("Tile of size {0} is loaded in {1} ms. Trigger GC.", tile.Size, _stopwatch.ElapsedMilliseconds));
            GC.Collect();
            _stopwatch.Reset();
        }

        private void OnTileActivated(Tile tile)
        {
            _trace.Normal(LogTag, String.Format("Tile activated: center:{0}", tile.MapCenter));
        }

        private void OnTileDeactivated(Tile tile)
        {
            _trace.Normal(LogTag, String.Format("Tile deactivated: center:{0}", tile.MapCenter));
        }
    }
}
