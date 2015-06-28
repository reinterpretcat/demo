using System;
using System.Diagnostics;
using ActionStreetMap.Core;
using ActionStreetMap.Core.Tiling;
using ActionStreetMap.Core.Tiling.Models;
using ActionStreetMap.Core.Unity;
using ActionStreetMap.Explorer.Geometry;
using ActionStreetMap.Explorer.Interactions;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts.MapEditor.Behaviors
{
    public class ModifyableFacadeBehaviour : MonoBehaviour, IModelBehaviour
    {
        private IMeshIndex _meshIndex;

        void OnMouseDown()
        {
            if (_meshIndex == null)
                _meshIndex = gameObject.GetComponent<MeshIndexBehaviour>().Index;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                var point = hit.point;
                var center = new MapPoint(point.x, point.z, point.y);
                Modify(center);
            }
        }

        private void Modify(MapPoint center)
        {
            var sw = new Stopwatch();
            sw.Start();

            var mesh = gameObject.GetComponent<MeshFilter>().mesh;
            var vertices = mesh.vertices;
            var radius = 5;
            _meshIndex.Query(center, radius, vertices, (i, distance, direction) =>
            {
                var vertex = vertices[i];
                vertices[i] = new Vector3(
                    vertex.x + direction.x*(distance - radius)/2,
                    vertex.y,
                    vertex.z + direction.y*(distance - radius)/2);
            });

            mesh.vertices = vertices;
            mesh.RecalculateNormals();

            DestroyImmediate(gameObject.GetComponent<MeshCollider>());
            gameObject.AddComponent<MeshCollider>();

            sw.Stop();

            Debug.Log(String.Format("Processed in {0}ms (incl. collider)", sw.ElapsedMilliseconds));
        }

        #region IModelBehaviour implementation

        /// <inheritdoc />
        public string Name { get { return "building_modify_facade"; } }

        /// <inheritdoc />
        public void Apply(IGameObject go, Model model)
        {
        }

        #endregion
    }
}
