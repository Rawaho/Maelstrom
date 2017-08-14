namespace Shared.Network
{
    public enum SubPacketOpcode : ushort
    {
        None                           = 0x0000,

        // client->server
        ClientCharacterList            = 0x0003,
        ClientEnterWorld               = 0x0004,
        ClientLobbyRequest             = 0x0005,
        ClientCharacterDelete          = 0x000A,
        ClientCharacterCreate          = 0x000B,
        ClientTerritoryFinalise        = 0x0069,
        ClientLogout                   = 0x0074,
        ClientContentFinderRequestInfo = 0x0078,
        ClientActorAction              = 0x0108,
        ClientGmCommandInt             = 0x010C,
        ClientGmCommandString          = 0x010D,
        ClientPlayerMove               = 0x010F,
        ClientInventoryAction          = 0x0116,

        ClientEventGossip              = 0x011F,
        ClientEventEmote               = 0x0120,
        ClientEventAreaTrigger         = 0x0121,
        ClientEventOutOfBounds         = 0x0122,
        ClientEventTerritory           = 0x0123,
        ClientEventSceneFinish         = 0x0128,

        // server->client
        ServerError                    = 0x0002,
        ServerServiceAccountList       = 0x000C,
        ServerCharacterList            = 0x000D,
        ServerCharacterCreate          = 0x000E,
        ServerEnterWorld               = 0x000F,
        ServerRealmList                = 0x0015,
        ServerRetainerList             = 0x0017,
        ServerLogout                   = 0x0077,
        
        ServerContentFinderNotify       = 0x0078,
        ServerContentFinderMemberStatus = 0x0079,
        ServerContentFinderPlayerInNeed = 0x007F,
        ServerContentFinderDutyInfo     = 0x007A,
        ServerContentFinderRegister     = 0x00B0,
        
        ServerMessage                  = 0x00BC,
        ServerPlayerSpawn              = 0x0110,
        ServerActorMove                = 0x0112,
        ServerPlayerSetup              = 0x011E,
        ServerPlayerStateFlags         = 0x0121,
        ServerClassSetup               = 0x0123,
        ServerActorAppearanceUpdate    = 0x0124,
        ServerItemSetup                = 0x0139,
        ServerContainerSetup           = 0x013A,
        ServerInventoryUpdateFinish    = 0x013B,
        ServerInventoryUpdate          = 0x013C,
        ServerItemUpdate               = 0x0140,
        // all 3 invoke the same client function just with different parameters
        ServerActorAction1             = 0x0142,
        ServerActorAction2             = 0x0143,
        ServerActorAction3             = 0x0144,

        ServerEventSceneStart          = 0x0154,
        /* also call the same client function as ServerEventSceneStart
        0x0155,
        0x0156,
        0x0157,
        0x0158,
        0x0159,
        0x015A,
        0x015B,*/
        ServerEventStart               = 0x015D,
        ServerEventStop                = 0x015E,

        ServerQuestJournalActiveList   = 0x0171,
        ServerQuestJournalCompleteList = 0x0173,
        ServerTerritorySetup           = 0x019A,
        ServerAchievementList          = 0x01CB,
        
        ServerContentFinderList        = 0x01CF,
        
        ServerTerritoryPending         = 0x0239,

        ServerUnknown01FB              = 0x01FB,
        ServerUnknown01FD              = 0x01FD,

        // bidirectional
        NewWorld                       = 0x0066,
        Chat                           = 0x0067,
    }
}
