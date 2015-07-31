using ActionStreetMap.Core.Tiling;
using ActionStreetMap.Core.Tiling.Models;
using ActionStreetMap.Core.Unity;
using ActionStreetMap.Explorer.Interactions;
using ActionStreetMap.Explorer.Scene;
using UnityEngine;

namespace Assets.Scripts.MapEditor.Behaviors
{
    public class ModifiableMeshBehaviour : MonoBehaviour, IModelBehaviour
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
                const float radius = 2;
                // TODO this should be changed with adding weapons
                BehaviourHelper.Modify(new MeshQuery()
                {
                    Epicenter = hit.point,
                    Radius = radius,
                    ForceDirection = transform.forward,
                    ForcePower = 1,
                    OffsetThreshold = 1,
                });
            }
        }

        #region IModelBehaviour implementation

        /// <inheritdoc />
        public string Name { get { return "mesh_modify"; } }

        /// <inheritdoc />
        public void Apply(IGameObject go, Model model)
        {
        }

        #endregion
    }
}
