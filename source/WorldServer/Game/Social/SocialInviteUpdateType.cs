namespace WorldServer.Game.Social
{
    public enum SocialInviteUpdateType
    {
        None     = 0,
        Invite   = 1,
        Canceled = 2,
        // TODO: 3, 4 have handlers in client
        Declined = 5
    }
}
