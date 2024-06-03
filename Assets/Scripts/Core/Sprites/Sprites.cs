using System;

namespace Core.Sprites {
    
[Serializable]
public enum CardAttributeType {
    Power = 0,
    Resistance = 1,
    Health = 2,
    Aim = 3,
    Magic = 4,
    Consumable = 5,
    Probability = 6,
    Rarity = 7,
    GemSlot = 8,
    Range = 9,
    Quantity = 10,
}

[Serializable]
public enum ResourceType {
    None = 0,
    Coin = 1,
    Diamond = 2,
    RockScroll = 3,
    CharScroll = 4,
    AbilityScroll = 5,
    Chest = 6,
    ChestKey = 7,
    Card = 8,
    Money = 9,
    MasterKey = 10
}

}