using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scenes.MapLevelAssets.Scripts
{
    /// <summary> Provides the way to draw lines on game object. </summary>
    public class TerrainEditorBehaviour : MonoBehaviour
    {
        /// <summary> Radius of last point detection logic. </summary>
        public float SensivityRadius = 0.5f;
        /// <summary> Line color. </summary>
        public Color LineColor = new Color(1, 0, 0, 1);

        private float _heightError = 0.5f;
        private List<Vector3> _markPoints = new List<Vector3>();

        private void OnMouseDown()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                var point = hit.point;
                if (IsPolygonClosedByPoint(point))
                {
                    _markPoints.Clear();
                }
                else
                    _markPoints.Add(point);
            }
        }

        private bool IsPolygonClosedByPoint(Vector3 point)
        {
            return _markPoints.Count > 1 &&
                _markPoints.Any(mark => Vector3.Distance(mark, point) <= SensivityRadius);
        }

        void DrawConnectingLines()
        {
            if (_markPoints.Count > 0)
            {
                for (int i = 0; i < _markPoints.Count; i++)
                {
                    var cur = _markPoints[i];
                    Vector3 next;
                    if (i == _markPoints.Count - 1)
                    {
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit))
                            next = hit.point;
                        else
                            return;
                    }
                    else
                        next = _markPoints[i + 1];

                    GL.Begin(GL.LINES);
                    GL.Color(LineColor);
                    GL.Vertex3(cur.x, cur.y + _heightError, cur.z);
                    GL.Vertex3(next.x, next.y + _heightError, next.z);
                    GL.End();
                }
            }
        }

        /// <summary> To show the lines in the game window when it is running. </summary>
        void OnPostRender()
        {
            DrawConnectingLines();
        }

        /// <summary> To show the lines in the editor. </summary>
        void OnDrawGizmos()
        {
            DrawConnectingLines();
        }
    }
}
