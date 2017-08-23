namespace WorldServer.Game
{
    public enum LogMessage
    {
        None                       = 0,
        PartyInviteCanceled        = 2,

        /// <summary>
        /// That player is already in another party.
        /// </summary>
        PartyInviteInviteeParty    = 326,

        /// <summary>
        /// That player has already been invited.
        /// </summary>
        SocialPartyPending         = 328,

        /// <summary>
        /// Unable to invite. The party is full.
        /// </summary>
        PartyInvitePartyFull       = 329,

        /// <summary>
        /// Unable to promote to party leader. That player is currently offline.
        /// </summary>
        PartyPromoteOffline        = 332
    }
}
