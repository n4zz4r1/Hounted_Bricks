using System;

namespace Core.Utils.Constants {


[Serializable]
public enum UI {
    None,
    BuffArea,
    ResourceArea,
    ResourceIcon,
}

[Serializable]
public enum Buff {
    None,
    PlayerDamage,
    StoneDivided,
    LucasMultiStrike,
}

[Serializable]
public enum BuffType {
    None,
    StoneCollision,
    StoneLaunch,
}

[Serializable]
public enum Sprites {
    Consumable
}
//
// [Serializable]
// public enum GameResource {
//     Heart = 0,
//     Drop = 1,
//     Coin = 2,
//     Diamond = 3,
// }
//
// [Serializable]
// public enum GameResourceType {
//     Heart = GameResource.Heart,
//     Drop = GameResource.Drop,
//     Coin = GameResource.Coin,
// }

}