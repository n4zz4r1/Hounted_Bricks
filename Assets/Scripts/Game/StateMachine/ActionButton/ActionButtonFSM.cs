using System;
using System.Collections.Generic;
using Core.Data;
using Core.StateMachine.Cards;
using Core.Utils.Constants;
using DG.Tweening;
using Framework.Base;
using Game.Controller.Game;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.StateMachine.ActionButton {

public class ActionButtonFSM : StateMachine<ActionButtonFSM, State<ActionButtonFSM>>, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler {

    protected override State<ActionButtonFSM> GetInitialState => States.Preload;
    protected override ActionButtonFSM FSM => this;

    [SerializeField] public Card card = Card.NONE;
    [SerializeField] public ActionButtonComponents components;
    [SerializeField] public GameController gameController;
    [SerializeField] public bool activeOnShooting;
    [SerializeField] public int abilityIndex = -1;

    #region Button Properties

    internal CardFSM CardFSM;
    internal readonly AtomicInt Counter = new(0);
    private readonly Dictionary<Transform, Vector3> _originalPositions = new();
    private readonly Vector3 _pressedPosition = new(0f, -5f, 0f);
    internal bool IsPointerInside { get; set; }
    internal bool IsPressed { get; set; }
    private Vector2 buttonOriginalPosition;
    public float moveDistance = 50f; // Distance to move to the right
    public float moveDuration = 0.5f; // Duration of the move

    #endregion

    protected override void Before() {
        _originalPositions.Clear();

        // When ability index is on, select card based on ability slot
        // TODO implement fetching cards from deck
        if (abilityIndex >= 0) {
            card = CardsDataV1.Instance.GetPlayerAbilityAtPosition(PlayerDataV1.Instance.GetSelectedCharacterCard(), abilityIndex);
        }
        
        buttonOriginalPosition = components.rectTransform != null ? components.rectTransform.anchoredPosition : Vector2.zero;
        foreach (Transform child in transform)
            _originalPositions[child] = child.localPosition;
    }

    // This sync will be called every time game state changes
    protected override void SyncDataBase() {
        if (CardFSM == null || CardFSM.abilityFSM == null) return;
        
        // ReSharper disable once ConvertIfStatementToSwitchStatement
        if (!CardFSM.abilityFSM.activeOnShootingStage && GameController.Instance!.State == Game.Controller.Game.States.PlayerTurn)
            State.Enable(FSM);
        else if(CardFSM.abilityFSM.activeOnShootingStage && GameController.Instance!.State == Game.Controller.Game.States.Shooting) 
            State.Enable(FSM);
        else 
            State.Disable(FSM);
    }

    public void OnPointerDown(PointerEventData eventData) =>  State.Pressed(FSM);
    public void OnPointerUp(PointerEventData eventData) => State.Released(FSM);
    public void OnPointerEnter(PointerEventData eventData) {
        IsPointerInside = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        IsPointerInside = false;
        IsPressed = false;
        State.Released(FSM);
    }
    
    public void MoveRight() =>  components.rectTransform?.DOAnchorPosX(buttonOriginalPosition.x + moveDistance, moveDuration).SetEase(Ease.OutQuad);
    
    public void MoveLeft() =>  components.rectTransform?.DOAnchorPosX(buttonOriginalPosition.x, moveDuration).SetEase(Ease.OutQuad);

    internal void MoveChildrenIcons(bool pressed) {
        // only move if button is not disabled
        foreach (Transform child in transform)
            if (_originalPositions.TryGetValue(child, out var position))
                child.localPosition = pressed ? _originalPositions[child] + _pressedPosition : _originalPositions[child];
    }

    public void AddCounter(int counter = 1) {
        Counter.Value += counter;
        components.counter.text = Counter.Value.ToString();
        State.Enable(FSM);
    }

    // Once action is done
    public void ActionDoneCallback() {
        Counter.Value--;
        State.Disable(FSM);
        components.counter.text = Counter.Value.ToString();
    }

    // Once action is canceled
    public void ActionCanceledCallback() {
        State.Enable(FSM);
    }
}

public enum SpecialAction {
    NONE,
    RECYCLE,
    NEXT_WAVE,
}

[Serializable]
public class ActionButtonComponents {
    [SerializeField] public TextMeshProUGUI counter;
    [SerializeField] public Image counterImage;
    [SerializeField] public Image image;
    [SerializeField] public Image icon;
    [SerializeField] public Sprite spriteEnabled;
    [SerializeField] public Sprite spritePressed;
    [SerializeField] public CanvasGroup canvasGroup;
    [SerializeField] public RectTransform rectTransform;
    [SerializeField] public TextMeshProUGUI cardDescription;

    [SerializeField] public GameObject consumableBox;
    [SerializeField] public TextMeshProUGUI consumableText;
}

}