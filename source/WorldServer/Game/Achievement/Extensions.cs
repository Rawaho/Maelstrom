namespace WorldServer.Game.Achievement
{
    public static class Extensions
    {
        public static int GetCriteriaParameter(this SaintCoinach.Xiv.Achievement entry, CriteriaParameter parameter)
        {
            return entry.Data[(int)parameter];
        }
    }
}
