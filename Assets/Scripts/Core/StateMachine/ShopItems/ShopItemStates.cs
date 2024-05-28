using Core.Data;
using Core.StateMachine.Resource;
using Core.Utils;
using Core.Utils.Constants;
using Framework.Base;

namespace Core.StateMachine.ShopItems {

public abstract class States {
    public static readonly Preload Preload = new();
    public static readonly Available Available = new();
    public static readonly NoFunds NoFunds = new();
    public static readonly SoldOut SoldOut = new();
}

public class Preload : State<ShopItemFSM> {
    public override void Enter(ShopItemFSM fsm) {
        fsm.Sync();
    }
}

public class Available : State<ShopItemFSM> {
    public override void Enter(ShopItemFSM fsm) {
        fsm.components.shopItemButton.interactable = true;
        fsm.components.shopItemButton.enabled = true;
        if (fsm.rewardType != ResourceType.CARD)
            fsm.components.shopItemButton.image.color = ResourceUtils.From(fsm.rewardType).BackgroundColor;
    }

    public override void Buy(ShopItemFSM fsm) {
        fsm.components.shopItemButton.enabled = false;

        var success = true;

        // First, spend cost
        // todo implement here for money $$$$
        if (fsm.costType != ResourceType.MONEY)
            success = ResourcesV1.Instance.SpendResources(fsm.costType, (long)fsm.cost);

        if (!success) return;

        // Second, earn reward if succeeded
        if (fsm.rewardType == ResourceType.CARD)
            CardsDataV1.Instance.AddCard(fsm.card);
        else
            ResourcesV1.Instance.AddResources(fsm.rewardType, fsm.reward);

        fsm.components.shopItemButton.enabled = true;
        fsm.SyncAllData<ResourceFSM, State<ResourceFSM>>(TagType.Resource);
        fsm.SyncAllData(typeof(ShopItemFSM));
    }
}

public class SoldOut : State<ShopItemFSM> {
    public override void Enter(ShopItemFSM fsm) {
        fsm.components.availableBox.SetActive(false);
    }
}


public class NoFunds : State<ShopItemFSM> {
    public override void Enter(ShopItemFSM fsm) {
        fsm.components.effectHandler.MoveDown();
        fsm.components.shopItemButton.enabled = false;
        fsm.components.shopItemButton.image.color = Colors.DISABLED;
        fsm.components.shopItemButton.interactable = false;
    }

    public override void Exit(ShopItemFSM fsm) {
        fsm.components.shopItemButton.enabled = true;
        fsm.components.effectHandler.MoveUp();
    }
}

}