using System;
using System.Diagnostics.CodeAnalysis;

namespace Core.Utils.Constants {

[Serializable]
public enum RockPile {
    None,
    Basic,
    GoldMine,
}

public class Monsters {
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum Monster {
        M01_Ghost_Green,
        M01_Ghost_Yellow,
        M01_Ghost_Blue,
        M01_Ghost_Red,
        M02_Shield_Blue,
        M02_Shield_Green,
        M02_Shield_Red,
        M02_Shield_Yellow,

        M03_Strong_Blue,
        M03_Strong_Green,
        M03_Strong_Red,
        M03_Strong_Yellow,
        M04_Fast_Blue,
        M04_Fast_Green,
        M04_Fast_Red,
        M04_Fast_Yellow,
        M05_Corner_Green01,
        M05_Corner_Green02,
        M05_Corner_Green03,
        M05_Corner_Yellow01,
        M05_Corner_Yellow02,
        M05_Corner_Yellow03,
        M05_Corner_Blue01,
        M05_Corner_Blue02,
        M05_Corner_Blue03,
        M05_Corner_Red01,
        M05_Corner_Red02,

        M05_Corner_Red03
        // M06_Shaman_Blue,
        // M06_Shaman_Green,
        // M06_Shaman_Red,
        // M06_Shaman_Yellow,
        // M07_Mage_Blue,
        // M07_Mage_Green,
        // M07_Mage_Red,
        // M07_Mage_Yellow,
        // M08_Skeleton,
        // ME_Chest,
        // ME_Chest_Video,
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum MonsterBoss {
        NONE,
        B01_Mini_Green,
        B02_Garden,
        B03_LivingRoom,
        B04_Kitchen,
        B05_Bathroom,
        B06_BedRoom,
        B07_Crypt
    }
}
}