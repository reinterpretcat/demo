using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Demo
{
    public class WaterBehavior: MonoBehaviour
    {
        public float scale = 1f;
        public float speed = 2.0f;
        public float noiseStrength = 0.1f;
        public float UpdateFrequency = .05f;

        MeshFilter mf = new MeshFilter();


        Vector3[] baseHeight;
        Vector3[] newVerts;

        void Start()
        {
            mf = this.GetComponent<MeshFilter>();
            // mesh = mf.mesh;
            baseHeight = mf.mesh.vertices;
            newVerts = new Vector3[baseHeight.Length];
            StartCoroutine(DoWave());
        }

        IEnumerator DoWave()
        {
            while (true)
            {
                for (var i = 0; i < newVerts.Length; i++)
                {

                    Vector3 vertex = baseHeight[i];

                    float s = (Time.time * speed + baseHeight[i].x + baseHeight[i].y + baseHeight[i].z) * scale;
                    float finalVal = Mathf.Sin(1.25f * s) * noiseStrength;//(Mathf.Sin(s) + Mathf.Sin(1.25f * s)) * noiseStrength;

                    vertex.y += finalVal;

                    newVerts[i] = vertex;
                }
                mf.mesh.vertices = newVerts;
                mf.mesh.RecalculateNormals();

                yield return new WaitForSeconds(UpdateFrequency);

            }
        }
    }
}
