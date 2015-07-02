using System.Collections.Generic;
using System.Linq;
using ActionStreetMap.Core;
using ActionStreetMap.Core.Scene;
using ActionStreetMap.Core.Tiling;
using ActionStreetMap.Explorer.Interactions;
using ActionStreetMap.Explorer.Tiling;
using ActionStreetMap.Infrastructure.Diagnostic;

using UnityEngine;
using Tree = ActionStreetMap.Core.Scene.Tree;

namespace Assets.Scripts.MapEditor
{
    /// <summary> Editor controller. </summary>
    public sealed class EditorController
    {
        private const string CategoryName = "Editor";

        private readonly ITileController _tileController;
        private readonly ITileModelEditor _tileModelEditor;
        private readonly ITrace _trace;

        /// <summary> Creates instance of <see cref="EditorController"/>. </summary>
        public EditorController(ITileController tileController, ITileModelEditor tileModelEditor, 
            ITrace trace)
        {
            _tileController = tileController;
            _tileModelEditor = tileModelEditor;
            _trace = trace;
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
        public void AddTree(Vector3 point)
        {
            _tileModelEditor.AddTree(new Tree()
            {
                Point = new MapPoint(point.x, point.z, point.y)
            });
        }

        #region Terrain modifications

        /// <summary> Modifies terrain height in given point. </summary>
        public void ModifyTerrain(Vector3 center, bool upMode)
        {
            // TODO save this event
            var anyCellFound = false;
            foreach (var cell in GetCells(center))
            {
                anyCellFound = true;
                var meshIndex = cell.GetComponent<MeshIndexBehaviour>().Index;
                var mesh = cell.GetComponent<MeshFilter>().mesh;
                var vertices = mesh.vertices;

                const float radius = 5;

                bool isModified = false;
                meshIndex.Query(new MapPoint(center.x, center.z, center.y), radius, vertices, 
                    (i, distance, _) =>
                {
                    var vertex = vertices[i];
                    float heightDiff = (distance - radius) / 2;
                    vertices[i] = new Vector3(
                        vertex.x,
                        vertex.y + (upMode ? -heightDiff : heightDiff),
                        vertex.z);
                    isModified = true;
                });

                if (isModified)
                {
                    mesh.vertices = vertices;
                    mesh.RecalculateBounds();
                    mesh.RecalculateNormals();

                    UnityEngine.Object.DestroyImmediate(cell.GetComponent<MeshCollider>());
                    cell.AddComponent<MeshCollider>();
                }
            }

            if (!anyCellFound)
                _trace.Warn(CategoryName, "Cannot find any terrain cell");           
        }

        private IEnumerable<GameObject> GetCells(Vector3 point)
        {
            var tile = _tileController.GetTile(new MapPoint(point.x, point.z, point.y));
            var tileObject = tile.GameObject.GetComponent<GameObject>();

            foreach (Transform child in tileObject.transform)
            {
                var name = child.gameObject.name;
                if (name == "terrain")
                {
                    foreach (Transform cell in child.gameObject.transform)
                    {
                        if (cell.gameObject.GetComponent<Renderer>().bounds.Contains(point))
                            yield return cell.gameObject;
                    }
                }
            }
        }

        #endregion
    }
}
