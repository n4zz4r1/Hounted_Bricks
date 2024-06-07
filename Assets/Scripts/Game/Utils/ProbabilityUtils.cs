using System;
using UnityEngine;
using Random = System.Random;

namespace Game.Utils {
public class ProbabilityUtils : SingletonBase<ProbabilityUtils> {
    private readonly Random rnd = new();

    public int GenerateProbabilityIndex(int[] prob) {
        var random = rnd.Next(1, 101); // probabilities is aways between 1 and 100
        var percentage = 0;

        for (var i = 0; i < prob.Length; i++) {
            percentage += prob[i];

            if (percentage >= random) return i;
        }

        throw new ProbabilityException();
    }

    public CornerTypeModel RandomCornerPosition() {
        var random = rnd.Next(1, 5); // probabilities is aways between 1 and 4
        switch (random) {
            case 1: return new CornerTypeModel(90f, 1f, 0f); // CornerTypeModel.TOP_LEFT;
            case 2: return new CornerTypeModel(0f, 0f, 0f); // CornerTypeModel.TOP_RIGHT;
            case 3: return new CornerTypeModel(180f, 1f, 1f); // CornerTypeModel.BOTTOM_LEFT;
            case 4: return new CornerTypeModel(270f, 0f, 1f); // CornerTypeModel.BOTTOM_RIGHT;
        }

        return null;
    }

    public bool Between100(int chance) {
        if (chance == 0) return false;

        var randomNumber = rnd.Next(1, 100);
        return chance >= randomNumber;
    }

    public int Dice(int max) {
        var next = rnd.Next(0, max);
        return next;
    }

    public int Dice(int min, int max) {
        var next = rnd.Next(min, max);
        Debug.Log("Next, " + next + " from " + min + " to " + max);
        return next;
    }
}

public class CornerTypeModel {
    public static CornerTypeModel TOP_RIGHT = new(0, 0, 0);
    public static CornerTypeModel TOP_LEFT = new(90f, 1f, 0);
    public static CornerTypeModel BOTTOM_LEFT = new(180f, 1f, 1f);
    public static CornerTypeModel BOTTOM_RIGHT = new(270f, 0f, 1f);
    internal float radius;
    internal float x;
    internal float y;

    public CornerTypeModel(float radius, float x, float y) {
        this.radius = radius;
        this.x = x;
        this.y = y;
    }
}

public class ProbabilityException : Exception { }

// [SuppressMessage("ReSharper", "InconsistentNaming")]
// public abstract class StageProbabilities
// {
//     // Stage Level constants 
//     // Max level always 100
//     public static int MAX_LEVEL = 100;
//     // This is the monster factor strength, multiply per level.
//     public static float F_MONSTER = 0.05f;
//
//     // Factor for medals, based on monster quantity
//     public static float F_MEDAL_BRONZE = 1f;
//     public static float F_MEDAL_SILVER = 1.6f;
//     public static float F_MEDAL_GOLD = 2f;
//
//     // Score factor based on monsters quantity divided by total of waves
//     public static float F_SCORE = 1f;
//
//     // Factor: gold related to totalForce
//     public static float F_RES_GOLD = 0.01f;
//     // Factor: diamond related to gold
//     public static float F_RES_DIAMOND = 0.1f;
//
//     // Aura factor will earn based on gold factor and monster force, multiply by:
//     public static float F_RES_GOLD_AURA = 5f;
//     public static float F_RES_DIAMOND_AURA = 5f;
//
//     // Factor: Gold inside chest
//     public static float F_REWARD_COIN_GOLD = .2f;
//     public static float F_REWARD_COIN_SILVER = .6f;
//     public static float F_REWARD_COIN_BRONZE = .4f;
//     public static float F_REWARD_COIN_CHEST = .4f;
//
//     public static float F_REWARD_DIAMOND_GOLD = .8f;
//     public static float F_REWARD_DIAMOND_SILVER = .2f;
//     public static float F_REWARD_DIAMOND_CHEST = .2f;
//
//     // Chance in percentage to find chest 
//     public static int F_FINDING_CHEST_CHANCE = 20;
//     public static int F_FINDING_CHEST_VIDEO_CHANCE = 80;
//
//     // Factor: probability to find monsters with aura 
//     public static int F_MONSTER_WITH_AURA_GOLD = 5;
//     public static int F_MONSTER_WITH_AURA_DIAMOND = 1;
// }
}