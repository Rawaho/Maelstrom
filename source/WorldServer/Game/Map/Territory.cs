using Shared.SqPack.GameTable;

namespace WorldServer.Game.Map
{
    public class Territory : BaseMap
    {
        public ushort Id { get; }
        public byte Weather { get; private set; }

        public Territory(TerritoryTypeEntry entry)
        {
            Id = (ushort)entry.Index;
        }
    }
}
