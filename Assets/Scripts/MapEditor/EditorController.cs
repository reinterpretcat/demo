using System.Collections.Generic;
using System.Linq;
using ActionStreetMap.Core;
using ActionStreetMap.Core.Scene;
using ActionStreetMap.Explorer.Tiling;
using ActionStreetMap.Infrastructure.Reactive;
using UnityEngine;

namespace Assets.Scripts.Editor
{
    /// <summary> Editor con troller. </summary>
    public sealed class EditorController
    {
        private readonly ITileModelEditor _tileModelEditor;

        /// <summary> Creates instance of <see cref="EditorController"/>. </summary>
        public EditorController(ITileModelEditor tileModelEditor, IMessageBus messageBus)
        {
            _tileModelEditor = tileModelEditor;

            messageBus.AsObservable<TerrainPolygonMessage>()
                      .Subscribe(m => AddBuilding(m.Polygon));
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

        public static void Subscribe(ITileModelEditor tileModelEditor, IMessageBus messageBus)
        {
            // TODO do it in nicer way
            new EditorController(tileModelEditor, messageBus);
        }
    }
}
