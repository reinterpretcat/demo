using UnityEngine;

namespace Assets.Scripts.Character
{
    public class MouseZoomPan: MonoBehaviour
    {
        //public float TurnSpeed = 4.0f;		// Speed of camera turning when mouse moves in along an axis
        public float PanSpeed = 100;		// Speed of the camera when being panned
        public float ZoomSpeed = 20;		// Speed of the camera going back and forth

        private Vector3 _mouseOrigin;	// Position of cursor when mouse dragging starts
        private bool _isPanning;		// Is the camera being panned?
        //private bool _isRotating;	// Is the camera being rotated?
        private bool _isZooming;		// Is the camera zooming?

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _mouseOrigin = Input.mousePosition;
                _isPanning = true;
            }

            //if (Input.GetMouseButtonDown(1))
            //{
            //    _mouseOrigin = Input.mousePosition;
            //    //_isRotating = true;
            //}

            // Get the middle mouse button
            if (Input.GetMouseButtonDown(2))
            {
                _mouseOrigin = Input.mousePosition;
                _isZooming = true;
            }

            // Disable movements on button release
            if (!Input.GetMouseButton(0)) _isPanning = false;
            //if (!Input.GetMouseButton(1)) _isRotating = false;
            if (!Input.GetMouseButton(2)) _isZooming = false;

            ////// Rotate camera along X and Y axis
            //if (_isRotating)
            //{
            //    Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - _mouseOrigin);

            //    transform.RotateAround(transform.position, transform.right, -pos.y * TurnSpeed);
            //    transform.RotateAround(transform.position, Vector3.up, pos.x * TurnSpeed);
            //}

            // Move the camera on it's XY plane
            if (_isPanning)
            {
                Vector3 pos = -Camera.main.ScreenToViewportPoint(Input.mousePosition - _mouseOrigin);
                Vector3 move = new Vector3(pos.x * PanSpeed, pos.y * PanSpeed, 0);
                transform.Translate(move, Space.Self);
            }

            // Move the camera linearly along Z axis
            if (_isZooming)
            {
                Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - _mouseOrigin);

                Vector3 move = pos.y * ZoomSpeed * transform.forward;
                transform.Translate(move, Space.World);
            }
        }
    }
}
