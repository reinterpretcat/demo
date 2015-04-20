using System;
using System.Collections.Generic;
using ActionStreetMap.Infrastructure.IO;
using ActionStreetMap.Infrastructure.Reactive;
using Assets.Scripts.Console;
using Assets.Scripts.Console.Utils;
using Assets.Scripts.Demo;
using ActionStreetMap.Core;
using ActionStreetMap.Explorer;
using ActionStreetMap.Infrastructure.Dependencies;
using ActionStreetMap.Infrastructure.Diagnostic;
using UnityEngine;

using RecordType = ActionStreetMap.Infrastructure.Diagnostic.DefaultTrace.RecordType;

namespace Assets.Scripts.Character
{
    public class ActionStreetMapBehavior : MonoBehaviour
    {
        public float Delta = 10;

        private IPositionObserver<MapPoint> _positionObserver;

        private ITrace _trace;

        private bool _isInitialized = false;

        private Vector3 _position = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        
        private DebugConsole _console;

        // Use this for initialization
        private void Start()
        {
            Initialize();
            
            
        }

        private void DrawRoundedCorner(MapPoint angularPoint, MapPoint p1, MapPoint p2, float radius)
        {
            //Vector 1
            double dx1 = angularPoint.X - p1.X;
            double dy1 = angularPoint.Y - p1.Y;

            //Vector 2
            double dx2 = angularPoint.X - p2.X;
            double dy2 = angularPoint.Y - p2.Y;

            //Angle between vector 1 and vector 2 divided by 2
            double angle = (Math.Atan2(dy1, dx1) - Math.Atan2(dy2, dx2)) / 2;

            //The length of segment between angular point and the points of intersection with the circle of a given radius
            double tan = Math.Abs(Math.Tan(angle));
            double segment = radius / tan;

            //Check the segment
            double length1 = GetLength(dx1, dy1);
            double length2 = GetLength(dx2, dy2);

            double length = Math.Min(length1, length2);

            if (segment > length)
            {
                segment = length;
                radius = (float)(length * tan);
            }

            //Points of intersection are calculated by the proportion between the coordinates of the vector, length of vector and the length of the segment.
            var p1Cross = GetProportionPoint(angularPoint, segment, length1, dx1, dy1);
            var p2Cross = GetProportionPoint(angularPoint, segment, length2, dx2, dy2);

            //Calculation of the coordinates of the circle center by the addition of angular vectors.
            double dx = angularPoint.X * 2 - p1Cross.X - p2Cross.X;
            double dy = angularPoint.Y * 2 - p1Cross.Y - p2Cross.Y;

            double L = GetLength(dx, dy);
            double d = GetLength(segment, radius);

            var circlePoint = GetProportionPoint(angularPoint, d, L, dx, dy);

            //StartAngle and EndAngle of arc
            var startAngle = Math.Atan2(p1Cross.Y - circlePoint.Y, p1Cross.X - circlePoint.X);
            var endAngle = Math.Atan2(p2Cross.Y - circlePoint.Y, p2Cross.X - circlePoint.X);

            //Sweep angle
            var sweepAngle = endAngle - startAngle;

            //Some additional checks
            if (sweepAngle < 0)
            {
                startAngle = endAngle;
                sweepAngle = -sweepAngle;
            }

            if (sweepAngle > Math.PI)
                sweepAngle = Math.PI - sweepAngle;

            //DrawLine(p1, p1Cross, Color.red);
            //DrawLine(p2, p2Cross, Color.red);

            //var left = circlePoint.X - radius;
           // var top = circlePoint.Y - radius;
           // var diameter = 2 * radius;
          

            //DrawArc(left, top, diameter, diameter, (float)(startAngle * degreeFactor), (float)(sweepAngle * degreeFactor));
            var points = GetArc(circlePoint, p1Cross, p2Cross, startAngle, sweepAngle, radius);

            for (int i = 0; i < points.Count; i++)
            {
                if (i != points.Count -1)
                    DrawLine(points[i], points[i+1], Color.red);
            }
        }

        private double GetLength(double dx, double dy)
        {
            return Math.Sqrt(dx * dx + dy * dy);
        }

        private MapPoint GetProportionPoint(MapPoint point, double segment, double length, double dx, double dy)
        {
            double factor = segment / length;
            return new MapPoint((float)(point.X - dx * factor), (float)(point.Y - dy * factor));
        }

        private List<MapPoint> GetArc(MapPoint circlePoint, MapPoint startPoint, MapPoint endPoint, double startAngle, double sweepAngle, float radius)
        {
            const double degreeInRad = 180/Math.PI;
            
            var degrees = Math.Abs(sweepAngle * degreeInRad);
            int sign = Math.Sign(sweepAngle);
            if (sign < 0)
                degrees = 180 - degrees;

            var step = 20;
            int pointCount = (int)Math.Ceiling(degrees / step);
            var points = new List<MapPoint>(pointCount + 1);
            points.Add(startPoint);
            for (int i = 1; i < pointCount; i++)
            {
                var pointX = (float) (circlePoint.X + Math.Cos(startAngle + sign*(double) (i*step)/degreeInRad)*radius);
                var pointY = (float) (circlePoint.Y + Math.Sin(startAngle + sign*(double) (i*step)/degreeInRad)*radius);
                points.Add(new MapPoint(pointX, pointY));
            }
            points.Add(endPoint);

            return points;
        }

        private void DrawLine(MapPoint start, MapPoint end, Color color)
        {
            Debug.DrawLine(new Vector3(start.X, 100, start.Y), new Vector3(end.X, 100, end.Y), color, 20f, false);
        }

        private void DrawTestPolygon()
        {
            var polygon = new List<MapPoint>()
            {
new MapPoint(162.8398f, -93.245f),
new MapPoint(162.642f, -92.95956f),
new MapPoint(162.3584f, -92.75901f),
new MapPoint(162.0234f, -92.66753f),
new MapPoint(161.6773f, -92.69615f),
new MapPoint(161.3618f, -92.84143f),
new MapPoint(161.2934f, -92.89339f),
new MapPoint(161.276f, -92.66331f),
new MapPoint(161.5401f, -92.43776f),
new MapPoint(161.7111f, -92.13548f),
new MapPoint(161.7684f, -91.79295f),
new MapPoint(161.7052f, -91.45147f),
new MapPoint(161.5336f, -91.15773f),
new MapPoint(161.6731f, -91.24914f),
new MapPoint(161.9287f, -91.48418f),
new MapPoint(162.2494f, -91.61758f),
new MapPoint(162.5963f, -91.63329f),
new MapPoint(162.9277f, -91.52937f),
new MapPoint(163.0939f, -91.42106f),
new MapPoint(163.1461f, -91.56002f),
new MapPoint(163.0134f, -91.65378f),
new MapPoint(162.7897f, -91.91946f),
new MapPoint(162.6704f, -92.24562f),
new MapPoint(162.6698f, -92.59291f),
new MapPoint(162.7881f, -92.91946f),








            };

            for (int i = 0; i < polygon.Count; i++)
            {
                DrawLine(polygon[i], polygon[i != polygon.Count - 1? i + 1: 0], Color.red);
            }
        }

        // Update is called once per frame
        private void Update()
        {
            //DrawLine(new MapPoint(200, 0), new MapPoint(0, 200));
            //DrawRoundedCorner(new MapPoint(0, 0), new MapPoint(20, 0), new MapPoint(20, 20), 5f);
           /* var p0 = new MapPoint(0, 0);
            var p1 = new MapPoint(10, 0);
            var p2 = new MapPoint(0, 10);
            DrawLine(p0, p1, Color.blue);
            DrawLine(p0, p2, Color.blue);
            DrawRoundedCorner(new MapPoint(0, 0), p1, p2, 5f);*/

            //DrawTestPolygon();

           // DrawLine(new MapPoint(549.5191f, -1311.029f), new MapPoint(549.801f, -1315.13f), Color.red);
            //DrawLine(new MapPoint(549.5191f, -1311.029f), new MapPoint(548.6796f, -1298.793f), Color.blue);

            if (_isInitialized && _position != transform.position)
            {
                _position = transform.position;
                Scheduler.ThreadPool.Schedule(() => 
                    _positionObserver.OnNext(new MapPoint(_position.x, _position.z, _position.y)));
            }
        }

        #region Initialization

        private void Initialize()
        {
            Scheduler.MainThread = new UnityMainThreadScheduler();
            // create and register DebugConsole inside Container
            var container = new Container();
            
            InitializeConsole(container);

            Scheduler.ThreadPool.Schedule(() =>
            {
                try
                {
                    IMessageBus messageBus = new MessageBus();
                    // these services should be registered inside container before GameRunner is constructed.
                    container.RegisterInstance(_trace);
                    container.RegisterInstance<IPathResolver>(new WinPathResolver());
                    container.RegisterInstance(messageBus);

                    var gameRunner = new GameRunner(container, @"Config/settings.json")
                        .RegisterPlugin<DemoBootstrapper>("demo", messageBus, _trace);
                    _positionObserver = gameRunner;
                    gameRunner.RunGame(new GeoCoordinate(52.53163,13.39195));

                    _isInitialized = true;
                }
                catch (Exception ex)
                {
                    _console.LogMessage(new ConsoleMessage("Error running game:" + ex, RecordType.Error, Color.red));
                    throw;
                }
            });
        }

        private void InitializeConsole(IContainer container)
        {
            var consoleGameObject = new GameObject("_DebugConsole_");
            _console = consoleGameObject.AddComponent<DebugConsole>();
            container.RegisterInstance(_console);
            // that is not nice, but we need to use commands registered in DI with their dependencies
            _console.Container = container; 
            _trace = new DebugConsoleTrace(_console);
            _console.IsOpen = true;

            UnityMainThreadDispatcher.RegisterUnhandledExceptionCallback(ex => 
                _trace.Error("fatal", ex, "Unhandled exception"));
        }

        #endregion
    }
}
