using Core.Data;
using Core.StateMachine.Menu;
using Core.Utils.Constants;
using Framework.Base;

namespace Core.StateMachine.CharacterCards {

public abstract class States {
    public static readonly Created Created = new();
    public static readonly NotFound NotFound = new();
    public static readonly Selected Selected = new();
    public static readonly Unselected Unselected = new();
}

public class Created : State<CharacterCardFSM> {
    public override void Before(CharacterCardFSM fsm) {
        var selectedCharacter = PlayerDataV1.Instance.selectedCharacter;
        if (selectedCharacter == fsm.card) {
            SetupStars(fsm);
            fsm.ChangeState(States.Selected);
        }
        else if (CardsDataV1.Instance.HasCard(fsm.card)) {
            SetupStars(fsm);
            fsm.ChangeState(States.Unselected);
        }
        else {
            fsm.ChangeState(States.NotFound);
        }
    }

    private static void SetupStars(CharacterCardFSM fsm) {
        // for (var i = 0; i < FSM.cardFSM.power.stamina; i++) {
        //     FSM.components.starsStamina[i].color = ColorUtils.YELLOW;
        // }
        // for (var i = 0; i < FSM.cardFSM.power.life; i++) {
        //     FSM.components.starsLife[i].color = ColorUtils.YELLOW;
        // }
        // for (var i = 0; i < FSM.cardFSM.power.strength; i++) {
        //     FSM.components.starsStrength[i].color = ColorUtils.YELLOW;
        // }
        // for (var i = 0; i < FSM.cardFSM.power.magic; i++) {
        //     FSM.components.starsMagic[i].color = ColorUtils.YELLOW;
        // }
    }
}

public class NotFound : State<CharacterCardFSM> {
    public override void Enter(CharacterCardFSM fsm) {
        fsm.components.boxFound.SetActive(false);
        fsm.components.boxNotFound.SetActive(true);
        fsm.components.buttonFoundBox.interactable = false;
        fsm.components.buttonFoundBox.enabled = false;
    }
}

public class Selected : State<CharacterCardFSM> {
    public override void Enter(CharacterCardFSM fsm) {
        // fsm.components.boxFoundShadow.enabled = true;
        fsm.components.boxNotFound.SetActive(false);
        fsm.components.boxFoundImage.color = Colors.ENABLED_2;
        fsm.components.boxFoundCanvas.alpha = 1f;
        fsm.components.buttonAbility.enabled = true;
        fsm.components.icon.sprite = fsm.components.iconSprite;
        fsm.components.buttonFoundBox.interactable = false;
        fsm.components.buttonFoundBox.enabled = false;
    }

    public override void SyncData(CharacterCardFSM fsm) {
        var selectedCharacter = PlayerDataV1.Instance.selectedCharacter;
        if (selectedCharacter == fsm.card)
            fsm.ChangeState(States.Selected);
        else if (CardsDataV1.Instance.HasCard(fsm.card)) fsm.ChangeState(States.Unselected);
    }
}

public class Unselected : State<CharacterCardFSM> {
    public override void Enter(CharacterCardFSM fsm) {
        // fsm.components.boxFoundShadow.enabled = false;
        fsm.components.boxNotFound.SetActive(false);
        fsm.components.boxFoundImage.color = Colors.DISABLED;
        fsm.components.boxFoundCanvas.alpha = 0.5f;
        fsm.components.buttonAbility.enabled = false;
        fsm.components.icon.sprite = fsm.components.iconSpriteBW;
        fsm.components.buttonFoundBox.interactable = true;
        fsm.components.buttonFoundBox.enabled = true;
    }

    public override void Select(CharacterCardFSM fsm) {
        PlayerDataV1.Instance.SetSelectedCharacter(fsm.card);
        fsm.ChangeState(States.Selected);
        fsm.SyncAllData(typeof(CharacterCardFSM));
        fsm.SyncAllData(typeof(MenuFSM));
    }
}

}