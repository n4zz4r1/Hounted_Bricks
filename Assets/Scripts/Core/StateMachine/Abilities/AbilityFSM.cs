using System;
using Core.StateMachine.Cards;
using Framework.Base;
using UnityEngine;
using UnityEngine.Events;

namespace Core.StateMachine.Abilities {
public abstract class AbilityFSM : StateMachine<AbilityFSM, State<AbilityFSM>> {
    // [SerializeField] public int counter;
    [SerializeField] public AbilityType abilityType = AbilityType.NONE;
    [SerializeField] public bool activeOnShootingStage;
    protected UnityAction AbilityCanceledCallback;
    protected UnityAction AbilityDoneCallback;

    protected CardFSM CardFSM;
    // [SerializeField] public CardFSM cardFSM;

    protected override State<AbilityFSM> GetInitialState => States.Ready;
    protected override AbilityFSM FSM => this;

    // protected override State<AbilityFSM> GetInitialState =>
    //     cardFSM && CardsDataV1.Instance.HasCard(cardFSM.cardId) ? States.Available : States.Unavailable;

    // internal ActionButtonFSM ActionButtonFSM;
    //
    // internal UnityAction<ActionButtonFSM, bool> CallBack = (_, _) => { };
    // internal GameController GameController;
    //
    //
    // // Behave for action abilities

    public abstract void Execute<T>(T GameController, CardFSM cardFSM, UnityAction callback,
        UnityAction canceledCallback) where T : MonoBehaviour;

    //
    // // Behave for improvement abilities
    // public void Initialize(GameController gameController) {
    //     GameController = gameController;
    //     Execute();
    // }
}

public abstract class Ability<T> : AbilityFSM where T : MonoBehaviour {
    protected T GameController;
    protected GameObject Panel;

    public override void Execute<T1>(T1 controller, CardFSM cardFSM, UnityAction callback,
        UnityAction canceledCallback) {
        CardFSM = cardFSM;
        AbilityDoneCallback = callback;
        AbilityCanceledCallback = canceledCallback;
        GameController = controller as T;
        InitAction();
    }
    
    // public void Execute(T controller, UnityAction callback,  UnityAction canceledCallback) {
    //     AbilityDoneCallback = callback;
    //     AbilityCanceledCallback = canceledCallback;
    //     GameController = controller;
    //     ExecuteAction();
    // }

    // public void Execute(T controller) {
    //     GameController = controller;
    //     ExecuteAction();
    // }

    protected abstract void InitAction();
}

[Serializable]
public enum AbilityType {
    NONE,
    ACTIVE_COUNTER,
    ACTIVE_CONSUMABLE
}

[Serializable]
public enum AbilityPanel {
    Move,
    LucasBasic,
    RockPile
}
}