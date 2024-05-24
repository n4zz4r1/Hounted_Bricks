using System;
using Core.Controller.Bag;
using Core.StateMachine.Cards;
using Framework.Base;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Core.StateMachine.CardSlots {

public class CardSlotFSM : StateMachine<CardSlotFSM, State<CardSlotFSM>> {

    [SerializeField] public Components components;

    internal BagController BagController;

    protected override CardSlotFSM FSM => this;
    protected override State<CardSlotFSM> GetInitialState => States.Empty;

    public Sprite TemporaryIconSprite { get; set; }
    public CardFSM TemporaryCard { get; set; }
    public Sprite OriginalIconSprite { get; set; }
    public CardFSM SelectedCardFSM { get; set; }
    public int Index { get; set; }
    public CardFSM CurrentCard { get; set; }

    protected override void Before() {
        BagController = gameObject.transform.root.GetComponentInChildren<BagController>();
        base.Before();
    }
}

[Serializable]
public class Components {
    [SerializeField] public BoxCollider2D slotCollider;
    [SerializeField] public Image slotBox;
    [SerializeField] public Image slotIcon;
    [SerializeField] public GameObject backgroundFilledInBox;
}

}