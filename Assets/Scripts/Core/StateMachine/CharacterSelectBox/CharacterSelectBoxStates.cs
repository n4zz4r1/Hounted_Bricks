using Core.Data;
using Core.StateMachine.CharacterCards;
using Core.StateMachine.Menu;
using Framework.Base;

namespace Core.StateMachine.CharacterSelectBox {

public abstract class States {
    public static readonly Preload Preload = new();
    public static readonly Selected Selected = new();
    public static readonly NotSelected NotSelected = new();
    public static readonly NotFound NotFound = new();
    public static readonly Dead Dead = new();
}

public class Preload : State<CharacterSelectBoxFSM> {
    public override void Enter(CharacterSelectBoxFSM fsm) {
        fsm.components.disabledBox.SetActive(false);
        fsm.components.enabledBox.SetActive(false);
        fsm.components.notFoundBox.SetActive(false);

        if (!CardsDataV1.Instance.HasCard(fsm.character)) {
            fsm.ChangeState(States.NotFound);
            return;
        }

        var hearts = PlayerDataV1.Instance.GetLife(fsm.character);
        fsm.RefreshHearts(hearts);

        if (hearts > 0)
            if (PlayerDataV1.Instance.selectedCharacter == fsm.character)
                fsm.ChangeState(States.Selected);
            else
                fsm.ChangeState(States.NotSelected);
        else
            fsm.ChangeState(States.Dead);
    }
}

public class NotFound : State<CharacterSelectBoxFSM> {
    public override void Enter(CharacterSelectBoxFSM fsm) {
        fsm.components.disabledBox.SetActive(false);
        fsm.components.enabledBox.SetActive(false);
        fsm.components.notFoundBox.SetActive(true);
        foreach (var componentsHeart in fsm.components.hearts)
            componentsHeart.gameObject.SetActive(false);
    }
}

public class Selected : State<CharacterSelectBoxFSM> {
    public override void Enter(CharacterSelectBoxFSM fsm) {
        fsm.components.enabledBox.SetActive(true);
        fsm.components.disabledBox.SetActive(false);
        fsm.components.notFoundBox.SetActive(false);
        var hearts = PlayerDataV1.Instance.GetLife(fsm.character);
        fsm.RefreshHearts(hearts);
    }

    // public override void Enter(CharacterSelectBoxFSM FSM)
    // {
    //     for (var i = 0; i < FSM.MaxHeart; i++)
    //         FSM.components.hearts[i].color = ColorUtils.WHITE;
    // }
    //
    // public override void TakeHit(CharacterSelectBoxFSM FSM)
    // {
    //     PlayerDataV1.Instance.RemoveLife(FSM.character);
    //     PlayerDataV1.Instance.SetLastMatchTime(FSM.character, TimeServices.now());
    //     FSM.LastDateTime = PlayerDataV1.Instance.GetLastMatchTime(FSM.character);
    //     FSM.ChangeState(States.SOME_HEARTS);
    // }
}

public class NotSelected : State<CharacterSelectBoxFSM> {
    public override void Enter(CharacterSelectBoxFSM fsm) {
        fsm.components.disabledBox.SetActive(true);
        fsm.components.enabledBox.SetActive(false);
        fsm.components.notFoundBox.SetActive(false);
        var hearts = PlayerDataV1.Instance.GetLife(fsm.character);
        fsm.RefreshHearts(hearts);
    }

    public override void Choose(CharacterSelectBoxFSM fsm) {
        PlayerDataV1.Instance.SetSelectedCharacter(fsm.character);
        fsm.SyncAllData(typeof(CharacterCardFSM));
        fsm.SyncAllData(typeof(MenuFSM));
        fsm.SyncAllData(typeof(CharacterSelectBoxFSM));
    }
    // public override void Enter(CharacterSelectBoxFSM FSM)
    // {
    //     var hearts = PlayerDataV1.Instance.GetLife(FSM.character);
    //     RefreshHearts(FSM, hearts);
    // }
    //
    // public override void Update(CharacterSelectBoxFSM FSM)
    // {
    //     FSM.UpdateTime();
    // }
    //
    // public override void TakeHit(CharacterSelectBoxFSM FSM)
    // {
    //     PlayerDataV1.Instance.RemoveLife(FSM.character);
    //     var hearts = PlayerDataV1.Instance.GetLife(FSM.character);
    //     if (hearts == 0)
    //         FSM.ChangeState(States.NO_HEARTS);
    //     else
    //         RefreshHearts(FSM, hearts);
    // }
    //
    // public override void AddLife(CharacterSelectBoxFSM FSM)
    // {
    //     PlayerDataV1.Instance.AddLife(FSM.character);
    //     var hearts = PlayerDataV1.Instance.GetLife(FSM.character);
    //     if (hearts == FSM.MaxHeart)
    //         FSM.ChangeState(States.FULL_HEARTS);
    //     else
    //         RefreshHearts(FSM, hearts);
    // }
    //
}

public class Dead : State<CharacterSelectBoxFSM> {
    public override void Enter(CharacterSelectBoxFSM fsm) {
        fsm.components.disabledBox.SetActive(true);
        fsm.components.enabledBox.SetActive(false);
        fsm.components.notFoundBox.SetActive(false);
        var hearts = PlayerDataV1.Instance.GetLife(fsm.character);
        fsm.RefreshHearts(hearts);
    }

    // public override void Enter(CharacterSelectBoxFSM FSM)
    // {
    //     for (var i = 0; i < FSM.MaxHeart; i++)
    //         FSM.components.hearts[i].color = ColorUtils.BLACK;
    // }
    //
    // public override void Update(CharacterSelectBoxFSM FSM)
    // {
    //     FSM.UpdateTime();
    // }
    //
    // public override void AddLife(CharacterSelectBoxFSM FSM)
    // {
    //     PlayerDataV1.Instance.AddLife(FSM.character);
    //     FSM.ChangeState(States.SOME_HEARTS);
    // }
}

}