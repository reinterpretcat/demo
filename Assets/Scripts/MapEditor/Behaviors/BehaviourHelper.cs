using ActionStreetMap.Explorer.Interactions;
using ActionStreetMap.Explorer.Scene;
using UnityEngine;

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

                query.CollidePoint = hitCollider.ClosestPointOnBounds(query.Epicenter);
                query.ForceDirection = query.CollidePoint - query.Epicenter;
                var mesh = hitCollider.gameObject.GetComponent<MeshFilter>().mesh;
                query.Vertices = mesh.vertices;

                meshIndexBehavior.IsMeshModified = meshIndexBehavior.Index.Modify(query) > 0;

                if (meshIndexBehavior.IsMeshModified)
                    mesh.vertices = query.Vertices;
            }
        }
    }
}
