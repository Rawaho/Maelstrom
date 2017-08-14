using Shared.SqPack.GameTable;

namespace WorldServer.Game.Map
{
    public class Territory : BaseMap
    {
        public TerritoryTypeEntry Entry { get; }
        public byte Weather { get; private set; }

        public Territory(TerritoryTypeEntry entry)
        {
            Entry = entry;
        }
    }
}
