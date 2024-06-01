using Core.Controller.Audio;
using Core.Data;
using Core.StateMachine.Cards;
using Core.Utils;
using Core.Utils.Constants;
using Framework.Base;
using Game.Controller.Game;
using AbilityType = Core.StateMachine.Abilities.AbilityType;

namespace Game.StateMachine.ActionButton {

public abstract class States {
    public static readonly Preload Preload = new();
    public static readonly Enabled Enabled = new();
    public static readonly Disabled Disabled = new();
    public static readonly NotAvailable NotAvailable = new();
}

public class Preload : State<ActionButtonFSM> {
    public override void Before(ActionButtonFSM fsm) {
        if (fsm.card == Card.NONE) return;
        
        // If you don't have card, change to NotAvailable
        AssetLoader<Card>.Load<ActionButtonFSM, CardFSM>(fsm.card, fsm, SetCard);

        //
        // if (!CardsDataV1.Instance.HasCard(fsm.card))
        //     fsm.ChangeState(States.NotAvailable);
        // else
        // {
        //     fsm.CardFSM = CardFSM.GetRawCardFSM(fsm.card);
        //     // initial counter for that ability
        //     fsm.Counter.Value = fsm.CardFSM.abilityFSM.counter;
        //     fsm.components.counter.text = fsm.CardFSM.abilityFSM.counter.ToString();
        //     fsm.ChangeState(States.Enable);
        // }
    }

    private static void SetCard(CardFSM cardFSM, ActionButtonFSM fsm) {
        fsm.CardFSM = cardFSM;
        fsm.components.icon.sprite = cardFSM.components.cardIcon.sprite;
        
        // Set Counter number, else unlimited actions
        if (cardFSM.abilityFSM?.abilityType is AbilityType.ACTIVE_COUNTER) {
            fsm.Counter.Value = cardFSM.Attribute(CardAttribute.QUANTITY);
            fsm.components.counter.text = fsm.Counter.Value.ToString();
        }
        else 
            fsm.Counter.Value = 9999;
        
        // set counter based on ability c
        if (!CardsDataV1.Instance.HasCard(fsm.card) || cardFSM.abilityFSM == null)
            fsm.ChangeState(States.NotAvailable);
        else if (cardFSM.abilityFSM.activeOnShootingStage)
            fsm.ChangeState(States.Disabled);
        else
            fsm.ChangeState(States.Enabled);
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
    public override void Enter(ActionButtonFSM fsm) {
        fsm.components.image.color = Colors.PRIMARY;
    }

    public override void Pressed(ActionButtonFSM fsm) {
        AudioController.PlayFX(CommonFX.CLICK_BUTTON_FX);
        fsm.IsPressed = true;
        fsm.MoveChildrenIcons(true);
        fsm.components.image.sprite = fsm.components.spritePressed;        
    }

    public override void Released(ActionButtonFSM fsm) {
        fsm.MoveChildrenIcons(false);
        fsm.components.image.sprite = fsm.components.spriteEnabled;
        if (!fsm.IsPointerInside || !fsm.IsPressed) {
            return;
        }

        fsm.ChangeState(States.Disabled);

        // Execute ability and disable button
        fsm.CardFSM.abilityFSM.Execute(fsm.gameController, fsm.ActionDoneCallback, fsm.ActionCanceledCallback);
    }
    
    public override void Disable(ActionButtonFSM fsm) {
        fsm.ChangeState(States.Disabled);
    }
    
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
        fsm.components.image.color = Colors.DISABLED;
        fsm.components.canvasGroup.alpha = 0.8f;
    }
    public override void Exit(ActionButtonFSM fsm) {
        fsm.MoveChildrenIcons(false);
        fsm.components.image.sprite = fsm.components.spriteEnabled;
        fsm.components.canvasGroup.alpha = 1f;
    }

    public override void Enable(ActionButtonFSM fsm) {
        if(fsm.Counter.Value > 0)
            fsm.ChangeState(States.Enabled);
    }
}

public class NotAvailable : State<ActionButtonFSM> {
    public override void Enter(ActionButtonFSM fsm) {
        fsm.MoveChildrenIcons(true);
        fsm.components.counterImage.gameObject.SetActive(false);
        fsm.components.image.sprite = fsm.components.spritePressed;
        fsm.components.canvasGroup.alpha = 0.1f;
        fsm.components.canvasGroup.blocksRaycasts = false;
        fsm.components.canvasGroup.interactable = false;
        fsm.components.image.color = Colors.DISABLED;
    }
}

}