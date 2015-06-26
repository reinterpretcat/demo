using System;
using System.Diagnostics;
using ActionStreetMap.Core;
using ActionStreetMap.Explorer.Geometry;
using ActionStreetMap.Explorer.Interactions;
using ActionStreetMap.Infrastructure.Reactive;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts.MapEditor.Behaviors
{
    public class ModifyableTerrainBehaviour: MonoBehaviour
    {
        private IMeshIndex _meshIndex;

        private static EditorActionMode _action = EditorActionMode.None;

        private IMessageBus _messageBus;
        /// <summary> Messages bus. </summary>
        public IMessageBus MessageBus
        {
            set
            {
                _messageBus = value;
                _messageBus.AsObservable<EditorActionMode>().Subscribe(m => _action = m );
            }
        }

        void Start()
        {
            _meshIndex = gameObject.GetComponent<MeshIndexBehaviour>().Index;
        }

        private void OnMouseDown()
        {
            if (_action != EditorActionMode.TerrainUp && _action != EditorActionMode.TerrainDown)
                return;
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                var point = hit.point;
                var center = new MapPoint(point.x, point.z, point.y);
                Modify(center, _action == EditorActionMode.TerrainUp);
            }
        }

        void Modify(MapPoint center, bool upMode)
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
    }
}
