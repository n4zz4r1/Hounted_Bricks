using System;
using Core.Data;
using Core.Handler;
using Core.Sprites;
using Core.Utils;
using Core.Utils.Constants;
using Framework.Base;
using UnityEngine;
using Button = UnityEngine.UI.Button;


namespace Core.StateMachine.ShopItems {

public class ShopItemFSM : StateMachine<ShopItemFSM, State<ShopItemFSM>> {
    [SerializeField] public ResourceType costType = ResourceType.Money;
    [SerializeField] public float cost;
    [SerializeField] public ResourceType rewardType = ResourceType.Coin;
    [SerializeField] public long reward;
    [SerializeField] public Card card = Card.NONE;
    [SerializeField] public ShopItemComponents components;

    protected override ShopItemFSM FSM => this;
    protected override State<ShopItemFSM> GetInitialState => States.Preload;

    protected override void Before() {
        components.shopItemButton.onClick.AddListener(() => { State.Buy(FSM); });
    }

    protected override void SyncDataBase() {
        if (rewardType == ResourceType.Card && CardsDataV1.Instance.HasCard(card))
            ChangeState(States.SoldOut);
        else if (costType != ResourceType.Money &&
                 !ResourcesV1.Instance.HasEnoughResource(costType, (long)cost))
            ChangeState(States.NoFunds);
        else
            ChangeState(States.Available);
    }
}

public enum ShopItemType {
    CARD_SPECIAL,
    CARD_DIAMOND,
    COINS,
    CHESTS,
    MONEY
}

[Serializable]
public class ShopItemComponents {
    [SerializeField] public Button shopItemButton;
    [SerializeField] public GameObject availableBox;
    [SerializeField] public ButtonPressEffectHandler effectHandler;
}

}