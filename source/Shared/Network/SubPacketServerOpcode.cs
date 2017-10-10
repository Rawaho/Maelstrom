namespace Shared.Network
{
    public enum SubPacketServerOpcode : ushort
    {
        None                                    = 0x0000,
        ServerError                             = 0x0002,
        ServerServiceAccountList                = 0x000C,
        ServerCharacterList                     = 0x000D,
        ServerCharacterCreate                   = 0x000E,
        ServerEnterWorld                        = 0x000F,
        ServerRealmList                         = 0x0015,
        ServerRetainerList                      = 0x0017,
        ServerNewWorld                          = 0x0066,
        ServerChat                              = 0x0067,
        ServerLogout                            = 0x0077,
        ServerContentFinderNotify               = 0x0078,
        ServerContentFinderMemberStatus         = 0x0079,
        ServerContentFinderPlayerInNeed         = 0x007F,
        ServerContentFinderDutyInfo             = 0x007A,
        ServerContentFinderRegister             = 0x00B8, // 4.1
        ServerSocialInviteResponse              = 0x00BB, // 4.1
        ServerSocialMessage                     = 0x00BC, // 4.1
        ServerSocialInviteUpdate                = 0x00BD, // 4.1
        ServerSocialList                        = 0x00BE, // 4.1 
        ServerMessage                           = 0x00C6, // 4.1
        ServerSocialAction                      = 0x00CE, // 4.1
        ServerSocialBlacklist                   = 0x00D4, // 4.1
        ServerPlayerSpawn                       = 0x011C, // 4.1
        ServerActorMove                         = 0x011E, // 4.1
        ServerParty                             = 0x0125, // 4.1
        ServerPlayerSetup                       = 0x012B, // 4.1
        ServerPlayerStateFlags                  = 0x013A, // 4.1
        ServerClassSetup                        = 0x013B, // 4.1
        ServerActorAppearanceUpdate             = 0x013C, // 4.1
        ServerItemSetup                         = 0x014C, // 4.1
        ServerContainerSetup                    = 0x014D, // 4.1
        ServerInventoryUpdateFinish             = 0x014E, // 4.1
        ServerInventoryUpdate                   = 0x014F, // 4.1
        ServerItemUpdate                        = 0x0153, // 4.1
        // all 3 invoke the same client function just with different parameters
        ServerActorAction1                      = 0x0142,
        ServerActorAction2                      = 0x0143,
        ServerActorAction3                      = 0x0144,
        ServerEventSceneStart                   = 0x0160, // 4.1
        /* also call the same client function as ServerEventSceneStart
        0x0161,
        0x0162,
        0x0163,
        0x0164,
        0x0165,
        0x0166,
        0x0167,*/
        ServerEventStart                        = 0x0169, // 4.1
        ServerEventStop                         = 0x016A, // 4.1
        ServerQuestJournalActiveList            = 0x017D, // 4.1 
        ServerQuestJournalCompleteList          = 0x017F, // 4.1
        ServerTerritorySetup                    = 0x019A, // 4.1
        ServerAchievementList                   = 0x01D7, // 4.1
        ServerContentFinderList                 = 0x01CF,
        ServerUnknown0207                       = 0x0207, // 4.1
        ServerUnknown0209                       = 0x0209, // 4.1
        ServerTerritoryPending                  = 0x0248  // 4.1
    }
}
