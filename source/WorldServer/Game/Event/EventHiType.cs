namespace WorldServer.Game.Event
{
    public enum EventHiType : ushort
    {
        Quest               = 0x0001,
        GilShop             = 0x0004,
        DefaultGossip       = 0x0009,
        CustomGossip        = 0x000B,
        Opening             = 0x0013,
        SpecialShop         = 0x001B,
        SwitchGossip        = 0x001F,
        FccShop             = 0x002A,
        LotteryExchangeShop = 0x0034,
        DisposalShop        = 0x0035
    }
}
