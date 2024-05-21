using System;
using Core.StateMachine.Cards;
using Core.StateMachine.Menu;
using Core.Utils;
using Core.Utils.Constants;
using Framework.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Core.StateMachine.Rewards {

public abstract class States {
    public static readonly Created Created = new();
    public static readonly NotEarned NotEarned = new();
    public static readonly Earned Earned = new();
}

public class Created : State<RewardFSM> {
    public override void Enter(RewardFSM fsm) {
        fsm.UpdateSprites();
    }

    public override void RollTheDice(RewardFSM fsm) {
        fsm.resourceType = fsm.CurrentResourceTypeToRoll;
        if (fsm.CurrentResourceTypeToRoll == ResourceType.CARD) {
            fsm.cardSorted = CardDice.Roll();
            fsm.amount = 1;
        }
        else {
            fsm.amount = Dices.RollDiceFromRewardType(fsm.CurrentResourceTypeToRoll);
        }

        fsm.components.quantityCounter.text = fsm.amount.ToString();
        // TODO double check label
        // fsm.components.rewardTypeLabel.text = ResourceFSM.FromRewardType(fsm.CurrentResourceTypeToRoll);
    }

    public override void Earn(RewardFSM fsm) {
        fsm.Earn();
    }
}

public class NotEarned : State<RewardFSM> {
    public override void Enter(RewardFSM fsm) {
        fsm.UpdateSprites();
    }

    // // Change reward type
    public override void ChangeReward(RewardFSM fsm, int quantity) {
        fsm.amount = quantity;
        fsm.components.quantityCounter.text = fsm.amount.ToString();
        // TODO double check label
        // fsm.components.rewardTypeLabel.text = SpriteAddress<String, ResourceType>.LoadAssetAsync(fsm.CurrentResourceTypeToRoll);
        fsm.resourceType = fsm.CurrentResourceTypeToRoll;
        fsm.cardSorted = fsm.CurrentCardToRoll;
        fsm.UpdateSprites();

        if (fsm.CurrentResourceTypeToRoll != ResourceType.CARD) return;

        fsm.components.boxCardReward.SetActive(true);
        var cardEarnedFSM = CardFSM.GetRawDisabledCardFSM(fsm.cardSorted);
        var cardEarned = fsm.CreateInstance(cardEarnedFSM.gameObject, fsm.components.boxCardReward.transform);
        cardEarned.transform.localPosition = new Vector3(0, 0, 0);
    }

    // Earn reward
    public override void Earn(RewardFSM fsm) {
        fsm.Earn();
    }
}

public class Earned : State<RewardFSM> {
    public override void Enter(RewardFSM fsm) {
        if (fsm.shouldDestroy) {
            Debug.Log("Should destroy");
            fsm.SelfDestroy();
            return;
        }

        fsm.UpdateSprites();

        switch (fsm.resourceType) {
            case ResourceType.COIN:
                fsm.components.boxCoinReward.GetComponent<Image>().color = Colors.WHITE;
                fsm.components.quantityCounter.gameObject.SetActive(true);
                fsm.components.rewardTypeLabel.gameObject.SetActive(true);
                break;
            case ResourceType.DIAMOND:
                fsm.components.boxDiamondReward.GetComponent<Image>().color = Colors.WHITE;
                fsm.components.quantityCounter.gameObject.SetActive(true);
                fsm.components.rewardTypeLabel.gameObject.SetActive(true);
                break;
            case ResourceType.CHEST:
                // TODO chest
                break;
            case ResourceType.CARD:
                // Don't earn if it has all cards
                if (fsm.cardSorted == Card.NONE) return;

                fsm.components.boxCardReward.SetActive(true);
                var cardEarnedFSM = CardFSM.GetRawCardFSM(fsm.cardSorted);
                var cardEarned =
                    fsm.CreateInstance(cardEarnedFSM.gameObject, fsm.components.boxCardReward.transform);
                cardEarned.transform.localPosition = new Vector3(0, 0, 0);
                fsm.SyncAllData(typeof(MenuFSM));
                break;
            case ResourceType.NONE:
                break;
            case ResourceType.ROCK_SCROLL:
                break;
            case ResourceType.CHAR_SCROLL:
                break;
            case ResourceType.ABILITY_SCROLL:
                break;
            case ResourceType.CHEST_KEYS:
                break;
            case ResourceType.MONEY:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

}