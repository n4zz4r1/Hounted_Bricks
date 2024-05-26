using System;
using Core.Controller.Audio;
using Core.StateMachine.Cards;
using Core.Utils.Constants;
using Framework.Base;
using Game.Controller.Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using States = Game.StateMachine.ActionButton.States;

namespace Core.StateMachine.ActionButton {

public class ActionButtonFSM : StateMachine<ActionButtonFSM, State<ActionButtonFSM>> {
    [SerializeField] public Card card = Card.NONE;
    [SerializeField] public ActionButtonComponents components;
    [SerializeField] public GameController gameController;
    [SerializeField] public AudioClip clickFX;

    internal CardFSM CardFSM;
    public AtomicInt Counter = new(0);
    protected override ActionButtonFSM FSM => this;
    protected override State<ActionButtonFSM> GetInitialState => States.Created;

    protected override void Before() {
        components.button.onClick.AddListener(() => {
            AudioController.PlayFX(FSM.clickFX);
            State.Click(FSM);
        });
    }


    // This sync will be called every time gamestate changes

    public void Active() {
        State.Active(FSM);
    }

    public void Inactive() {
        State.Inactive(FSM);
    }
}

[Serializable]
public class ActionButtonComponents {
    [SerializeField] public Button button;
    [SerializeField] public Image icon;
    [SerializeField] public TextMeshProUGUI counter;
    [SerializeField] public Image counterImage;
}

}