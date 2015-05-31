using ActionStreetMap.Core;
using UnityEngine;

namespace Assets.Scripts.Character
{
    public class OverviewModeBehaviour: MonoBehaviour
    {
        public float PanSpeed = 50;		// Speed of the camera when being panned

        private Vector3 _mouseOrigin;	// Position of cursor when mouse dragging starts
        private bool _isPanning;		// Is the camera being panned?

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _mouseOrigin = Input.mousePosition;
                _isPanning = true;
            }

            // Disable movements on button release
            if (!Input.GetMouseButton(0)) _isPanning = false;

            // Move the camera on it's XY plane
            if (_isPanning)
            {
                Vector3 pos = -Camera.main.ScreenToViewportPoint(Input.mousePosition - _mouseOrigin);
                Vector3 move = new Vector3(pos.x * PanSpeed, pos.y * PanSpeed, 0);
                transform.Translate(move, Space.Self);

                var centerOfScreen = Camera.main.ScreenToWorldPoint(
                    new Vector3(Screen.width / 2, Screen.height / 2, Camera.main.nearClipPlane));

                ApplicationManager.Instance
                    .Move(new MapPoint(centerOfScreen.x, centerOfScreen.z, centerOfScreen.y));
            }
        }
    }
}
