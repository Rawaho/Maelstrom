namespace WorldServer.Game.Social
{
    public class SocialInviteRequest
    {
        public ulong HostId { get; }
        public SocialType Type { get; }
        public uint EntityId { get; }

        public SocialInviteRequest(ulong hostId, SocialType type, uint entityId)
        {
            HostId   = hostId;
            Type     = type;
            EntityId = entityId;
        }
    }
}
