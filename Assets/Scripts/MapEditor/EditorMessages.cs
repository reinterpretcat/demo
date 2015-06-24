using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Editor
{
    internal sealed class TerrainPolygonMessage
    {
        public readonly List<Vector3> Polygon;

        public TerrainPolygonMessage(List<Vector3> polygon)
        {
            Polygon = polygon;
        }
    }
}
