using System;
using System.Collections.Generic;
using Core.StateMachine.Cards;
using Core.Utils.Constants;
using Framework.Base;
using Game.Controller.Game;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.StateMachine.ActionButton {

public class ActionButtonFSM : StateMachine<ActionButtonFSM, State<ActionButtonFSM>>, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler {

    [SerializeField] public Card card = Card.NONE;
    [SerializeField] public ActionButtonComponents components;
    [SerializeField] public GameController gameController;

    #region Button Properties

    internal CardFSM CardFSM;
    internal readonly AtomicInt Counter = new(0);
    protected override ActionButtonFSM FSM => this;
    protected override State<ActionButtonFSM> GetInitialState => States.Preload;
    private readonly Dictionary<Transform, Vector3> _originalPositions = new();
    private readonly Vector3 _pressedPosition = new(0f, -5f, 0f);
    internal bool IsPointerInside { get; set; }

    #endregion

    protected override void Before() {
        _originalPositions.Clear();
        foreach (Transform child in transform)
            _originalPositions[child] = child.localPosition;
    }

    // This sync will be called every time game state changes
    protected override void SyncDataBase() {
        if (GameController.Instance!.State == Game.Controller.Game.States.PlayerTurn) {
            ChangeState(States.Enable);
        } else {
            ChangeState(States.Disable);
        }
    }

    public void OnPointerDown(PointerEventData eventData) =>  State.Pressed(FSM);
    public void OnPointerUp(PointerEventData eventData) => State.Released(FSM);
    public void OnPointerEnter(PointerEventData eventData) {
        IsPointerInside = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        IsPointerInside = false;
        State.Released(FSM);
    }
    internal void MoveChildrenIcons(bool pressed) {
        // only move if button is not disabled
        foreach (Transform child in transform)
            if (_originalPositions.TryGetValue(child, out var position))
                child.localPosition = pressed ? _originalPositions[child] + _pressedPosition : _originalPositions[child];
    }

}

[Serializable]
public class ActionButtonComponents {
    [SerializeField] public TextMeshProUGUI counter;
    [SerializeField] public Image counterImage;
    [SerializeField] public Image image;
    [SerializeField] public Image icon;
    [SerializeField] public Sprite spriteEnabled;
    [SerializeField] public Sprite spritePressed;
}

}