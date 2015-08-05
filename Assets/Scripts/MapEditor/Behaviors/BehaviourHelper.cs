using System;
using System.Collections;
using ActionStreetMap.Explorer.Interactions;
using ActionStreetMap.Explorer.Scene;
using ActionStreetMap.Infrastructure.Reactive;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.MapEditor.Behaviors
{
    static class BehaviourHelper
    {
        public static void Modify(MeshQuery query)
        {
            Collider[] hitColliders = Physics.OverlapSphere(query.Epicenter, query.Radius);
            foreach (var hitCollider in hitColliders)
            {
                var meshIndexBehavior = hitCollider.gameObject.GetComponent<MeshIndexBehaviour>();
                if (meshIndexBehavior == null)
                    continue;

                var mesh = hitCollider.gameObject.GetComponent<MeshFilter>().mesh;
                query.Vertices = mesh.vertices;

                var meshQueryResult = meshIndexBehavior.Index.Modify(query);

                if (meshQueryResult.IsDestroyed)
                {
                    var collider = hitCollider;
                    ObservableUnity
                        .FromCoroutine(() => SplitMesh(collider.gameObject))
                        .Subscribe();
                }
                else if (meshQueryResult.IsModified)
                {
                    mesh.vertices = meshQueryResult.Vertices;
                    meshIndexBehavior.IsMeshModified = true;
                }

                Debug.Log(String.Format("MeshIndex:{0} modified:{1} scanned tris:{2}, forceDir:{3}", 
                    hitCollider.name, meshQueryResult.ModifiedVertices, meshQueryResult.ScannedTriangles, query.ForceDirection));
            }
        }

        private static IEnumerator SplitMesh(GameObject gameObject)
        {
            var mf = gameObject.GetComponent<MeshFilter>();
            var mr = gameObject.GetComponent<MeshRenderer>();
            Mesh m = mf.mesh;
            Vector3[] verts = m.vertices;
            Vector3[] normals = m.normals;
            Color[] colors = m.colors;
            for (int submesh = 0; submesh < m.subMeshCount; submesh++)
            {
                int[] indices = m.GetTriangles(submesh);
                for (int i = 0; i < indices.Length; i += 3)
                {
                    var newVerts = new Vector3[3];
                    var newNormals = new Vector3[3];
                    var newColors = new Color[3];
                    for (int n = 0; n < 3; n++)
                    {
                        int index = indices[i + n];
                        newVerts[n] = verts[index];
                        newColors[n] = colors[index];
                        newNormals[n] = normals[index];
                    }
                    var mesh = new Mesh();
                    mesh.vertices = newVerts;
                    mesh.normals = newNormals;
                    mesh.colors = newColors;

                    mesh.triangles = new int[] { 0, 1, 2, 2, 1, 0 };

                    var go = new GameObject("Triangle " + (i / 3));
                    go.transform.parent = gameObject.transform.parent;
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localRotation = Quaternion.identity;

                    go.transform.position = gameObject.transform.position;
                    go.transform.rotation = gameObject.transform.rotation;
                    go.AddComponent<MeshRenderer>().material = mr.materials[submesh];
                    go.AddComponent<MeshFilter>().mesh = mesh;
                    go.AddComponent<SphereCollider>();
                    go.AddComponent<Rigidbody>().AddExplosionForce(0.1f, gameObject.transform.position, 3);

                    GameObject.Destroy(go, 5 + Random.Range(0.0f, 5.0f));
                }
            }
            mr.enabled = false;

            Time.timeScale = 0.2f;
            yield return new WaitForSeconds(0.0f);
            Time.timeScale = 1.0f;
            GameObject.Destroy(gameObject);
        }
    }
}
