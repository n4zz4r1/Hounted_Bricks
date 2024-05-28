using Core.Data;
using Core.StateMachine.Cards;
using Framework.Base;
using UnityEngine;

namespace Core.StateMachine.Abilities {

public abstract class AbilityFSM : StateMachine<AbilityFSM, State<AbilityFSM>> {

    [SerializeField] public int counter;
    [SerializeField] public AbilityType abilityType = AbilityType.NONE;
    [SerializeField] public CardFSM cardFSM;

    protected override State<AbilityFSM> GetInitialState =>
        cardFSM && CardsDataV1.Instance.HasCard(cardFSM.cardId) ? States.Available : States.Unavailable;

    // internal ActionButtonFSM ActionButtonFSM;
    //
    // internal UnityAction<ActionButtonFSM, bool> CallBack = (_, _) => { };
    // internal GameController GameController;
    //
    //
    // // Behave for action abilities
    // public void Execute(ActionButtonFSM actionButtonFSM, GameController gameController,
    //     UnityAction<ActionButtonFSM, bool> callbackAction) {
    //     ActionButtonFSM = actionButtonFSM;
    //     CallBack = callbackAction;
    //     GameController = gameController;
    //     Execute();
    // }
    //
    // // Behave for improvement abilities
    // public void Initialize(GameController gameController) {
    //     GameController = gameController;
    //     Execute();
    // }

    protected virtual void Execute() { }
}

public enum AbilityType {
    NONE,
    ACTIVE_COUNTER,
    ACTIVE_CONSUMABLE,
}

}