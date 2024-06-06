using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.StateMachine.Cards;
using Core.Utils;
using Core.Utils.Constants;
using Framework.Base;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Core.StateMachine.CardSlots {
public enum CardSlotType {
    Rock,
    Ability
}

public class CardSlotFSM : StateMachine<CardSlotFSM, State<CardSlotFSM>>, IPointerDownHandler, IPointerUpHandler,
    IPointerEnterHandler, IPointerExitHandler {
    protected static readonly int RemoveAnim = Animator.StringToHash("Remove");

    [SerializeField] public CardSlotType type = CardSlotType.Rock;
    [SerializeField] public int index;
    [SerializeField] public Components components;
    public Card PlayerCard { get; set; } = Card.NONE;

    protected override CardSlotFSM FSM => this;
    protected override State<CardSlotFSM> GetInitialState => States.Preload;

    public Sprite TemporaryIconSprite { get; set; }
    public CardFSM TemporaryCard { get; set; }
    public Sprite OriginalIconSprite { get; set; }
    public CardFSM SelectedCardFSM { get; set; }
    public CardFSM CurrentCard { get; protected set; }
    public Dictionary<Card, CardFSM> CardPrefabDictionary { get; set; } = new();

    protected override async Task BeforeAsync() {
        // update icon, label and text based on its type
        CardPrefabDictionary.Add(Card.Card_001_Crooked_Rock,
            await AssetLoader<Card>.Load<CardFSM>(Card.Card_001_Crooked_Rock));
        CardPrefabDictionary.Add(Card.Card_002_Rounded_Rock,
            await AssetLoader<Card>.Load<CardFSM>(Card.Card_002_Rounded_Rock));
        CardPrefabDictionary.Add(Card.Card_003_Arrowed_Rock,
            await AssetLoader<Card>.Load<CardFSM>(Card.Card_003_Arrowed_Rock));
        CardPrefabDictionary.Add(Card.Card_004_Bomb_Rock,
            await AssetLoader<Card>.Load<CardFSM>(Card.Card_004_Bomb_Rock));
    }

    protected virtual void PersistCard() { }
    protected virtual void ClearSlot() { }

    protected override void SyncDataBase() {
        SyncSlots();
    }

    protected virtual void SyncSlots() { }

    #region Remove Event Region

    private bool IsPointerInside { get; set; }
    private bool IsPressed { get; set; }

    public void OnPointerDown(PointerEventData eventData) {
        IsPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData) {
        IsPressed = false;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        IsPointerInside = true;
    }

    // When dragged out, remove it
    public void OnPointerExit(PointerEventData eventData) {
        if (IsPointerInside && IsPressed)
            ClearSlot();

        IsPointerInside = false;
        IsPressed = false;
    }

    #endregion
}

[Serializable]
public class Components {
    [SerializeField] public BoxCollider2D slotCollider;
    [SerializeField] public Image slotBox;
    [SerializeField] public Image slotIcon;
    [SerializeField] public GameObject backgroundFilledInBox;
    [SerializeField] public GameObject glow;
    [SerializeField] public List<Image> paths;
    [SerializeField] public Animator animator;
}
}