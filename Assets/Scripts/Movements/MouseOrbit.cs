
using UnityEngine;

namespace Assets.Scripts.Movements
{
    public class MouseOrbit: MonoBehaviour
    {
        public Transform target;
        public float distance = 10.0f;
        public float zoomSensitivity = 10f;

        public float xSpeed = 250.0f;
        public float ySpeed = 120.0f;

        public float yMinLimit = -20f;
        public float yMaxLimit = 80f;

        private float x = 0.0f;
        private float y = 0.0f;

        private float zoomDistance;

        void Start()
        {
            var angles = transform.eulerAngles;
            x = angles.y;
            y = angles.x;

            zoomDistance = -distance;
            // Make the rigid body not change rotation
            if (rigidbody)
                rigidbody.freezeRotation = true;
        }
        private float ZoomAmount = 10; //With Positive and negative values
        private float MaxToClamp = 10;
        private float ROTSpeed = 10;

        private void LateUpdate()
        {
            zoomDistance += Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity;

            if (target && Input.GetMouseButton(1))
            {
                x += (float) (Input.GetAxis("Mouse X")*xSpeed*0.02);
                y -= (float) (Input.GetAxis("Mouse Y") * ySpeed * 0.02);

                y = ClampAngle(y, yMinLimit, yMaxLimit);
            }
            var rotation = Quaternion.Euler(y, x, 0);
            var position = rotation * (new Vector3(0.0f, 0.0f, zoomDistance)) + target.position;

            transform.rotation = rotation;
            transform.position = position;
        }

        private static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360)
                angle += 360;
            if (angle > 360)
                angle -= 360;
            return Mathf.Clamp(angle, min, max);
        }
    }
}
