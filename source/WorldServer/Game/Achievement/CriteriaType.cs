namespace WorldServer.Game.Achievement
{
    public enum CriteriaType : byte
    {
        None                    = 0,
        Counter                 = 1, // see CriteriaCounterType
        Achievement             = 2,
        Level                   = 3,
        MateriaMelding          = 4,
        // 5
        // 6
        HuntingLog              = 7,
        Exploration             = 8,
        QuestCompleteAny        = 9,
        ChocoboRank             = 10,
        PvPRank                 = 11,
        PvPMatch                = 12,
        PvPMatchWin             = 13,
        // 14
        ReputationRank          = 15,
        FrontlineCampaign       = 17,
        FrontlineCampaignWin    = 18,
        FrontlineCampaignWinAny = 19,
        AetherCurrent           = 20,
        Minion                  = 21,
        VerminionChallenge      = 23,
        AnimaWeapon             = 24
    }
}
