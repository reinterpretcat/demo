using ActionStreetMap.Explorer.Interactions;
using ActionStreetMap.Explorer.Scene;
using UnityEngine;

namespace Assets.Scripts.MapEditor.Behaviors
{
    static class BehaviourHelper
    {
        public static void Modify(Vector3 forceDirection, Vector3 epicenter, float radius)
        {
            Collider[] hitColliders = Physics.OverlapSphere(epicenter, radius);
            foreach (var hitCollider in hitColliders)
            {
                var meshIndexBehavior = hitCollider.gameObject.GetComponent<MeshIndexBehaviour>();
                if (meshIndexBehavior == null)
                    continue;

                var collidePoint = hitCollider.ClosestPointOnBounds(epicenter);
                var mesh = hitCollider.gameObject.GetComponent<MeshFilter>().mesh;
                var vertices = mesh.vertices;
                var index = meshIndexBehavior.Index;

                meshIndexBehavior.IsMeshModified = index.Modify(new MeshQuery()
                    {
                        Epicenter = epicenter,
                        CollidePoint = collidePoint,
                        ForceDirection = forceDirection,
                        ForcePower = 1,
                        OffsetThreshold = 1,
                        Radius = radius,
                        Vertices = vertices
                    }) > 0;

                if (meshIndexBehavior.IsMeshModified)
                    mesh.vertices = vertices;
            }
        }
    }
}
