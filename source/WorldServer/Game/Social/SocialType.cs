namespace WorldServer.Game.Social
{
    public enum SocialType
    {
        None                = 0,
        Party               = 1,
        Friend              = 2,
        FreeCompanyPetition = 4,
        FreeCompany         = 5,
        NoviceNetwork       = 8,
        Party2              = 9  // calls the same client function as Party but never seen it sent
    }
}
