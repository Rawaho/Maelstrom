namespace Shared.SqPack.GameTable
{
    public class PlaceNameEntry : Exd.Entry
    {
        public string Name => (string)Data[0];
    }
}
