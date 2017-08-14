namespace Shared.SqPack.GameTable
{
    public class OpeningEntry : Exd.Entry
    {
        public string Name => (string)Data[0];
        public uint NextEventId => (uint)Data[1];
    }
}
