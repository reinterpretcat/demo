using System;
using System.Collections.Generic;
using System.Diagnostics;
using ActionStreetMap.Core;
using ActionStreetMap.Core.Geometry.Triangle.Geometry;
using ActionStreetMap.Core.Geometry.Triangle.Meshing.Algorithm;
using ActionStreetMap.Explorer.Scene;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts
{
    public class ModifyableBehaviour: MonoBehaviour
    {
        private MeshBehaviour _triangleSource; 
        void Start()
        {
            Debug.Log("Start is called");
            _triangleSource = gameObject.GetComponent<MeshBehaviour>();
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
            var triangles = mesh.triangles;

            var radius = 5;
            var epicenter = new Vector3(center.X, center.Elevation, center.Y);

            var indecies = _triangleSource.GetAffectedIndices(center, radius + 15f);

            ModifyVertices(indecies, vertices, epicenter, radius);

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


        private void ModifyVertices(List<int> indecies, Vector3[] vertices, Vector3 epicenter, float radius)
        {
            for (int j = 0; j < indecies.Count; j++)
            {
                int outerIndex = indecies[j] * 3;
                // modify vertices
                for (var k = 0; k < 3; k++)
                {
                    var index = outerIndex + k;
                    var vertex = vertices[index];
                    var distance = Vector3.Distance(vertex, epicenter);
                    if (distance < radius)
                        vertices[index] = new Vector3(vertex.x, vertex.y - (distance - radius) / 2, vertex.z);
                }
            }

            for (int j = 0; j < indecies.Count; j++)
            {
                int outerIndex = indecies[j] * 3;
                // search and modify vertices on triangle sides
                for (var i = 0; i < indecies.Count; i++)
                {
                    if (i == j) continue;

                    int innerIndex = indecies[i] * 3;

                    for (int k = 0; k < 3; k++)
                    {
                        int vertIndex = innerIndex + k;
                        if (ModifyVertextOnSegment(vertices, vertIndex, outerIndex + 0, outerIndex + 1) ||
                            ModifyVertextOnSegment(vertices, vertIndex, outerIndex + 1, outerIndex + 2) ||
                            ModifyVertextOnSegment(vertices, vertIndex, outerIndex + 2, outerIndex + 0))
                            modifiedCount++;
                    }
                }
            }
        }

        private int modifiedCount = 0;

        private bool IsVertextOnSegment(Vector3 p, Vector3 a, Vector3 b)
        {
            var vert2D = new Vector2(p.x, p.z);
            var a2D = new Vector2(a.x, a.z);
            var b2D = new Vector2(b.x, b.z);

            if (vert2D == a2D || vert2D == b2D) return false;
            return Math.Abs(Vector2.Distance(vert2D, a2D) + Vector2.Distance(vert2D, b2D) - Vector2.Distance(a2D, b2D)) < 0.01f;
        }

        private bool ModifyVertextOnSegment(Vector3[] vertices, int vIndex, int aIndex, int bIndex)
        {
            var p = vertices[vIndex];
            var a = vertices[aIndex];
            var b = vertices[bIndex];

            if (!IsVertextOnSegment(p, a, b))
                return false;

            var ray = b - a; // find direction from p1 to p2
            var rel = p - a; // find position relative to p1
            var n = ray.normalized; // create ray normal
            var l = Vector3.Dot(n, rel); // calculate dot
            var result = a + n * l; // convert back into world space

            vertices[vIndex] = result;

            return true;
        }

    }
}
