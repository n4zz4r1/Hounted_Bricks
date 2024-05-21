using System;
using Core.StateMachine.Cards;
using Framework.Base;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Core.StateMachine.CardSlots {

public class CardSlotFSM : StateMachine<CardSlotFSM, State<CardSlotFSM>> {
    [FormerlySerializedAs("CardSlotComponents")] [SerializeField]
    public Components components;

    protected override CardSlotFSM FSM => this;
    protected override State<CardSlotFSM> GetInitialState => States.Empty;

    public Sprite TemporaryIconSprite { get; set; }
    public CardFSM TemporaryCard { get; set; }
    public Sprite OriginalIconSprite { get; set; }
    public CardFSM SelectedCardFSM { get; set; }
    public int Index { get; set; }
    public CardFSM CurrentCard { get; set; }
}

[Serializable]
public class Components {
    [FormerlySerializedAs("SlotCollider")] [SerializeField]
    public BoxCollider2D slotCollider;

    [FormerlySerializedAs("SlotBox")] [SerializeField]
    public Image slotBox;

    [FormerlySerializedAs("SlotIcon")] [SerializeField]
    public Image slotIcon;

    [SerializeField] public GameObject backgroundFilledInBox;
}

}