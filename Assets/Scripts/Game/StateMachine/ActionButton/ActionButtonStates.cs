using Core.StateMachine.ActionButton;
using Core.Utils.Constants;
using Framework.Base;
using UnityEngine;

namespace Game.StateMachine.ActionButton {

public abstract class States {
    public static readonly Created Created = new();
    public static readonly Enabled Enable = new();
    public static readonly Disabled Disable = new();
    public static readonly NotEarned NotEarned = new();
}

public class Created : State<ActionButtonFSM> {
    public override void Before(ActionButtonFSM FSM) {
        // if (FSM.card == Card.NONE) return;
        //
        // if (!CardsDataV1.Instance.HasCard(FSM.card))
        //     FSM.ChangeState(States.NotEarned);
        // else
        // {
        //     FSM.CardFSM = CardFSM.GetRawCardFSM(FSM.card);
        //     // initial counter for that ability
        //     FSM.Counter.Value = FSM.CardFSM.abilityFSM.counter;
        //     FSM.components.counter.text = FSM.CardFSM.abilityFSM.counter.ToString();
        //     FSM.ChangeState(States.Enable);
        // }
    }

    public override void Active(ActionButtonFSM FSM) {
        FSM.components.button.interactable = true;
        FSM.components.counterImage.color = Colors.WithAlpha(Color.green, 1f);
    }

    public override void Inactive(ActionButtonFSM FSM) {
        FSM.components.button.interactable = false;
        FSM.components.counterImage.color = Colors.WithAlpha(Color.white, 0.5f);
    }
}

public class Enabled : State<ActionButtonFSM> {
    // public override void Enter(ActionButtonFSM FSM)
    // {
    //     FSM.components.icon.sprite = FSM.CardFSM.components.cardIcon.sprite;
    // }
    //
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
    public override void Enter(ActionButtonFSM FSM) {
        FSM.components.counterImage.color = Colors.WithAlpha(Color.white, 0.5f);
        FSM.components.button.interactable = false;
    }
}

public class NotEarned : State<ActionButtonFSM> {
    public override void Enter(ActionButtonFSM FSM) {
        FSM.components.counterImage.color = Color.white;
        FSM.components.button.gameObject.SetActive(false);
    }
}

}