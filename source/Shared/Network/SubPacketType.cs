namespace Shared.Network
{
    public enum SubPacketType : ushort
    {
        None              = 0,
        ClientHelloWorld  = 1,
        ServerHelloWorld  = 2,
        Message           = 3,
        KeepAliveRequest  = 7,
        KeepAliveResponse = 8,
        ClientHello       = 9,
        ServerHello       = 10
    }
}
