namespace Shared.SqPack.GameTable
{
    public class TerritoryTypeEntry : Exd.Entry
    {
        public string Name => (string)Data[0];
        public string Directory => (string)Data[1];
        public ushort PlaceNameId => (ushort)Data[5];
        public byte Type => (byte)Data[8]; // TerritoryIntendedUse??
    }
}
