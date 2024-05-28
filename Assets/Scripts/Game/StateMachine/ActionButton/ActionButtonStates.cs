using Core.Controller.Audio;
using Core.Data;
using Core.StateMachine.Cards;
using Core.Utils.Constants;
using Framework.Base;
using UnityEngine;

namespace Game.StateMachine.ActionButton {

public abstract class States {
    public static readonly Preload Preload = new();
    public static readonly Enabled Enable = new();
    public static readonly Disabled Disable = new();
    public static readonly NotAvailable NotAvailable = new();
}

public class Preload : State<ActionButtonFSM> {
    public override void Before(ActionButtonFSM fsm) {
        if (fsm.card == Card.NONE) return;
        
        if (!CardsDataV1.Instance.HasCard(fsm.card))
            fsm.ChangeState(States.NotAvailable);
        else
        {
            fsm.CardFSM = CardFSM.GetRawCardFSM(fsm.card);
            // initial counter for that ability
            fsm.Counter.Value = fsm.CardFSM.abilityFSM.counter;
            fsm.components.counter.text = fsm.CardFSM.abilityFSM.counter.ToString();
            fsm.ChangeState(States.Enable);
        }
    }

    // public override void Active(ActionButtonFSM fsm) {
    //     // fsm.components.button.interactable = true;
    //     fsm.components.counterImage.color = Colors.WithAlpha(Color.green, 1f);
    // }
    //
    // public override void Inactive(ActionButtonFSM fsm) {
    //     // fsm.components.button.interactable = false;
    //     fsm.components.counterImage.color = Colors.WithAlpha(Color.white, 0.5f);
    // }
    //

}

public class Enabled : State<ActionButtonFSM> {

    public override void Pressed(ActionButtonFSM fsm) {
        AudioController.PlayFX(CommonFX.CLICK_BUTTON_FX);
        fsm.MoveChildrenIcons(true);
        fsm.components.image.sprite = fsm.components.spritePressed;        
    }

    public override void Released(ActionButtonFSM fsm) {
        fsm.MoveChildrenIcons(false);
        fsm.components.image.sprite = fsm.components.spriteEnabled;
    }

    // public override void Click(ActionButtonFSM FSM)
    // {
    //     FSM.CardFSM.abilityFSM.Execute(FSM, FSM.gameController, EndActionCallback);
    // }
    //
    // public override void SyncData(ActionButtonFSM FSM)
    // {
    //     
    //     if (FSM.card == Card.NONE) return;
    //     Debug.Log("Syncing Card: " + FSM.card);
    //     
    //     switch (FSM.CardFSM.abilityFSM.abilityType)
    //     {
    //         case AbilityType.GENERAL_IMPROVEMENT:
    //             break;
    //         case AbilityType.ROCK_IMPROVEMENT:
    //             break;
    //         case AbilityType.CONSUMABLE:
    //             if (FSM.gameController.State == Core.Controllers.Game.States.PLAYER_TURN)
    //                 FSM.State.Active(FSM);
    //             else
    //                 FSM.State.Inactive(FSM);
    //             break;
    //         case AbilityType.TIME_CONSUMABLE:
    //             break;
    //         case AbilityType.NONE:
    //             break;
    //         default:
    //             throw new ArgumentOutOfRangeException();
    //     }
    // }
    //
    // public override void Active(ActionButtonFSM FSM)
    // {
    //     FSM.components.button.interactable = true;
    //     FSM.components.counterImage.color = Colors.WithAlpha(Color.green, 1f);
    // }
    // public override void Inactive(ActionButtonFSM FSM)
    // {
    //     FSM.components.button.interactable = false;
    //     FSM.components.counterImage.color = Colors.WithAlpha(Color.white, 0.5f);
    // }
    //
    // private static void EndActionCallback(ActionButtonFSM FSM, bool success)
    // {
    //     if (!success) return;
    //
    //     var counter = FSM.Counter.Subtract(1);
    //     FSM.components.counter.text = counter.ToString();
    //     if (counter == 0)
    //     {
    //         FSM.ChangeState(States.Disable);
    //     }
    // }
}

public class Disabled : State<ActionButtonFSM> {
    public override void Enter(ActionButtonFSM fsm) {
        fsm.MoveChildrenIcons(true);
        fsm.components.image.sprite = fsm.components.spritePressed;
        // FSM.components.button.interactable = false;
    }
    public override void Exit(ActionButtonFSM fsm) {
        fsm.MoveChildrenIcons(false);
        fsm.components.image.sprite = fsm.components.spriteEnabled;
    }
}

public class NotAvailable : State<ActionButtonFSM> {
    public override void Enter(ActionButtonFSM FSM) {
        FSM.components.counterImage.color = Color.white;
        // FSM.components.button.gameObject.SetActive(false);
    }
}

}