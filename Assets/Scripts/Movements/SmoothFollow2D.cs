

using UnityEngine;

namespace Assets.Scripts.Demo
{

    public class SmoothFollow2D: MonoBehaviour
    {
        public Transform target;
        public float smoothTime = 0.3f;
        private Transform thisTransform;
        private Vector3 velocity;

        private void Start()
        {
            thisTransform = transform;
        }

        private void Update()
        {
            var x  = Mathf.SmoothDamp(thisTransform.position.x, target.position.x, ref velocity.x, smoothTime);
            var z = Mathf.SmoothDamp(thisTransform.position.z, target.position.z, ref velocity.z, smoothTime);

            thisTransform.position = new Vector3(x, thisTransform.position.y, z);
        }
    }
}