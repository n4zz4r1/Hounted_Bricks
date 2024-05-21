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
    Card_010_Special_Small_Diamond_Pack,
    Card_011_Special_Big_Diamond_Pack,
    Card_012_Improved_Crooked_Rock,
    Card_013_Improved_Rounded_Rock,
    Card_014_Improved_Arrowed_Rock,
    Card_015_Improved_Bomb_Rock,

    // Lucas Abilities
    Card_016_Ab_Lucas_Move,
    Card_017_Ab_Lucas_SandBag,
    Card_018_Ab_Lucas_MoreRockPlease,
    Card_019_Ab_Lucas_RockParty,
    Card_020_Ab_Lucas_NoPainNoGain,
    Card_021_Ab_Lucas_MoreLovePlease,
    Card_022_Ab_Lucas_PerfectAim,
    Card_023_Ab_Lucas_LetsRock,
    Card_024_Ab_Lucas_Mine,
    Card_025_Ab_Lucas_GiveMeMoney,

    // Lisa Abilities
    Card_026_Ab_Lisa_FireByTurn,
    Card_027_Ab_Lisa_FireByLucky,

    // ReSharper disable once IdentifierTypo
    Card_028_Ab_Lisa_FireByRebounce,
    Card_029_Ab_Lisa_Healer,
    Card_030_Ab_Lisa_Firewall,
    Card_031_Ab_Lisa_FireByLuckyTwo,
    Card_032_Ab_Lisa_FireStronger,
    Card_033_Ab_Lisa_FireSpread,
    Card_034_Ab_Lisa_Fireworks,
    Card_035_Ab_Lisa_SpecialFireBomb,

    // Bill Cards
    Card_036_Ab_Bill_AcidBomb,
    Card_037_Ab_Bill_PoisonByLucky,
    Card_038_Ab_Bill_PoisonByTurn,
    Card_039_Ab_Bill_PoisonByTimes,
    Card_040_Ab_Bill_PoisonByLuckyTwo,
    Card_041_Ab_Bill_PerfectShoes,
    Card_042_Ab_Bill_UltraAim,
    Card_043_Ab_Bill_AcidRain,
    Card_044_Ab_Bill_SuperPoison,

    // ReSharper disable once IdentifierTypo
    Card_045_Ab_Bill_Viruz,

    // Basic Cards
    Card_046_Basic_Move,
    Card_047_Basic_SandBag,
    Card_048_Basic_SuperAim
}

}