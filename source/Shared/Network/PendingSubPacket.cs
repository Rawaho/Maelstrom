namespace Shared.Network
{
    public struct PendingSubPacket
    {
        public SubPacket SubPacket { get; }
        public uint Source { get; }
        public uint Target { get; }

        public PendingSubPacket(SubPacket subPacket, uint source, uint target)
        {
            SubPacket = subPacket;
            Source    = source;
            Target    = target;
        }
    }
}
