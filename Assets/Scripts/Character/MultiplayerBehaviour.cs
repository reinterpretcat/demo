using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Character
{
    public class MultiplayerBehaviour: NetworkBehaviour
    {
        public override void OnStartLocalPlayer()
        {
            GetComponent<ThirdPersonController>().enabled = true;

            var mouseOrbit = Camera.main.GetComponent<MouseOrbit>();
            mouseOrbit.target = transform;
            mouseOrbit.enabled = true;
        }
    }
}
