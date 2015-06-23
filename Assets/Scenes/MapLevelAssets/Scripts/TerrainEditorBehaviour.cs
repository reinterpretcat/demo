﻿using System.Collections.Generic;
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
        
        // NOTE: Point buffer should be static to allow cross tile selection
        private static readonly List<Vector3> MarkPoints = new List<Vector3>();

        private void OnMouseDown()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                var point = hit.point;
                if (IsPolygonClosedByPoint(point))
                {
                    MarkPoints.Clear();
                }
                else
                    MarkPoints.Add(point);
            }
        }

        private bool IsPolygonClosedByPoint(Vector3 point)
        {
            return MarkPoints.Count > 1 &&
                MarkPoints.Any(mark => Vector3.Distance(mark, point) <= SensivityRadius);
        }

        void DrawConnectingLines()
        {
            if (MarkPoints.Count > 0)
            {
                var count = MarkPoints.Count;
                var lastItemIndex = MarkPoints.Count - 1;
                for (int i = 0; i < count; i++)
                {
                    var cur = MarkPoints[i];
                    Vector3 next;
                    if (i == lastItemIndex)
                    {
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit))
                            next = hit.point;
                        else
                            return;
                    }
                    else
                        next = MarkPoints[i + 1];

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
