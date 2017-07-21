using System;
using System.Data;
using System.Numerics;
using Shared.Database;
using Shared.Database.Datacentre;

namespace Shared.Game
{
    public class WorldPosition
    {
        public ushort TerritoryId { get; private set; }
        public Vector3 Offset { get; private set; }
        public float Orientation { get; private set; }

        public Vector3 PackedOffsetShort => new Vector3(0x8000 + Offset.X * 32.767f, 0x8000 + Offset.Y * 32.767f, 0x8000 + Offset.Z * 32.767f);
        public byte PackedOrientationByte => (byte)(0x80 * (Orientation + Math.PI) / Math.PI);
        public ushort PackedOrientationShort => (ushort)(0x8000 * (Orientation + Math.PI) / Math.PI);

        public WorldPosition(ushort territoryId, Vector3 offset, float orientation)
        {
            TerritoryId = territoryId;
            Offset      = offset;
            Orientation = orientation;
        }

        public WorldPosition(DataRow row)
        {
            TerritoryId = row.Read<ushort>("territoryId");
            Offset      = new Vector3(row.Read<float>("x"), row.Read<float>("y"), row.Read<float>("z"));
            Orientation = row.Read<float>("o");
        }

        public void SaveToDatabase(ulong id, DatabaseTransaction transaction)
        {
            transaction.AddPreparedStatement(DataCentreDatabase.DataCentrePreparedStatement.CharacterPositionInsert,
                id, TerritoryId, Offset.X, Offset.Y, Offset.Z, Orientation);
        }

        public void Relocate(WorldPosition worldPosition)
        {
            TerritoryId = worldPosition.TerritoryId;
            Relocate(worldPosition.Offset, worldPosition.Orientation);
        }

        public void Relocate(Vector3 position, float orientation)
        {
            Offset      = position;
            Orientation = orientation;
        }

        public bool InRadius(WorldPosition position, float radius)
        {
            float dx = Math.Abs(Offset.X - position.Offset.X);
            if (dx > radius)
                return false;

            float dy = Math.Abs(Offset.Y - position.Offset.Y);
            if (dy > radius)
                return false;

            if (dx + dy <= radius)
                return true;

            return dx * dx + dy * dy <= radius * radius;
        }
    }
}
