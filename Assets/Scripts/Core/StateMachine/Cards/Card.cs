using System;
using System.Linq;
using Core.Sprites;
using Core.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.StateMachine.Cards {

[Serializable]
public class CardAbilityComponent {
    public AbilityType abilityType = AbilityType.NONE;
    public int value;
}

[Serializable]
public class CardAttributesComponent {
    [FormerlySerializedAs("attribute")] [SerializeField] public Sprites.CardAttributeType attributeType;
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
public enum CardType {
    None = 0,
    Rock = 1,
    Character = 2,
    Special = 3,
    Ability = 4
}

public abstract class CardTypeUtils {
    public static ResourceType ToResource(CardType cardType) {
        return cardType switch {
            CardType.None => ResourceType.None,
            CardType.Rock => ResourceType.RockScroll,
            CardType.Ability => ResourceType.AbilityScroll,
            CardType.Character => ResourceType.CharScroll,
            _ => ResourceType.None
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