using System;
using System.Linq;
using Core.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.StateMachine.Cards {

[Serializable]
public class CardAbilityComponent {
    [FormerlySerializedAs("AbilityType")] public AbilityType abilityType = AbilityType.NONE;
    [FormerlySerializedAs("Value")] public int value;
}

[Serializable]
public class CardAttributesComponent {
    [SerializeField] public CardAttribute attribute;
    [SerializeField] public StringStringDictionary updates;

    // Concat all values and bring current level value
    public float ConcatValue(int level) {
        if (updates.Count == 0)
            return -1;

        return updates.Where(keypair => int.Parse(keypair.Key) <= level)
            .Aggregate(0f, (current, keypair) => current + float.Parse(keypair.Value));
    }

    public float ValueOfLevel(int level) {
        return updates.ContainsKey(level.ToString()) ? float.Parse(updates[level.ToString()]) : 0f;
    }
}

[Serializable]
public enum CardAttribute {
    POWER = 0,
    RESISTANCE = 1,
    HEALTH = 2,
    AIM = 3,
    MAGIC = 4,
    CONSUMABLE = 5,
    PROBABILITY = 6,
    RARITY = 7,
    GEM_SLOT = 8,
    RANGE = 9,
    QUANTITY = 10,
}

[Serializable]
public enum CardType {
    NONE = 0,
    ROCK = 1,
    CHARACTER = 2,
    SPECIAL = 3,
    BASIC_ABILITY = 4,
    PASSIVE_ABILITY = 5,
    ACTIVE_ABILITY = 6
}

public abstract class CardTypeUtils {
    public static ResourceType ToResource(CardType cardType) {
        return cardType switch {
            CardType.NONE => ResourceType.NONE,
            CardType.ROCK => ResourceType.ROCK_SCROLL,
            CardType.BASIC_ABILITY => ResourceType.ABILITY_SCROLL,
            CardType.ACTIVE_ABILITY => ResourceType.ABILITY_SCROLL,
            CardType.PASSIVE_ABILITY => ResourceType.ABILITY_SCROLL,
            CardType.CHARACTER => ResourceType.CHAR_SCROLL,
            _ => ResourceType.NONE
        };
    }
}

[Serializable]
public enum CardRarity {
    COMMON = 0,
    UNCOMMON = 1,
    RARE = 2,
    LEGENDARY = 3,
    GOLD = 4
}

[Serializable]
public enum AbilityType {
    NONE,
    CONSUME_PER_TURN,
    CONSUME_ONCE,
    IMPROVEMENT
}

}