using Core.StateMachine.ActionButton;
using Framework.Base;
using Game.Controller.Game;
using UnityEngine;
using UnityEngine.Events;

namespace Game.StateMachine.Abilities {

public abstract class AbilityFSM : StateMachine<AbilityFSM, State<AbilityFSM>> {
    [SerializeField] public int counter;
    [SerializeField] public AbilityType abilityType = AbilityType.NONE;
    internal ActionButtonFSM ActionButtonFSM;

    internal UnityAction<ActionButtonFSM, bool> CallBack = (_, _) => { };
    internal GameController GameController;

    protected override State<AbilityFSM> GetInitialState => States.Created;

    // Behave for action abilities
    public void Execute(ActionButtonFSM actionButtonFSM, GameController gameController,
        UnityAction<ActionButtonFSM, bool> callbackAction) {
        ActionButtonFSM = actionButtonFSM;
        CallBack = callbackAction;
        GameController = gameController;
        Execute();
    }

    // Behave for improvement abilities
    public void Initialize(GameController gameController) {
        GameController = gameController;
        Execute();
    }

    protected virtual void Execute() { }
}

public enum AbilityType {
    GENERAL_IMPROVEMENT,
    ROCK_IMPROVEMENT,
    CONSUMABLE,
    TIME_CONSUMABLE,
    NONE
}

}