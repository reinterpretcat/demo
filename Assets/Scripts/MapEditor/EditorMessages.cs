using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.MapEditor
{
    internal abstract class MultiplayerMessage : MessageBase
    {
        public EditorActionMode ActionMode;
        public abstract short Id { get; }

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write((int)ActionMode);
        }

        public override void Deserialize(NetworkReader reader)
        {
            ActionMode = (EditorActionMode) reader.ReadInt32();
        }
    }

    internal sealed class TerrainPointMessage : MultiplayerMessage
    {
        public const short MsgId = 1000;
        public Vector3 Point;
        public override short Id { get { return MsgId; } }

        public override void Serialize(NetworkWriter writer)
        {
            base.Serialize(writer);
            writer.Write(Point);
        }

        public override void Deserialize(NetworkReader reader)
        {
            base.Deserialize(reader);
            Point = reader.ReadVector3();
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
