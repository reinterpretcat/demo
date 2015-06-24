using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.MapEditor
{
    internal sealed class TerrainPointMessage
    {
         public readonly Vector3 Point;

         public TerrainPointMessage(Vector3 point)
        {
            Point = point;
        }
    }

    internal sealed class TerrainPolylineMessage
    {
        public readonly List<Vector3> Polyline;

        public TerrainPolylineMessage(List<Vector3> polygon)
        {
            Polyline = polygon;
        }
    }

    internal sealed class TerrainPolygonMessage
    {
        public readonly List<Vector3> Polygon;

        public TerrainPolygonMessage(List<Vector3> polygon)
        {
            Polygon = polygon;
        }
    }
}
