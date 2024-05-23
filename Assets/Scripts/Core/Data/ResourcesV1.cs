using System;
using System.Collections.Generic;
using Core.StateMachine.Cards;
using Core.Utils;
using Core.Utils.Constants;
using Framework.Base;
using UnityEngine;

namespace Core.Data {

[Serializable]
public class ResourcesV1 : Data<ResourcesV1> {
    // TODO improve performance by indexing / using dictionaries
    private GameResource GetGameResource(ResourceType type) {
        return gameResources.Find(g => g.resourceType == type);
    }

    public long GetResourcesAmount(ResourceType type) {
        return GetGameResource(type).quantity;
    }

    public long GetResourcesAmount(CardType cardType) {
        return GetResourcesAmount(CardTypeUtils.ToResource(cardType));
    }

    public void SetResource(ResourceType type, long amount) {
        GetGameResource(type).quantity = amount;
    }

    public bool BuyResourceWithDiamond(long price, ResourceType type, long quantity) {
        var result = true;
        Transaction(() => {
            if (HasEnoughResource(ResourceType.DIAMOND, price)) {
                GetGameResource(ResourceType.DIAMOND).quantity =
                    GetGameResource(ResourceType.DIAMOND).quantity - price;
                GetGameResource(type).quantity = GetGameResource(type).quantity + quantity;
            }
            else {
                result = false;
            }
        });
        return result;
    }

    public Gem AddGem(GemType type, GemSize size = GemSize.SMALL) {
        var gem = new Gem(Guid.NewGuid(), type, size, Card.NONE, -1);
        Transaction(() => { gems.Add(gem); });
        return gem;
    }

    public Gem AttachGem(Gem gem, Card card, int slotNumber = 0) {
        Transaction(() => {
            var find = gems.Find(g => g.ID == gem.ID);
            find.cardAttached = card;
            find.slotNumber = slotNumber;
        });
        return gem;
    }

    public Gem DetachGem(Gem gem) {
        Transaction(() => {
            var find = gems.Find(g => g.ID == gem.ID);
            find.cardAttached = Card.NONE;
            find.slotNumber = -1;
        });
        return gem;
    }

    // TODO improve performance here
    public Dictionary<int, Gem> GetAttachedGems(Card card) {
        var dictionary = new Dictionary<int, Gem>();
        foreach (var gem in gems.FindAll(g => g.cardAttached == card))
            dictionary[gem.slotNumber] = gem;

        return dictionary;
    }

    public List<Gem> GetAvailableGems() {
        return gems.FindAll(g => g.cardAttached == Card.NONE);
    }

    public bool OpenChest() {
        var result = true;

        Transaction(() => {
            if (GetGameResource(ResourceType.CHEST).quantity <= 0 || (!PlayerDataV1.Instance.HasMasterKey() &&
                                                                      GetGameResource(ResourceType.CHEST_KEYS)
                                                                          .quantity <= 0)) {
                result = false;
            }
            else {
                if (!PlayerDataV1.Instance.HasMasterKey())
                    GetGameResource(ResourceType.CHEST_KEYS).quantity =
                        GetGameResource(ResourceType.CHEST_KEYS).quantity - 1;
                GetGameResource(ResourceType.CHEST).quantity = GetGameResource(ResourceType.CHEST).quantity - 1;
            }
        });

        return result;
    }

    public bool AddResources(ResourceType type, long amount, Card card = Card.NONE) {
        var result = false;

        // For cards, add on CardsV1
        if (type.Equals(ResourceType.CARD)) {
            CardsDataV1.Instance.AddCard(card);
            return true;
        }

        Transaction(() => {
            var gameResource = GetGameResource(type);
            // should never add more than it's limit
            if (gameResource.limit != 0 && gameResource.quantity >= gameResource.limit) {
                result = false;
            }
            else {
                result = true;
                gameResource.quantity = gameResource.quantity + amount;
            }
        });

        return result;
    }

    public bool SpendResources(ResourceType type, long amount) {
        var result = false;
        Transaction(() => {
            var gameResource = GetGameResource(type);

            // should return false it is not enough
            if (!HasEnoughResource(type, amount)) {
                result = false;
            }
            else {
                result = true;
                gameResource.quantity = gameResource.quantity - amount;
            }
        });

        return result;
    }

    public bool SpendResources(ResourceType type, long amount, ResourceType type2, long amount2) {
        var result = false;
        Transaction(() => {
            // should return false it is not enough
            if (!HasEnoughResource(type, amount) || !HasEnoughResource(type2, amount2)) {
                result = false;
            }
            else {
                result = true;
                GetGameResource(type).quantity = GetGameResource(type).quantity - amount;
                GetGameResource(type2).quantity = GetGameResource(type2).quantity - amount;
            }
        });

        return result;
    }

    public bool HasResource(ResourceType type) {
        return GetGameResource(type).quantity > 0;
    }

    public bool HasEnoughResource(ResourceType type, long amount) {
        return GetGameResource(type).quantity - amount >= 0;
    }

    #region Properties

    // [SerializeField] private long[] resources = new[] { 0L, 0L, 0L, 0L, 0L, 0L, 0L, 0L, 0L };
    // [SerializeField] private long[] limits = new[] { 0L, 0L, 0L, 0L, 0L, 0L, 0L, 0L, 0L };
    [SerializeField] private List<Gem> gems = new();

    [SerializeField] private List<GameResource> gameResources = new() {
        new GameResource(ResourceType.COIN, 0, 0),
        new GameResource(ResourceType.CHEST_KEYS, 0, 0),
        new GameResource(ResourceType.CHEST, 0, 0),
        new GameResource(ResourceType.DIAMOND, 0, 0),
        new GameResource(ResourceType.ABILITY_SCROLL, 0, 0),
        new GameResource(ResourceType.CHAR_SCROLL, 0, 0),
        new GameResource(ResourceType.CARD, 0, 0),
        new GameResource(ResourceType.ROCK_SCROLL, 0, 0),
        new GameResource(ResourceType.NONE, 0, 0),
        new GameResource(ResourceType.MONEY, 0, 0)
    };

    #endregion
}

[Serializable]
public class Gem {
    [SerializeField] public GemType type;
    [SerializeField] public GemSize size;
    [SerializeField] public Card cardAttached;
    [SerializeField] public int slotNumber;
    public Guid ID;

    // JsonUtility requires a parameterless constructor
    public Gem() { }

    public Gem(Guid ID, GemType type, GemSize size, Card cardAttached, int slotNumber) {
        this.ID = ID;
        this.type = type;
        this.size = size;
        this.cardAttached = cardAttached;
        this.slotNumber = slotNumber;
    }
}

[Serializable]
public class GameResource {
    [SerializeField] public ResourceType resourceType;
    [SerializeField] public long quantity;
    [SerializeField] public long limit;

    // JsonUtility requires a parameterless constructor
    public GameResource() { }

    public GameResource(ResourceType resourceType, long quantity, long limit) {
        this.resourceType = resourceType;
        this.quantity = quantity;
        this.limit = limit;
    }
}

public enum GemType {
    POWER = 0,
    RESISTANCE = 1,
    HEALTH = 2,
    AIM = 3,
    MAGIC = 4
}

public enum GemSize {
    SMALL = 0,
    MEDIUM = 1,
    LARGE = 2,
    RADIANT = 3
}

}