using System;
using System.Collections.Generic;
using System.Linq;
using Core.Data;
using Core.Utils.Constants;

namespace Core.Utils {

public abstract class Dices {
    public static List<int> GenerateRandomList(int numberOfElements) {
        // Create a list containing numbers from 0 to numberOfElements
        var list = new List<int>();
        for (var i = 0; i <= numberOfElements; i++) list.Add(i);

        // Randomly shuffle the list using Fisher-Yates shuffle
        var rng = new Random();
        var n = list.Count;
        while (n > 1) {
            n--;
            var k = rng.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }

        return list;
    }

    public static int RollDiceFromRewardType(ResourceType type) {
        switch (type) {
            case ResourceType.COIN:
                return (int)(GameDataV1.Instance.level * CoinQuantityDice.Roll());
            case ResourceType.DIAMOND:
                return (int)(GameDataV1.Instance.level * DiamontQuantityDice.Roll());
                ;
            case ResourceType.CHEST:
                return 1; // create dice for chests
            case ResourceType.CARD:
                return 1;
            case ResourceType.NONE:
                return 0;
            // TODO
            case ResourceType.ROCK_SCROLL:
            case ResourceType.CHAR_SCROLL:
            case ResourceType.ABILITY_SCROLL:
            case ResourceType.CHEST_KEYS:
            case ResourceType.MONEY:
            default:
                return 0;
        }
    }
}

[Serializable]
public class NormalRewardDice : AbstractDice<NormalRewardDice, ResourceType> {
    public override Factor<ResourceType>[] GetProbabilities => new[] {
        new Factor<ResourceType>(ResourceType.COIN, 75f),
        new Factor<ResourceType>(ResourceType.DIAMOND, 20f),
        new Factor<ResourceType>(ResourceType.CARD, 5f)
    };
}

[Serializable]
public class LowRewardDice : AbstractDice<LowRewardDice, ResourceType> {
    public override Factor<ResourceType>[] GetProbabilities => new[] {
        new Factor<ResourceType>(ResourceType.COIN, 32f),
        new Factor<ResourceType>(ResourceType.DIAMOND, 6f),
        new Factor<ResourceType>(ResourceType.CARD, 2f),
        new Factor<ResourceType>(ResourceType.NONE, 60f)
    };
}

[Serializable]
public class HighRewardDice : AbstractDice<HighRewardDice, ResourceType> {
    public override Factor<ResourceType>[] GetProbabilities => new[] {
        new Factor<ResourceType>(ResourceType.COIN, 60f),
        new Factor<ResourceType>(ResourceType.DIAMOND, 30f),
        new Factor<ResourceType>(ResourceType.CARD, 10f)
    };
}

[Serializable]
public class CoinQuantityDice : AbstractDice<CoinQuantityDice, double> {
    public override Factor<double>[] GetProbabilities => new[] {
        new Factor<double>(1.3d, 1f),
        new Factor<double>(1.25d, 2f),
        new Factor<double>(1.2d, 3f),
        new Factor<double>(1.15d, 4f),
        new Factor<double>(1.1d, 5f),
        new Factor<double>(0.9d, 5f),
        new Factor<double>(0.85d, 4f),
        new Factor<double>(0.80d, 3f),
        new Factor<double>(0.75d, 2f),
        new Factor<double>(0.70d, 1f)
    };
}

[Serializable]
public class DiamontQuantityDice : AbstractDice<DiamontQuantityDice, double> {
    public override Factor<double>[] GetProbabilities => new[] {
        new Factor<double>(1.15d, 4f),
        new Factor<double>(1.1d, 5f),
        new Factor<double>(0.9d, 5f),
        new Factor<double>(0.85d, 4f)
    };
}


[Serializable]
public class CardDice : AbstractDice<CardDice, Card> {
    private static readonly float LEGENDARY = 1;
    private static readonly float RARE = 2;
    private static readonly float COMMON = 5;
    private static readonly float UNCOMMON = 4;

    public override Factor<Card>[] GetProbabilities => new List<Factor<Card>> {
        new(Card.Card_001_Crooked_Rock, 60f),
        new(Card.Card_002_Rounded_Rock, 40f),
        new(Card.Card_003_Arrowed_Rock, 25f),
        new(Card.Card_004_Bomb_Rock, 10f),

        new(Card.Card_016_Ab_Lucas_Move, COMMON),
        new(Card.Card_017_Ab_Lucas_SandBag, COMMON),
        new(Card.Card_018_Ab_Lucas_MoreRockPlease, UNCOMMON),
        new(Card.Card_019_Ab_Lucas_RockParty, UNCOMMON),
        new(Card.Card_020_Ab_Lucas_NoPainNoGain, UNCOMMON),
        new(Card.Card_021_Ab_Lucas_MoreLovePlease, RARE),
        new(Card.Card_022_Ab_Lucas_PerfectAim, RARE),
        new(Card.Card_023_Ab_Lucas_LetsRock, LEGENDARY),
        new(Card.Card_024_Ab_Lucas_Mine, LEGENDARY),
        new(Card.Card_025_Ab_Lucas_GiveMeMoney, LEGENDARY),

        new(Card.Card_026_Ab_Lisa_FireByTurn, COMMON),
        new(Card.Card_027_Ab_Lisa_FireByLucky, COMMON),
        new(Card.Card_028_Ab_Lisa_FireByRebounce, COMMON),
        new(Card.Card_029_Ab_Lisa_Healer, UNCOMMON),
        new(Card.Card_030_Ab_Lisa_Firewall, UNCOMMON),
        new(Card.Card_031_Ab_Lisa_FireByLuckyTwo, RARE),
        new(Card.Card_032_Ab_Lisa_FireStronger, RARE),
        new(Card.Card_033_Ab_Lisa_FireSpread, RARE),
        new(Card.Card_034_Ab_Lisa_Fireworks, LEGENDARY),
        new(Card.Card_035_Ab_Lisa_SpecialFireBomb, LEGENDARY),

        new(Card.Card_036_Ab_Bill_AcidBomb, COMMON),
        new(Card.Card_037_Ab_Bill_PoisonByLucky, UNCOMMON),
        new(Card.Card_038_Ab_Bill_PoisonByTurn, UNCOMMON),
        new(Card.Card_039_Ab_Bill_PoisonByTimes, UNCOMMON),
        new(Card.Card_040_Ab_Bill_PoisonByLuckyTwo, RARE),
        new(Card.Card_041_Ab_Bill_PerfectShoes, RARE),
        new(Card.Card_042_Ab_Bill_UltraAim, RARE),
        new(Card.Card_043_Ab_Bill_AcidRain, LEGENDARY),
        new(Card.Card_044_Ab_Bill_SuperPoison, LEGENDARY),
        new(Card.Card_045_Ab_Bill_Viruz, LEGENDARY),

        new(Card.Card_012_Improved_Crooked_Rock, UNCOMMON),
        new(Card.Card_013_Improved_Rounded_Rock, UNCOMMON),
        new(Card.Card_014_Improved_Arrowed_Rock, UNCOMMON)
        // new(Card.Card_015_Improved_Bomb_Rock, UNCOMMON),
    }.Where(card => !CardsDataV1.Instance.HaveAllCardsFrom(card.Value)).ToArray();
}

}