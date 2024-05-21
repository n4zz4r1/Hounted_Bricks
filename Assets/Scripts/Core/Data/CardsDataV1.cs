using System;
using System.Collections.Generic;
using System.Linq;
using Core.Utils;
using Core.Utils.Constants;
using Framework.Base;
using UnityEngine;

namespace Core.Data {

[Serializable]
public class CardsDataV1 : Data<CardsDataV1> {
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

    public Card GetParentCard(Card child) {
        return _cardRelationship.Exists(c => c.Child == child)
            ? _cardRelationship.Find(c => c.Child == child).Parent
            : Card.NONE;
    }

    public Card GetChildCard(Card parent) {
        return _cardRelationship.Exists(c => c.Parent == parent)
            ? _cardRelationship.Find(c => c.Parent == parent).Child
            : Card.NONE;
    }

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

    public bool IncreaseLevel(Card card, ResourceType resourceUpdateType) {
        var success = true;

        Transaction(() => {
            var cardLevel = cardsLevel.Find(c => c.card.Equals(card));
            var coinCost = GameMathUtils.GenerateUpdateCostByLevel(cardLevel.level + 1);
            var resourceUpdateCost =
                GameMathUtils.GenerateUpdateCostByLevel(cardLevel.level + 1, resourceUpdateType);

            cardLevel.level++;

            // spend resource
            success = ResourcesV1.Instance.SpendResources(ResourceType.COIN, coinCost, resourceUpdateType,
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

        // Characters
        Card.Card_005_Char_Lucas,
        // TODO remove characters
        Card.Card_006_Char_Lisa,

        // Basic Cards
        // TODO remove
        Card.Card_046_Basic_Move
    };

    [SerializeField] private List<CardLevel> cardsLevel = new() {
        new CardLevel(Card.Card_001_Crooked_Rock, 1),
        new CardLevel(Card.Card_002_Rounded_Rock, 1),
        new CardLevel(Card.Card_003_Arrowed_Rock, 1),
        new CardLevel(Card.Card_004_Bomb_Rock, 1),
        new CardLevel(Card.Card_005_Char_Lucas, 1),
        new CardLevel(Card.Card_006_Char_Lisa, 1),
        new CardLevel(Card.Card_007_Char_Bill, 1)
    };

    [SerializeField] private readonly List<CardLevel> _cardsMaxQuantity = new() {
        new CardLevel(Card.Card_001_Crooked_Rock, 20),
        new CardLevel(Card.Card_002_Rounded_Rock, 20),
        new CardLevel(Card.Card_003_Arrowed_Rock, 10),
        new CardLevel(Card.Card_004_Bomb_Rock, 10),
        new CardLevel(Card.Card_010_Special_Small_Diamond_Pack, 20),
        new CardLevel(Card.Card_011_Special_Big_Diamond_Pack, 20),
        new CardLevel(Card.Card_012_Improved_Crooked_Rock, 3),
        new CardLevel(Card.Card_013_Improved_Rounded_Rock, 3),
        new CardLevel(Card.Card_014_Improved_Arrowed_Rock, 3),
        // new CardLevel(Card.Card_015_Improved_Bomb_Rock,         4),
        new CardLevel(Card.Card_046_Basic_Move, 1)
    };

    [SerializeField] private readonly List<CardRelated> _cardRelationship = new() {
        new CardRelated(Card.Card_001_Crooked_Rock, Card.Card_012_Improved_Crooked_Rock),
        new CardRelated(Card.Card_002_Rounded_Rock, Card.Card_013_Improved_Rounded_Rock),
        new CardRelated(Card.Card_003_Arrowed_Rock, Card.Card_014_Improved_Arrowed_Rock)
        // new CardRelated(Card.Card_004_Bomb_Rock,        Card.Card_015_Improved_Bomb_Rock),
    };

    #endregion
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