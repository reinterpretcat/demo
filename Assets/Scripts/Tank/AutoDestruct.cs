using UnityEngine;

namespace Assets.Scripts.TankDemo
{
    public class AutoDestruct : MonoBehaviour
    {
        public float DestructTime = 2.0f;

        void Start()
        {
            Destroy(gameObject, DestructTime);
        }
    }
}
