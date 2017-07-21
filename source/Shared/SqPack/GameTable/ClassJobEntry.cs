namespace Shared.SqPack.GameTable
{
    public class ClassJobEntry : Exd.Entry
    {
        public string Name => (string)Data[0];
        public string Abbreviation => (string)Data[1];
        public sbyte ClassId => (sbyte)Data[4];
        public byte CityState => (byte)Data[31];
    }
}
