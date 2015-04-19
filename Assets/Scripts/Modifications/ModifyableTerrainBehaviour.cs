using System;
using System.Collections.Generic;
using System.Diagnostics;
using ActionStreetMap.Core;
using ActionStreetMap.Explorer.Geometry;
using ActionStreetMap.Explorer.Interactions;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts.Modifications
{
    public class ModifyableTerrainBehaviour: MonoBehaviour
    {
        private IMeshIndex _meshIndex;

        void Start()
        {
            _meshIndex = gameObject.GetComponent<MeshIndexBehaviour>().Index;
        }

        private void OnMouseDown()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                var point = hit.point;
                var center = new MapPoint(point.x, point.z, point.y);
                Modify(center);
            }
        }

        void Modify(MapPoint center)
        {
            var sw = new Stopwatch();
            sw.Start();
            
            var mesh = gameObject.GetComponent<MeshFilter>().mesh;
            var vertices = mesh.vertices;

            var radius = 5;
            _meshIndex.Query(center, radius, vertices, (i, distance, _) =>
            {
                var vertex = vertices[i];
                vertices[i] = new Vector3(vertex.x, vertex.y - (distance - radius)/2, vertex.z);
            });

            mesh.vertices = vertices;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            DestroyImmediate(gameObject.GetComponent<MeshCollider>());
            gameObject.AddComponent<MeshCollider>();

            sw.Stop();
            Debug.Log(String.Format("Processed in {0}ms (incl. collider)", sw.ElapsedMilliseconds));
        }
    }
}
