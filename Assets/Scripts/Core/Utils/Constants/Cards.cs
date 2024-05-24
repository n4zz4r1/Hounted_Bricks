using System;
using System.Diagnostics.CodeAnalysis;

namespace Core.Utils.Constants {

[Serializable]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum Card {
    NONE,
    Card_001_Crooked_Rock,
    Card_002_Rounded_Rock,
    Card_003_Arrowed_Rock,
    Card_004_Bomb_Rock,
    Card_005_Char_Lucas,
    Card_006_Char_Lisa,
    Card_007_Char_Bill,
    Card_008_Special_Char_Willy,
    Card_009_Special_Char_Brutus,

    Card_010_Basic_Move,
    Card_011_Basic_SandBag,
    Card_012_Basic_SuperAim,
    Card_013_Basic_Lucas,
    Card_014_Basic_Lisa,
    Card_015_Basic_Billy,

    // Lucas Abilities
    Card_016_Ab_Lucas_Move,
    Card_017_Ab_Lucas_SuperStone,
    Card_018_Ab_Lucas_SlingshotMater,
    Card_019_Ab_Lucas_SmallRock,
    Card_020_Ab_Lucas_Power,
    Card_021_Ab_Lucas_RockRain,
    Card_022_Ab_Lucas_SuperWall,
    Card_023_Ab_Lucas_LetsRock,
    Card_024_Ab_Lucas_Mine,
    Card_025_Ab_Lucas_GiveMeMoney,

    // Lisa Abilities
    Card_026_Ab_Lisa_Fire,
    Card_027_Ab_Lisa_FireByLucky,

    // ReSharper disable once IdentifierTypo
    Card_028_Ab_Lisa_FireByRebounce,
    Card_029_Ab_Lisa_Healer,
    Card_030_Ab_Lisa_Firewall,
    Card_031_Ab_Lisa_FireCamp,
    Card_032_Ab_Lisa_FireStronger,
    Card_033_Ab_Lisa_FireSpread,
    Card_034_Ab_Lisa_Fireworks,
    Card_035_Ab_Lisa_SpecialFireBomb,

    // Bill Cards
    Card_036_Ab_Bill_AcidBomb,
    Card_037_Ab_Bill_Poison,
    Card_038_Ab_Bill_PoisonByLucky,
    Card_039_Ab_Bill_PoisonByLuckTwo,
    Card_040_Ab_Bill_Barrel,
    Card_041_Ab_Bill_Run,
    Card_042_Ab_Bill_UltraAim,
    Card_043_Ab_Bill_AcidRain,
    Card_044_Ab_Bill_SuperPoison,

    // ReSharper disable once IdentifierTypo
    Card_045_Ab_Bill_Viruz

    // Basic Cards
}

}