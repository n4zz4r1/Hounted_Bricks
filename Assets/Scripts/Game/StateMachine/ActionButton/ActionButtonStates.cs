using Core.Controller.Audio;
using Core.Data;
using Core.Sprites;
using Core.StateMachine.Cards;
using Core.Utils;
using Core.Utils.Constants;
using Framework.Base;
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
        if (fsm.card == Card.NONE) {
            fsm.ChangeState(States.NotAvailable);
            return;
        }

        // If you don't have card, change to NotAvailable
        AssetLoader<Card>.Load<ActionButtonFSM, CardFSM>(fsm.card, fsm, SetCard);
    }

    private static void SetCard(CardFSM cardFSM, ActionButtonFSM fsm) {
        fsm.CardFSM = cardFSM;
        fsm.components.icon.sprite = cardFSM.components.cardIcon.sprite;
        if (fsm.components.cardDescription)
            fsm.components.cardDescription.text = cardFSM.GetTitle();

        // For basic cards, hide button frame
        if (fsm.CardFSM.cardType == CardType.BasicAbility) fsm.components.image.enabled = false;

        // Set Counter number, else unlimited actions
        if (cardFSM.abilityFSM?.abilityType is AbilityType.ACTIVE_COUNTER) {
            fsm.Counter.Value = cardFSM.Attribute(CardAttributeType.Quantity);
            fsm.components.counter.text = fsm.Counter.Value.ToString();
        }
        else if (cardFSM.cardType is CardType.Ability) {
            fsm.components.consumableText.text = cardFSM.Attribute(CardAttributeType.Consumable).ToString();

            fsm.Counter.Value = 9999;
        }
        else {
            fsm.Counter.Value = 9999;
        }

        // set counter based on ability c
        if (!CardsDataV1.Instance.HasCard(fsm.card) || cardFSM.abilityFSM == null)
            fsm.ChangeState(States.NotAvailable);
        else if (cardFSM.abilityFSM.activeOnShootingStage)
            fsm.ChangeState(States.Disabled);
        else
            fsm.ChangeState(States.Enabled);
    }
}

public class Enabled : State<ActionButtonFSM> {
    public override void Enter(ActionButtonFSM fsm) {
        fsm.components.image.color = fsm.card is Card.Card_E_Recycle or Card.Card_E_NextWave
            ? Colors.TERDIARY
            : RarityUtils.From(fsm.CardFSM.Rarity).NormalColor;
    }

    public override void Pressed(ActionButtonFSM fsm) {
        AudioController.PlayFX(CommonFX.ClickButtonFX);
        fsm.IsPressed = true;
        fsm.MoveChildrenIcons(true);
        fsm.components.image.sprite = fsm.components.spritePressed;
    }

    public override void Released(ActionButtonFSM fsm) {
        fsm.MoveChildrenIcons(false);
        fsm.components.image.sprite = fsm.components.spriteEnabled;
        if (!fsm.IsPointerInside || !fsm.IsPressed) return;

        fsm.ChangeState(States.Disabled);

        // Execute ability and disable button
        fsm.CardFSM.abilityFSM.Execute(fsm.gameController, fsm.CardFSM, fsm.ActionDoneCallback,
            fsm.ActionCanceledCallback);
    }

    public override void Disable(ActionButtonFSM fsm) {
        fsm.ChangeState(States.Disabled);
    }
}

public class Disabled : State<ActionButtonFSM> {
    public override void Enter(ActionButtonFSM fsm) {
        fsm.MoveChildrenIcons(true);
        fsm.components.image.sprite = fsm.components.spritePressed;
        fsm.components.image.color = Colors.DISABLED;
        fsm.components.canvasGroup.alpha = 0.8f;
        fsm.components.icon.sprite = fsm.CardFSM.components.bwIcon;
    }

    public override void Exit(ActionButtonFSM fsm) {
        fsm.MoveChildrenIcons(false);
        fsm.components.image.sprite = fsm.components.spriteEnabled;
        fsm.components.canvasGroup.alpha = 1f;
        fsm.components.icon.sprite = fsm.CardFSM.components.originalIcon;
    }

    public override void Enable(ActionButtonFSM fsm) {
        if (fsm.Counter.Value > 0)
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
        if (fsm.card == Card.NONE) {
            fsm.components.icon.gameObject.SetActive(false);
            fsm.components.canvasGroup.alpha = 1f;
            fsm.components.image.gameObject.SetActive(false);
            fsm.components.consumableBox.SetActive(false);
        }
        else {
            fsm.components.icon.sprite = fsm.CardFSM.components.bwIcon;
        }
    }
}
}