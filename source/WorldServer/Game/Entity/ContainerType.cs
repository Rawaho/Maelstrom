namespace WorldServer.Game.Entity
{
    public enum ContainerType : ushort
    {
        None = ushort.MaxValue,

        // 6 main inventories are mapped to 4 internal at the client (4 * 35 = 140)
        [ContainerType(35)]
        Inventory0         = 0,
        [ContainerType(35)]
        Inventory1         = 1,
        [ContainerType(35)]
        Inventory2         = 2,
        [ContainerType(35)]
        Inventory3         = 3,
        [ContainerType(15)]
        Equipped           = 1000,
        
        [ContainerType(35, ItemUiCategory.Sheild)]
        ArmouryOffHand     = 3200,
        [ContainerType(35, ItemUiCategory.Head)]
        ArmouryHead        = 3201,
        [ContainerType(35, ItemUiCategory.Body)]
        ArmouryBody        = 3202,
        [ContainerType(35, ItemUiCategory.Hands)]
        ArmouryHands       = 3203,
        [ContainerType(35, ItemUiCategory.Waist)]
        ArmouryWaist       = 3204,
        [ContainerType(35, ItemUiCategory.Legs)]
        ArmouryLegs        = 3205,
        [ContainerType(35, ItemUiCategory.Feet)]
        ArmouryFeet        = 3206,
        [ContainerType(35, ItemUiCategory.Necklace)]
        ArmouryNeck        = 3207,
        [ContainerType(35, ItemUiCategory.Earrings)]
        ArmouryEars        = 3208,
        [ContainerType(35, ItemUiCategory.Bracelets)]
        ArmouryWrists      = 3209,
        [ContainerType(35, ItemUiCategory.Ring)]
        ArmouryRings       = 3300,
        [ContainerType(18, ItemUiCategory.SoulCrystal)]
        ArmourySoulCrystal = 3400,
        [ContainerType(35, ItemUiCategory.PugilistArm, ItemUiCategory.GladiatorsArm, ItemUiCategory.MaraudersArm, ItemUiCategory.ArchersArm,
            ItemUiCategory.LancersArm, ItemUiCategory.OneHandedThaumaturgesArm, ItemUiCategory.TwoHandedThaumaturgesArm,
            ItemUiCategory.OneHandedConjurersArm, ItemUiCategory.TwoHandedConjurersArm, ItemUiCategory.ArcanistsGrimoire)]
        ArmouryMainHand    = 3500
    }
}
