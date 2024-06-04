using System;
using System.Collections.Generic;
using System.Linq;
using Core.Sprites;
using Core.StateMachine.CardSlots;
using Core.Utils;
using Core.Utils.Constants;
using Framework.Base;
using UnityEngine;

namespace Core.Data {

[Serializable]
public class CardsDataV1 : Data<CardsDataV1> {


    public static int PlayerIndex(Card player) {
        return player switch {
            Card.Card_005_Char_Lucas => 0,
            Card.Card_006_Char_Lisa => 6,
            Card.Card_007_Char_Bill => 12,
            _ => 0
        };
    }

    public void ChangeSavedAbility(Card player, int index, Card ability) {
        savedAbilities[PlayerIndex(player) + index] = ability;
        Save();
    }

    public void RemoveAbilityCardFromIndex(Card player, int index) {
        var playerIndex = PlayerIndex(player);

        if (index != 5) {
            for (var i = playerIndex + index; i < playerIndex + 5; i++) 
                savedAbilities[i] = savedAbilities[i + 1];
        }
        savedAbilities[playerIndex + 5] = Card.NONE;

        Save();
    }

    public Card GetPlayerAbilityAtPosition(Card player, int index) =>
        savedAbilities[PlayerIndex(player) + index];
    public Card GetPlayerAbilityAtPosition(CardSlotFSM cardSlotFSM) =>
        savedAbilities[PlayerIndex(cardSlotFSM.PlayerCard) + cardSlotFSM.index];
    
    public static Dictionary<Card, List<Card>> abilitiesByCharacter = new() {
        {
            Card.Card_005_Char_Lucas, new List<Card> {
                Card.Card_016_Ab_Lucas_Move,
                Card.Card_017_Ab_Lucas_SuperStone,
                Card.Card_018_Ab_Lucas_SlingshotMater,
                Card.Card_019_Ab_Lucas_SmallRock,
                Card.Card_020_Ab_Lucas_Power,
                Card.Card_021_Ab_Lucas_RockRain,
                Card.Card_022_Ab_Lucas_SuperWall,
                Card.Card_023_Ab_Lucas_LetsRock,
                Card.Card_024_Ab_Lucas_Mine,
                Card.Card_025_Ab_Lucas_GiveMeMoney,
            }
        },
        {
            Card.Card_006_Char_Lisa, new List<Card> {
                Card.Card_026_Ab_Lisa_Fire,
                Card.Card_027_Ab_Lisa_FireByLucky,
                Card.Card_028_Ab_Lisa_FireByRebounce,
                Card.Card_029_Ab_Lisa_Healer,
                Card.Card_030_Ab_Lisa_Firewall,
                Card.Card_031_Ab_Lisa_FireCamp,
                Card.Card_032_Ab_Lisa_FireStronger,
                Card.Card_033_Ab_Lisa_FireSpread,
                Card.Card_034_Ab_Lisa_Fireworks,
                Card.Card_035_Ab_Lisa_SpecialFireBomb,
            }
        },
        {
            Card.Card_007_Char_Bill, new List<Card> {
                Card.Card_036_Ab_Bill_AcidBomb,
                Card.Card_037_Ab_Bill_Poison,
                Card.Card_038_Ab_Bill_PoisonByLucky,
                Card.Card_039_Ab_Bill_PoisonByLuckyTwo,
                Card.Card_040_Ab_Bill_Barrel,
                Card.Card_041_Ab_Bill_Run,
                Card.Card_042_Ab_Bill_UltraAim,
                Card.Card_043_Ab_Bill_AcidRain,
                Card.Card_044_Ab_Bill_SuperPoison,
                Card.Card_045_Ab_Bill_Viruz,
            }
        }
    };
    
    public bool AddCard(Card card) {
        try {
            cards.Add(card);
            Save();
            return true;
        }
        catch {
            return false;
        }
    }

    public long GetTotalRocks() {
        return GetCardQuantity(Card.Card_001_Crooked_Rock)
               + GetCardQuantity(Card.Card_002_Rounded_Rock)
               + GetCardQuantity(Card.Card_003_Arrowed_Rock)
               + GetCardQuantity(Card.Card_004_Bomb_Rock);
    }

    public List<Card> GetAllRocksAvailable() {
        return cards.FindAll(card => card is Card.Card_001_Crooked_Rock or Card.Card_002_Rounded_Rock
            or Card.Card_003_Arrowed_Rock or Card.Card_004_Bomb_Rock);
    }

    public int GetCardLevel(Card card) {
        return cardsLevel.Exists(c => c.card.Equals(card)) ? cardsLevel.Find(c => c.card.Equals(card)).level : 1;
    }

    public CardLevel GetCardLevelObject(Card card) {
        CardLevel cardLevel;
        if (cardsLevel.Exists(c => c.card.Equals(card))) {
            cardLevel = cardsLevel.Find(c => c.card.Equals(card));
        }
        else {
            cardsLevel.Add(new CardLevel(card, 1));
            cardLevel = cardsLevel.Find(c => c.card.Equals(card));
        }

        return cardLevel;
    }

    public int GetCardMaxQuantity(Card card) {
        return _cardsMaxQuantity.Exists(c => c.card == card)
            ? _cardsMaxQuantity.Find(c => c.card == card).level
            : 1;
    }

    public bool HaveAllCardsFrom(Card card) {
        var maxQuantity = GetCardMaxQuantity(card);
        var quantity = (int)GetCardQuantity(card);

        return maxQuantity >= 1 && quantity >= maxQuantity;
    }
    //
    // public Card GetParentCard(Card child) {
    //     return _cardRelationship.Exists(c => c.Child == child)
    //         ? _cardRelationship.Find(c => c.Child == child).Parent
    //         : Card.NONE;
    // }
    //
    // public Card GetChildCard(Card parent) {
    //     return _cardRelationship.Exists(c => c.Parent == parent)
    //         ? _cardRelationship.Find(c => c.Parent == parent).Child
    //         : Card.NONE;
    // }

    public bool CardHasLevel(Card card) {
        return cardsLevel.Count(c => c.card.Equals(card)) > 0;
    }

    public long GetCardQuantity(Card card) {
        return cards.Count(c => c == card);
    }

    public long GetCardAvailable(Card card) {
        return GetCardMaxQuantity(card) - GetCardQuantity(card);
    }

    public bool HasCard(Card card) {
        return cards.Exists(c => c == card);
    }

    public bool HasAvailableCards(Card card) {
        return GetCardQuantity(card) < GetCardMaxQuantity(card);
    }

    public List<Card> GetAbilitiesAvailable(Card player) {
        // return abilitiesByCharacter[player].FindAll(c => HasCard(c) && !SavedAbilities[PlayerIndex(player)].Contains(c) );
        return abilitiesByCharacter[player].FindAll(HasCard);
    }

    public bool IncreaseLevel(Card card, ResourceType resourceUpdateType) {
        var success = true;

        Transaction(() => {
            var cardLevel = GetCardLevelObject(card);
            var coinCost = GameMathUtils.GenerateUpdateCostByLevel(cardLevel.level + 1);
            var resourceUpdateCost =
                GameMathUtils.GenerateUpdateCostByLevel(cardLevel.level + 1, resourceUpdateType);

            cardLevel.level++;

            // spend resource
            success = ResourcesV1.Instance.SpendResources(ResourceType.Coin, coinCost, resourceUpdateType,
                resourceUpdateCost);

            // if for some reason there was no money, rollback level
            // TODO Here it's not 100% transactional due to rollback on card level. It is a very rare scenario, but
            //      will be good to check for a definitive, two datas transaction.
            if (!success)
                cardLevel.level--;
        });

        return success;
    }

    #region Properties

    [SerializeField] public List<Card> cards = new() {
        Card.Card_001_Crooked_Rock, Card.Card_001_Crooked_Rock, Card.Card_001_Crooked_Rock,
        Card.Card_001_Crooked_Rock,
        Card.Card_002_Rounded_Rock,
        Card.Card_002_Rounded_Rock,
        Card.Card_003_Arrowed_Rock,
        Card.Card_003_Arrowed_Rock,
        Card.Card_004_Bomb_Rock,

        // Characters
        Card.Card_005_Char_Lucas,
        // TODO remove characters
        Card.Card_006_Char_Lisa,
        Card.Card_007_Char_Bill,

        // Basic Cards
        // TODO remove
        Card.Card_010_Basic_Move,

        Card.Card_011_Basic_SandBag,
        Card.Card_012_Basic_SuperAim,
        Card.Card_013_Basic_Lucas,
        Card.Card_014_Basic_Lisa,
        Card.Card_015_Basic_Billy,
        Card.Card_016_Ab_Lucas_Move,
        Card.Card_017_Ab_Lucas_SuperStone,
        Card.Card_018_Ab_Lucas_SlingshotMater,
        Card.Card_019_Ab_Lucas_SmallRock,
        Card.Card_020_Ab_Lucas_Power,
        Card.Card_021_Ab_Lucas_RockRain,
        Card.Card_022_Ab_Lucas_SuperWall,
        Card.Card_023_Ab_Lucas_LetsRock,
        Card.Card_024_Ab_Lucas_Mine,
        Card.Card_025_Ab_Lucas_GiveMeMoney,
        Card.Card_026_Ab_Lisa_Fire,
        Card.Card_027_Ab_Lisa_FireByLucky,
        Card.Card_028_Ab_Lisa_FireByRebounce,
        Card.Card_029_Ab_Lisa_Healer,
        Card.Card_030_Ab_Lisa_Firewall,
        Card.Card_031_Ab_Lisa_FireCamp,
        Card.Card_032_Ab_Lisa_FireStronger,
        Card.Card_033_Ab_Lisa_FireSpread,
        Card.Card_034_Ab_Lisa_Fireworks,
        // Card.Card_035_Ab_Lisa_SpecialFireBomb,
        Card.Card_036_Ab_Bill_AcidBomb,
        Card.Card_037_Ab_Bill_Poison,
        Card.Card_038_Ab_Bill_PoisonByLucky,
        Card.Card_039_Ab_Bill_PoisonByLuckyTwo,
        Card.Card_040_Ab_Bill_Barrel,
        Card.Card_041_Ab_Bill_Run,
        Card.Card_042_Ab_Bill_UltraAim,
        Card.Card_043_Ab_Bill_AcidRain,
        // Card.Card_044_Ab_Bill_SuperPoison,
        // Card.Card_045_Ab_Bill_Viruz,
        
        // Essencial
        Card.Card_E_Recycle,
        Card.Card_E_NextWave,
        
    };

    [SerializeField] private List<CardLevel> cardsLevel = new();

    [SerializeField] private readonly List<CardLevel> _cardsMaxQuantity = new() {
        new CardLevel(Card.Card_001_Crooked_Rock, 10),
        new CardLevel(Card.Card_002_Rounded_Rock, 10),
        new CardLevel(Card.Card_003_Arrowed_Rock, 10),
        new CardLevel(Card.Card_004_Bomb_Rock, 10)
    };

    [SerializeField] private Card[] savedAbilities = Enumerable.Repeat(Card.NONE, 18).ToArray();

    #endregion

    public long AbilityUsed(Card ability) 
        => savedAbilities.ToList().FindAll(c => c == ability).Count();
}

[Serializable]
public class CardLevel {
    public Card card;
    public int level;

    // JsonUtility requires a parameterless constructor
    public CardLevel() { }

    public CardLevel(Card card, int level) {
        this.card = card;
        this.level = level;
    }
}



[Serializable]
public class CardRelated {
    public CardRelated(Card parent, Card child) {
        Parent = parent;
        Child = child;
    }

    public Card Parent { get; set; }
    public Card Child { get; set; }
}

}