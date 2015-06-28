using System;
using System.Diagnostics;
using ActionStreetMap.Core;
using ActionStreetMap.Core.Tiling;
using ActionStreetMap.Core.Tiling.Models;
using ActionStreetMap.Core.Unity;
using ActionStreetMap.Explorer.Geometry;
using ActionStreetMap.Explorer.Interactions;
using ActionStreetMap.Infrastructure.Reactive;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts.MapEditor.Behaviors
{
    public class ModifyableTerrainBehaviour: MonoBehaviour, IModelBehaviour
    {
        private IMeshIndex _meshIndex;

        private static EditorActionMode _action = EditorActionMode.None;

        void Start()
        {
            ApplicationManager.Instance.GetService<IMessageBus>()
                .AsObservable<EditorActionMode>().Subscribe(m => _action = m);
        }

        void OnMouseDown()
        {
            if (_action != EditorActionMode.TerrainUp && _action != EditorActionMode.TerrainDown)
                return;

            if (_meshIndex == null)
                _meshIndex = gameObject.GetComponent<MeshIndexBehaviour>().Index;
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                var point = hit.point;
                var center = new MapPoint(point.x, point.z, point.y);
                Modify(center, _action == EditorActionMode.TerrainUp);
            }
        }

        private void Modify(MapPoint center, bool upMode)
        {
            var sw = new Stopwatch();
            sw.Start();
            
            var mesh = gameObject.GetComponent<MeshFilter>().mesh;
            var vertices = mesh.vertices;

            var radius = 5;

            bool isModified = false;
            _meshIndex.Query(center, radius, vertices, (i, distance, _) =>
            {
                var vertex = vertices[i];
                float heightDiff = (distance - radius)/2;
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

                DestroyImmediate(gameObject.GetComponent<MeshCollider>());
                gameObject.AddComponent<MeshCollider>();
            }

            sw.Stop();
            Debug.Log(String.Format("Processed in {0}ms (incl. collider). Status: {1} ", sw.ElapsedMilliseconds, isModified));
        }

        #region IModelBehaviour implementation

        /// <inheritdoc />
        public string Name { get { return "terrain_modify"; } }

        /// <inheritdoc />
        public void Apply(IGameObject go, Model model)
        {
            /*foreach (Transform cell in gameObject.transform)
            {
                cell.gameObject.AddComponent<ModifyableTerrainBehaviour>();
                cell.gameObject.AddComponent<TerrainDrawBehaviour>();
            }*/
        }

        #endregion
    }
}
