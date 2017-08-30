using SaintCoinach.Xiv;

namespace WorldServer.Game.Map
{
    public class Territory : BaseMap
    {
        public TerritoryType Entry { get; }
        public byte Weather { get; private set; }

        public Territory(TerritoryType entry)
        {
            Entry = entry;
        }
    }
}
