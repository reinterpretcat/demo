using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.MapEditor
{
    internal sealed class TerrainPolygonMessage
    {
        public readonly List<Vector3> Polygon;

        public TerrainPolygonMessage(List<Vector3> polygon)
        {
            Polygon = polygon;
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
}
