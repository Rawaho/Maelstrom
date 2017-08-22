namespace Shared.Network
{
    public enum SubPacketClientOpcode : ushort
    {
        None                                    = 0x0000,
        ClientCharacterList                     = 0x0003,
        ClientEnterWorld                        = 0x0004,
        ClientLobbyRequest                      = 0x0005,
        ClientCharacterDelete                   = 0x000A,
        ClientCharacterCreate                   = 0x000B,
        ClientNewWorld                          = 0x0066,
        ClientChat                              = 0x0067,
        ClientTerritoryFinalise                 = 0x0069,
        ClientLogout                            = 0x0074,
        ClientContentFinderRequestInfo          = 0x0078,
        ClientSocialInvite                      = 0x00A8,
        ClientSocialInviteResponse              = 0x00A9,
        ClientActorAction                       = 0x0108,
        ClientGmCommandInt                      = 0x010C,
        ClientGmCommandString                   = 0x010D,
        ClientPlayerMove                        = 0x010F,
        ClientInventoryAction                   = 0x0116,
        ClientEventGossip                       = 0x011F,
        ClientEventEmote                        = 0x0120,
        ClientEventAreaTrigger                  = 0x0121,
        ClientEventOutOfBounds                  = 0x0122,
        ClientEventTerritory                    = 0x0123,
        ClientEventSceneFinish                  = 0x0128
    }
}
