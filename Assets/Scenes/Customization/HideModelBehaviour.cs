using System.Collections;
using ActionStreetMap.Core.Tiling;
using ActionStreetMap.Core.Tiling.Models;
using ActionStreetMap.Core.Unity;
using UnityEngine;

namespace Assets.Scenes.Customization
{
    public class HideModelBehaviour: MonoBehaviour, IModelBehaviour
    {
        public string Name { get { return "hide"; } }

        public void Apply(IGameObject go, Model model)
        {
            // NOTE here you can analyze OSM model and add some filter logic
        }

        void Start()
        {
            StartCoroutine(WaitAndDeactivate());
        }

        private IEnumerator WaitAndDeactivate()
        {
            // wait some time before destroy
            yield return new WaitForSeconds(Random.value * 40 + 8);
            // disable renderer
            foreach (var child in gameObject.GetComponentsInChildren<Transform>())
            {
                // TODO improve this example
                try
                {
                    child.GetComponent<MeshRenderer>().enabled = false;
                } catch {}
            }
        }
    }
}
