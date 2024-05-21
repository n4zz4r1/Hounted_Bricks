using Core.Data;
using Core.StateMachine.CharacterCards;
using Core.Utils.Constants;
using Framework.Base;

namespace Core.StateMachine.AbilitySlots {

public abstract class States {
    public static readonly Started Started = new();
    public static readonly Found Found = new();
    public static readonly InUse InUse = new();
    public static readonly NotFound NotFound = new();
}

public class Started : State<AbilitySlotFSM> {
    public override void Enter(AbilitySlotFSM fsm) {
        if (CardsDataV1.Instance.HasCard(fsm.cardFSM.cardId))
            fsm.ChangeState(States.Found);
        else
            fsm.ChangeState(States.NotFound);
    }
}

public class Found : State<AbilitySlotFSM> {
    public override void Enter(AbilitySlotFSM fsm) {
        fsm.components.boxFound.SetActive(true);
        fsm.components.boxNotFound.SetActive(false);
        fsm.components.icon.sprite = fsm.cardFSM.components.cardIcon.sprite;
        fsm.components.icon.color = Colors.WHITE;

        if (PlayerDataV1.Instance
            .GetAbilities(fsm.ParentPopup.characterCardFSM.cardId)
            .Contains(fsm.cardFSM.cardId))
            fsm.ChangeState(States.InUse);
    }

    public override void SyncData(AbilitySlotFSM fsm) {
        var characterCard = fsm.ParentPopup.characterCardFSM.cardId;
        if (PlayerDataV1.Instance.HasChosenAbility(characterCard, fsm.Tier, fsm.cardFSM.cardId))
            fsm.ChangeState(States.InUse);
    }

    public override void Choose(AbilitySlotFSM fsm) {
        PlayerDataV1.Instance.ChooseAbility(fsm.Tier, fsm.cardFSM.cardId, fsm.ParentPopup.characterCardFSM.cardId);
        fsm.ParentPopup.components.useButton.gameObject.SetActive(false);
        fsm.SyncAllData(typeof(CharacterCardFSM));
        fsm.SyncAllData(typeof(AbilitySlotFSM));
    }

    public override void Select(AbilitySlotFSM fsm) {
        fsm.Select();
    }

    public override void Unselect(AbilitySlotFSM fsm) {
        fsm.Unselect();
    }
}

public class InUse : State<AbilitySlotFSM> {
    public override void Enter(AbilitySlotFSM fsm) {
        fsm.components.iconSelected.color = Colors.PRIMARY;
        fsm.components.iconSelected.enabled = true;
    }

    public override void SyncData(AbilitySlotFSM fsm) {
        var characterCard = fsm.ParentPopup.characterCardFSM.cardId;
        if (!PlayerDataV1.Instance.HasChosenAbility(characterCard, fsm.Tier, fsm.cardFSM.cardId))
            fsm.ChangeState(States.Found);
    }

    public override void Exit(AbilitySlotFSM fsm) {
        fsm.components.iconSelected.enabled = false;
    }

    public override void Select(AbilitySlotFSM fsm) {
        fsm.Select();
    }

    public override void Unselect(AbilitySlotFSM fsm) {
        fsm.Unselect();
        fsm.components.iconSelected.color = Colors.PRIMARY;
        fsm.components.iconSelected.enabled = true;
    }
}

public class NotFound : State<AbilitySlotFSM> {
    public override void Enter(AbilitySlotFSM fsm) {
        fsm.components.boxFound.SetActive(true);
        fsm.components.boxNotFound.SetActive(false);
        fsm.components.icon.sprite = fsm.cardFSM.components.cardIcon.sprite;
    }

    public override void Select(AbilitySlotFSM fsm) {
        fsm.Select();
    }

    public override void Unselect(AbilitySlotFSM fsm) {
        fsm.Unselect();
    }
}

}