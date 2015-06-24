using System.Collections.Generic;
using System.Linq;
using ActionStreetMap.Core;
using ActionStreetMap.Core.Scene;
using ActionStreetMap.Explorer.Tiling;
using ActionStreetMap.Infrastructure.Reactive;
using UnityEngine;
using Tree = ActionStreetMap.Core.Scene.Tree;

namespace Assets.Scripts.MapEditor
{
    /// <summary> Editor con troller. </summary>
    public sealed class EditorController
    {
        private readonly ITileModelEditor _tileModelEditor;

        private EditorActionMode _actionMode;

        /// <summary> Creates instance of <see cref="EditorController"/>. </summary>
        public EditorController(ITileModelEditor tileModelEditor, IMessageBus messageBus)
        {
            _tileModelEditor = tileModelEditor;

            messageBus.AsObservable<TerrainPointMessage>().Subscribe(HandlePointMessage);
            messageBus.AsObservable<TerrainPolylineMessage>().Subscribe(HandlePolylineMessage);
            messageBus.AsObservable<TerrainPolygonMessage>().Subscribe(HandlePolygonMessage);

            messageBus.AsObservable<EditorActionMode>().Subscribe(a => _actionMode = a);
        }

        private void HandlePointMessage(TerrainPointMessage msg)
        {
            var point = msg.Point;
            if (_actionMode == EditorActionMode.AddTree)
                AddTree(new MapPoint(point.x, point.z, point.y));
        }

        private void HandlePolylineMessage(TerrainPolylineMessage msg)
        {
            if (_actionMode == EditorActionMode.AddBarrier)
                AddBarrier(msg.Polyline);
        }

        private void HandlePolygonMessage(TerrainPolygonMessage msg)
        {
            if (_actionMode == EditorActionMode.AddBuilding)
                AddBuilding(msg.Polygon);
        }

        /// <summary> Adds building with default properties using given foorprint. </summary>
        public void AddBuilding(List<Vector3> footPrint)
        {
            _tileModelEditor.AddBuilding(new Building()
            {
                Height = 0,
                Footprint = footPrint.Select(p => new MapPoint(p.x, p.z, p.y)).ToList()
            });
        }

        /// <summary> Adds barrier with default properties using given foorprint. </summary>
        public void AddBarrier(List<Vector3> footPrint)
        {
            _tileModelEditor.AddBarrier(new Barrier()
            {
                Height = 0,
                Footprint = footPrint.Select(p => new MapPoint(p.x, p.z, p.y)).ToList()
            });
        }

        /// <summary> Adds tree with default properties. </summary>
        public void AddTree(MapPoint point)
        {
            _tileModelEditor.AddTree(new Tree()
            {
                Point = point
            });
        }

        /// <summary> Allows editor to listen events. </summary>
        public static void Subscribe(ITileModelEditor tileModelEditor, IMessageBus messageBus)
        {
            // TODO do it in nicer way
            new EditorController(tileModelEditor, messageBus);
        }
    }
}
