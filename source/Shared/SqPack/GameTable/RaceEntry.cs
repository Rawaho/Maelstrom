namespace Shared.SqPack.GameTable
{
    public class RaceEntry : Exd.Entry
    {
        public string MaleName => (string)Data[0];
        public string FemaleName => (string)Data[1];
        public int MaleBodyItemId => (int)Data[2];
        public int MaleHandsItemId => (int)Data[3];
        public int MaleLegsItemId => (int)Data[4];
        public int MaleFeetItemId => (int)Data[5];
        public int FemaleBodyItemId => (int)Data[6];
        public int FemaleHandsItemId => (int)Data[7];
        public int FemaleLegsItemId => (int)Data[8];
        public int FemaleFeetItemId => (int)Data[9];
    }
}
