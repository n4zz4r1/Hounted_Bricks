using System;
using Core.Data;
using Core.StateMachine.Cards;
using Core.StateMachine.Menu;
using Core.Utils;
using Core.Utils.Constants;
using Framework.Base;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Core.StateMachine.Rewards {

public class RewardFSM : StateMachine<RewardFSM, State<RewardFSM>> {
    [SerializeField] public Card cardSorted;
    [SerializeField] public bool shouldDestroy;
    [SerializeField] public int amount;

    [FormerlySerializedAs("Internal")] [SerializeField]
    public Components components;

    [SerializeField] public ResourceType resourceType = ResourceType.NONE;
    protected override RewardFSM FSM => this;
    protected override State<RewardFSM> GetInitialState => States.Created;

    public ResourceType CurrentResourceTypeToRoll { get; set; }
    public Card CurrentCardToRoll { get; set; }

    public static void SaveReward(ResourceType type, int amount, Card card) {
        ResourcesV1.Instance.AddResources(type, amount, card);
    }

    public void RollTheDice(ResourceType type) {
        CurrentResourceTypeToRoll = type;
        State.RollTheDice(FSM);
    }

    public void ChangeReward(ResourceType type, Card card, int quantity) {
        CurrentResourceTypeToRoll = type;
        CurrentCardToRoll = card;
        State.ChangeReward(FSM, quantity);
    }


    internal void UpdateSprites() {
        foreach (var cardFSM in components.boxCardReward.GetComponentsInChildren<CardFSM>())
            Destroy(cardFSM.gameObject);

        components.boxCoinReward.SetActive(false);
        components.boxDiamondReward.SetActive(false);
        components.quantityCounter.gameObject.SetActive(false);
        components.rewardTypeLabel.gameObject.SetActive(false);
        components.boxCoinReward.GetComponent<Image>().color = Colors.DISABLED_ALPHA;
        components.boxDiamondReward.GetComponent<Image>().color = Colors.DISABLED_ALPHA;

        if (State == States.Created) return;

        switch (resourceType) {
            case ResourceType.COIN:
                components.boxCoinReward.SetActive(true);
                break;
            case ResourceType.DIAMOND:
                components.boxDiamondReward.SetActive(true);
                break;
            case ResourceType.CHEST:
                break;
            case ResourceType.CARD:
                // components.boxCardReward.SetActive(true);
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

    internal void Earn() {
        SaveReward(resourceType, amount, cardSorted);
        FSM.SyncAllData(typeof(MenuFSM));
        ChangeState(States.Earned);
    }
}

[Serializable]
public class Components {
    [FormerlySerializedAs("BoxCoinReward")]
    public GameObject boxCoinReward;

    [FormerlySerializedAs("BoxDiamontReward")]
    public GameObject boxDiamondReward;

    [FormerlySerializedAs("BoxCardReward")]
    public GameObject boxCardReward;

    [FormerlySerializedAs("QuantityCounter")]
    public TextMeshProUGUI quantityCounter;

    [SerializeField] public TextMeshProUGUI rewardTypeLabel;
}

}