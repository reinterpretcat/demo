using System;
using System.Collections.Generic;
using System.Diagnostics;
using ActionStreetMap.Core;
using ActionStreetMap.Explorer.Geometry;
using ActionStreetMap.Explorer.Scene.Behaviours;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts.Modifications
{
    public class ModifyableFacadeBehaviour : MonoBehaviour
    {
        private IMeshIndex _meshIndex;
        void Start()
        {
            _meshIndex = gameObject.GetComponent<MeshIndexBehaviour>().Index;
        }
        private int modifiedCount = 0;

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
            var triangles = mesh.triangles;

            var radius = 3;
            var epicenter = new Vector3(center.X, center.Elevation, center.Y);

            MapPoint direction;
            List<int> indecies = _meshIndex.Query(center, radius, out direction);

            ModifyVertices(indecies, vertices, epicenter, radius, new Vector2(direction.X, direction.Y));

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            DestroyImmediate(gameObject.GetComponent<MeshCollider>());
            gameObject.AddComponent<MeshCollider>();

            sw.Stop();
            
            Debug.Log(String.Format("Processed: {0}, modified: {1}. Total: {2}. Time (incl. collider): {3} ms",
                indecies.Count, modifiedCount, vertices.Length, sw.ElapsedMilliseconds));
            modifiedCount = 0;
        }

        private void ModifyVertices(List<int> indecies, Vector3[] vertices, Vector3 epicenter, float radius, Vector2 direction)
        {
            for (int j = 0; j < indecies.Count; j++)
            {
                int outerIndex = indecies[j]*3;
                // modify vertices
                for (var k = 0; k < 3; k++)
                {
                    var index = outerIndex + k;
                    var vertex = vertices[index];
                    var distance = Vector3.Distance(vertex, epicenter);
                    if (distance < radius)
                    {
                        vertices[index] = new Vector3(
                            vertex.x + direction.x * (distance - radius) / 2, 
                            vertex.y,
                            vertex.z + direction.y * (distance - radius) / 2);

                        modifiedCount++;
                    }
                }
            }
        }
    }
}
