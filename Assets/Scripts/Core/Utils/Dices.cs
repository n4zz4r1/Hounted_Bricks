﻿using System;
using System.Collections.Generic;
using System.Linq;
using Core.Data;
using Core.Sprites;
using Core.Utils.Constants;
using UnityEngine;
using Random = System.Random;

namespace Core.Utils {
public abstract class Dices {

    private static Random random = new Random();

    // Receive 
    public static bool Roll(int percentual) {
        var result = random.NextDouble();
        return result < (percentual / 100f);
    }
    
    public static bool Roll(float percentual) {
        var result = random.NextDouble();
        return result < (percentual / 100f);
    }

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
            case ResourceType.Coin:
                return (int)(GameDataV1.Instance.level * CoinQuantityDice.Roll());
            case ResourceType.Diamond:
                return (int)(GameDataV1.Instance.level * DiamontQuantityDice.Roll());
            case ResourceType.Chest:
                return 1; // create dice for chests
            case ResourceType.Card:
                return 1;
            case ResourceType.None:
                return 0;
            // TODO
            case ResourceType.RockScroll:
            case ResourceType.CharScroll:
            case ResourceType.AbilityScroll:
            case ResourceType.ChestKey:
            case ResourceType.Money:
            default:
                return 0;
        }
    }
    
    
    public static Vector2 RotateRandomly(Vector2 from, Vector2 target)
    {
        // Generate a random angle in degrees
        float angle = GenerateRandomAngle();

        // Convert angle to radians
        float angleRadians = angle * Mathf.Deg2Rad;

        // Translate target to origin
        Vector2 direction = target - from;

        // Apply rotation
        float cosAngle = Mathf.Cos(angleRadians);
        float sinAngle = Mathf.Sin(angleRadians);
        Vector2 rotatedDirection = new Vector2(
            cosAngle * direction.x - sinAngle * direction.y,
            sinAngle * direction.x + cosAngle * direction.y
        );

        // Translate back to the original position
        Vector2 newTarget = from + rotatedDirection;

        return newTarget;
    }

    private static float GenerateRandomAngle()
    {
        // Generate a random boolean to decide between positive and negative angle ranges
        bool isPositive = random.NextDouble() >= 0.5;

        if (isPositive)
        {
            // Random angle between 25 and 45 degrees
            return (float)random.NextDouble() * 10 + 5;
        }
        else
        {
            // Random angle between -25 and -45 degrees
            return (float)random.NextDouble() * -10 - 5;
        }
    }
}

[Serializable]
public class NormalRewardDice : AbstractDice<NormalRewardDice, ResourceType> {
    public override Factor<ResourceType>[] GetProbabilities => new[] {
        new Factor<ResourceType>(ResourceType.Coin, 75f),
        new Factor<ResourceType>(ResourceType.Diamond, 20f),
        new Factor<ResourceType>(ResourceType.Card, 5f)
    };
}

[Serializable]
public class LowRewardDice : AbstractDice<LowRewardDice, ResourceType> {
    public override Factor<ResourceType>[] GetProbabilities => new[] {
        new Factor<ResourceType>(ResourceType.Coin, 32f),
        new Factor<ResourceType>(ResourceType.Diamond, 6f),
        new Factor<ResourceType>(ResourceType.Card, 2f),
        new Factor<ResourceType>(ResourceType.None, 60f)
    };
}

[Serializable]
public class HighRewardDice : AbstractDice<HighRewardDice, ResourceType> {
    public override Factor<ResourceType>[] GetProbabilities => new[] {
        new Factor<ResourceType>(ResourceType.Coin, 60f),
        new Factor<ResourceType>(ResourceType.Diamond, 30f),
        new Factor<ResourceType>(ResourceType.Card, 10f)
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
        new(Card.Card_017_Ab_Lucas_SuperStone, COMMON),
        new(Card.Card_018_Ab_Lucas_SlingshotMater, UNCOMMON),
        new(Card.Card_019_Ab_Lucas_SmallRock, UNCOMMON),
        new(Card.Card_020_Ab_Lucas_Power, UNCOMMON),
        new(Card.Card_021_Ab_Lucas_RockRain, RARE),
        new(Card.Card_022_Ab_Lucas_SuperWall, RARE),
        new(Card.Card_023_Ab_Lucas_LetsRock, LEGENDARY),
        new(Card.Card_024_Ab_Lucas_Mine, LEGENDARY),
        new(Card.Card_025_Ab_Lucas_GiveMeMoney, LEGENDARY),

        new(Card.Card_026_Ab_Lisa_Fire, COMMON),
        new(Card.Card_027_Ab_Lisa_FireByLucky, COMMON),
        new(Card.Card_028_Ab_Lisa_FireByRebounce, COMMON),
        new(Card.Card_029_Ab_Lisa_Healer, UNCOMMON),
        new(Card.Card_030_Ab_Lisa_Firewall, UNCOMMON),
        new(Card.Card_031_Ab_Lisa_FireCamp, RARE),
        new(Card.Card_032_Ab_Lisa_FireStronger, RARE),
        new(Card.Card_033_Ab_Lisa_FireSpread, RARE),
        new(Card.Card_034_Ab_Lisa_Fireworks, LEGENDARY),
        new(Card.Card_035_Ab_Lisa_SpecialFireBomb, LEGENDARY),

        new(Card.Card_036_Ab_Bill_AcidBomb, COMMON),
        new(Card.Card_037_Ab_Bill_Poison, UNCOMMON),
        new(Card.Card_038_Ab_Bill_PoisonByLucky, UNCOMMON),
        new(Card.Card_039_Ab_Bill_PoisonByLuckyTwo, UNCOMMON),
        new(Card.Card_040_Ab_Bill_Barrel, RARE),
        new(Card.Card_041_Ab_Bill_Run, RARE),
        new(Card.Card_042_Ab_Bill_UltraAim, RARE),
        new(Card.Card_043_Ab_Bill_AcidRain, LEGENDARY),
        new(Card.Card_044_Ab_Bill_SuperPoison, LEGENDARY),
        new(Card.Card_045_Ab_Bill_Viruz, LEGENDARY)

        // new(Card.Card_015_Improved_Bomb_Rock, UNCOMMON),
    }.Where(card => !CardsDataV1.Instance.HaveAllCardsFrom(card.Value)).ToArray();
}
}