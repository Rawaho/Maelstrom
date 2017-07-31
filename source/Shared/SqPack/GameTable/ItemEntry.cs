namespace Shared.SqPack.GameTable
{
    public class ItemEntry : Exd.Entry
    {
        public string Name => (string)Data[0];
        public string NamePlural => (string)Data[2];
        public string Description => (string)Data[8];
        public string ToolTipName => (string)Data[9];
        public byte UiCategoryId => (byte)Data[15];
        public byte EquipSlotCategoryId => (byte)Data[17];
        public uint MaxStackCount => (uint)Data[19];
        public ulong ModelPrimary => (ulong)Data[45];
        public ulong ModelSecondary => (ulong)Data[46];
    }
}
