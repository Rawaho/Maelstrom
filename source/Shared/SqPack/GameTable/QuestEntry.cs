namespace Shared.SqPack.GameTable
{
    public class QuestEntry : Exd.Entry
    {
        public string Name => (string)Data[0];
    }
}
