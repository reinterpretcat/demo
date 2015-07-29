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
                var point = hit.point;
                const float radius = 2;
                // TODO this should be changed with adding weapons
                BehaviourHelper.Modify(transform.forward, point, radius);
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
