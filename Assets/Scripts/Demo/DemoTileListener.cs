using System;
using System.Diagnostics;
using ActionStreetMap.Core;
using ActionStreetMap.Core.Tiling;
using ActionStreetMap.Core.Tiling.Models;
using ActionStreetMap.Infrastructure.Diagnostic;
using ActionStreetMap.Infrastructure.Reactive;
using Assets.Scripts.Map;
using UnityEngine;

namespace Assets.Scripts.Demo
{
    /// <summary> Listens for tile specific messages. </summary>
    public class DemoTileListener
    {
        private const string LogTag = "tile";

        private readonly ITrace _trace;

        private readonly Stopwatch _stopwatch = new Stopwatch();

        public DemoTileListener(IMessageBus messageBus, ITrace trace)
        {
            _trace = trace;

            messageBus.AsObservable<TileLoadStartMessage>().Do(m => OnTileBuildStarted(m.TileCenter)).Subscribe();
            messageBus.AsObservable<TileLoadFinishMessage>().Do(m => OnTileBuildFinished(m.Tile)).Subscribe();
            messageBus.AsObservable<TileDestroyMessage>().Do(m => OnTileDestroyed(m.Tile)).Subscribe();
        }

        private void OnTileDestroyed(Tile tile)
        {
            _trace.Debug(LogTag, "Tile destroyed: center:{0}", tile.MapCenter);
        }

        public void OnTileFound(Tile tile, MapPoint position)
        {
        }

        public void OnTileBuildStarted(MapPoint center)
        {
            _stopwatch.Start();
            _trace.Debug(LogTag, "Tile build begin: center:{0}", center);
        }

        public void OnTileBuildFinished(Tile tile)
        {
            _stopwatch.Stop();
            _trace.Debug(LogTag, "Tile of size {0}x{1} is loaded in {2} ms. Trigger GC.", tile.Width, tile.Height, _stopwatch.ElapsedMilliseconds);
            GC.Collect();
            _stopwatch.Reset();

            Scheduler.MainThread.Schedule(() =>
            {
                foreach (Transform child in tile.GameObject.GetComponent<GameObject>().transform)
                {
                    var name = child.gameObject.name;
                    if (name == "terrain")
                    {
                        foreach (Transform cell in child.gameObject.transform)
                            cell.gameObject.AddComponent<ModifyableTerrainBehaviour>();
                    }
                    else if (name.StartsWith("building"))
                    {
                        foreach (Transform buildingPart in child.gameObject.transform)
                            if (buildingPart.name == "facade")
                                buildingPart.gameObject.AddComponent<ModifyableFacadeBehaviour>();
                    }
                    else if(name.StartsWith("tree"))
                    {
                        child.gameObject.AddComponent<DestroyableMeshBehaviour>();
                    }
                }
            });
        }
    }
}
