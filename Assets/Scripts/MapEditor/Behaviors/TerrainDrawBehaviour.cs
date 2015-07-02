using System.Collections.Generic;
using System.Linq;
using ActionStreetMap.Core;
using ActionStreetMap.Core.Tiling;
using ActionStreetMap.Core.Tiling.Models;
using ActionStreetMap.Core.Unity;
using ActionStreetMap.Infrastructure.Reactive;
using UnityEngine;

namespace Assets.Scripts.MapEditor.Behaviors
{
    /// <summary> Provides the way to draw lines on game object. </summary>
    public class TerrainDrawBehaviour : MonoBehaviour, IModelBehaviour
    {
        /// <summary> Radius of last point detection logic. </summary>
        public float SensivityRadius = 1f;
        /// <summary> Line color. </summary>
        public Color LineColor = new Color(1, 0, 0, 1);

        private static TerrainInputMode _inputMode = TerrainInputMode.None;
        private static EditorActionMode _actionMode = EditorActionMode.None;

        private IMessageBus _messageBus;

        private float _heightError = 0.5f;

        private static Material LineMaterial = CreateMaterial();
        
        // NOTE: Point buffer should be static to allow cross tile selection
        private static readonly List<Vector3> MarkPoints = new List<Vector3>();

        private static Material CreateMaterial()
        {
            return new Material(
                 @"Shader ""Lines/Colored Blended"" {
                     SubShader {
                         Tags { ""RenderType""=""Opaque"" }
                         Pass {
                             ZWrite On
                             ZTest LEqual
                             Cull Off
                             Fog { Mode Off }
                             BindChannels {
                                 Bind ""vertex"", vertex Bind ""color"", color
                             }
                         }
                     }
                 }");
        }

        void Start()
        {
            _messageBus= ApplicationManager.Instance.GetService<IMessageBus>();
            _messageBus.AsObservable<TerrainInputMode>().Subscribe(m => _inputMode = m);
            _messageBus.AsObservable<EditorActionMode>().Subscribe(a => _actionMode = a);
        }

        void Update()
        {
            if (_inputMode == TerrainInputMode.None)
            {
                Clear();
                return;
            }

            if (Input.GetKey(KeyCode.Escape))
                MarkPoints.Clear();
            if (Input.GetKey(KeyCode.Return))
                SendPolyline();
        }

        void OnMouseDown()
        {
            if (_inputMode == TerrainInputMode.None)
            {
                Clear();
                return;
            }

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                var point = hit.point;
                if (_inputMode == TerrainInputMode.SetPoint)
                    SendPoint(point);
                else if (IsPolygonClosedByPoint(point))
                    SendPolygon();
                else
                    MarkPoints.Add(point);
            }
        }

        private bool IsPolygonClosedByPoint(Vector3 point)
        {
            return MarkPoints.Count > 1 &&
                MarkPoints.Any(mark => Vector3.Distance(mark, point) <= SensivityRadius);
        }

        void OnRenderObject()
        {
            LineMaterial.SetPass(0);

            GL.PushMatrix();
            GL.MultMatrix(transform.localToWorldMatrix);

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
            GL.PopMatrix();
        }

        private void Clear()
        {
            MarkPoints.Clear();
        }

        private void SendPoint(Vector3 point)
        {
            _messageBus.Send(new TerrainPointMessage()
            {
                ActionMode = _actionMode,
                Point = point
            });
            Clear();
        }

        private void SendPolyline()
        {
            if (MarkPoints.Count > 1)
                _messageBus.Send(new TerrainPolylineMessage()
                {
                    ActionMode = _actionMode,
                    Polyline = MarkPoints.ToList()
                });
            Clear();
        }

        private void SendPolygon()
        {
            if (MarkPoints.Count > 2)
                _messageBus.Send(new TerrainPolylineMessage()
                {
                    ActionMode = _actionMode,
                    Polyline = MarkPoints.ToList()
                });
            Clear();
        }

        #region IModelBehaviour implementation

        /// <inheritdoc />
        public string Name { get { return "terrain_draw"; } }

        /// <inheritdoc />
        public void Apply(IGameObject go, Model model)
        {
        }

        #endregion
    }
}
