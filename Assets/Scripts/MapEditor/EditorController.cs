using System;
using System.Collections.Generic;
using System.Linq;
using ActionStreetMap.Core.Geometry;
using ActionStreetMap.Core.Scene;
using ActionStreetMap.Explorer.Scene;
using ActionStreetMap.Explorer.Tiling;
using Assets.Scripts.MapEditor.Behaviors;
using UnityEngine;
using Tree = ActionStreetMap.Core.Scene.Tree;

namespace Assets.Scripts.MapEditor
{
    /// <summary> Editor controller. </summary>
    public sealed class EditorController
    {
        private readonly ITileModelEditor _tileModelEditor;

        /// <summary> Creates instance of <see cref="EditorController"/>. </summary>
        public EditorController(ITileModelEditor tileModelEditor)
        {
            _tileModelEditor = tileModelEditor;
        }

        /// <summary> Adds building with default properties using given foorprint. </summary>
        public void AddBuilding(List<Vector3> footPrint)
        {
            _tileModelEditor.AddBuilding(new Building()
            {
                Height = 0,
                Footprint = footPrint.Select(p => new Vector2d(p.x, p.z)).ToList()
            });
        }

        /// <summary> Adds barrier with default properties using given foorprint. </summary>
        public void AddBarrier(List<Vector3> footPrint)
        {
            _tileModelEditor.AddBarrier(new Barrier()
            {
                Height = 0,
                Footprint = footPrint.Select(p => new Vector2d(p.x, p.z)).ToList()
            });
        }

        /// <summary> Adds tree with default properties. </summary>
        public void AddTree(Vector3 point)
        {
            _tileModelEditor.AddTree(new Tree()
            {
                Point = new Vector2d(point.x, point.z)
            });
        }

        #region Terrain modifications

        /// <summary> Modifies terrain height in given point. </summary>
        public void ModifyTerrain(Vector3 center, bool upMode)
        {
            const float radius = 5;
            var forceDirection = upMode ? new Vector3(0, 1, 0) : new Vector3(0, -1, 0);
            BehaviourHelper.Modify(new MeshQuery()
            {
                Epicenter = center,
                Radius = radius,
                ForceDirection = forceDirection,
                OffsetThreshold = 1,
                GetForceChange = distance => 2 / ((float)Math.Pow(distance + 1, 1.67))
            });
        }

        #endregion
    }
}
