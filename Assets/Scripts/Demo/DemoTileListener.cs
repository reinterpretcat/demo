using System;
using System.Diagnostics;
using ActionStreetMap.Core;
using ActionStreetMap.Core.Geometry;
using ActionStreetMap.Core.Tiling;
using ActionStreetMap.Core.Tiling.Models;
using ActionStreetMap.Infrastructure.Diagnostic;
using ActionStreetMap.Infrastructure.Reactive;
using UnityEngine;

namespace Assets.Scripts.Demo
{
    /// <summary> Listens for tile specific messages. </summary>
    public class DemoTileListener
    {
        private const string LogTag = "tile";

        private readonly IMessageBus _messageBus;
        private readonly ITrace _trace;

        private readonly Stopwatch _stopwatch = new Stopwatch();

        public DemoTileListener(IMessageBus messageBus, ITrace trace)
        {
            _messageBus = messageBus;
            _trace = trace;

            _messageBus.AsObservable<TileLoadStartMessage>().Do(m => OnTileBuildStarted(m.TileCenter)).Subscribe();
            _messageBus.AsObservable<TileLoadFinishMessage>().Do(m => OnTileBuildFinished(m.Tile)).Subscribe();
            _messageBus.AsObservable<TileDestroyMessage>().Do(m => OnTileDestroyed(m.Tile)).Subscribe();
        }

        private void OnTileDestroyed(Tile tile)
        {
            _trace.Debug(LogTag, "Tile destroyed: center:{0}", tile.MapCenter.ToString());
        }

        public void OnTileFound(Tile tile, Vector2d position)
        {
        }

        public void OnTileBuildStarted(Vector2d center)
        {
            _stopwatch.Start();
            _trace.Debug(LogTag, "Tile build begin: center:{0}", center.ToString());
        }

        public void OnTileBuildFinished(Tile tile)
        {
            _stopwatch.Stop();
            _trace.Debug(LogTag, String.Format("{0} tile of size {1}x{2} is loaded in {3} ms. Trigger GC.",
                tile.RenderMode, tile.Rectangle.Width, tile.Rectangle.Height, _stopwatch.ElapsedMilliseconds));
            GC.Collect();
            _stopwatch.Reset();
        }

#if UNITY_EDITOR

        // Use this function to store dynamically built objects.
        private void SaveAsPrefab(GameObject gameObject)
        {
            // The paths to the mesh/prefab assets.
            string meshPath = String.Format("Assets/{0}.mesh", gameObject.name);
            string prefabPath = String.Format("Assets/{0}.prefab", gameObject.name);

            // Delete the assets if they already exist.
            UnityEditor.AssetDatabase.DeleteAsset(meshPath);
            UnityEditor.AssetDatabase.DeleteAsset(prefabPath);

            // Create the mesh somehow.
            Mesh mesh = gameObject.GetComponent<MeshFilter>().mesh;

            // Save the mesh as an asset.
            UnityEditor.AssetDatabase.CreateAsset(mesh, meshPath);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();

            // Create a transform somehow, using the mesh that was previously saved.
            Transform trans = gameObject.transform;

            // Save the transform's GameObject as a prefab asset.
            UnityEditor.PrefabUtility.CreatePrefab(prefabPath, trans.gameObject);
        }
#endif
    }
}
