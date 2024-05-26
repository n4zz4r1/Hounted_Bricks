using Core.Data;
using Core.Popup.StagePopup;
using Core.Services;
using Core.StateMachine.CharacterSelectBox;
using Core.Utils.Constants;
using Framework.Base;

namespace Core.StateMachine.CharacterLifeBar {

public abstract class CharacterLifeBarState : State<CharacterLifeBarFSM> {
    public virtual void TakeHit(CharacterLifeBarFSM fsm) { }
    public virtual void AddLife(CharacterLifeBarFSM fsm) { }
}

public abstract class States {
    public static readonly Created Created = new();
    public static readonly FullHearts FullHearts = new();
    public static readonly SomeHearts SomeHearts = new();
    public static readonly NoHearts NoHearts = new();
    public static readonly NotFound NotFound = new();
}

public class Created : CharacterLifeBarState {
    public override void Enter(CharacterLifeBarFSM fsm) {
        if (!CardsDataV1.Instance.HasCard(fsm.character)) {
            fsm.ChangeState(States.NotFound);
            return;
        }

        var hearts = PlayerDataV1.Instance.GetLife(fsm.character);
        fsm.LastDateTime = PlayerDataV1.Instance.GetLastMatchTime(fsm.character);

        if (hearts == 0)
            fsm.ChangeState(States.NoHearts);
        else if (hearts == fsm.MaxHeart)
            fsm.ChangeState(States.FullHearts);
        else
            fsm.ChangeState(States.SomeHearts);
    }
}

public class NotFound : CharacterLifeBarState {
    public override void Enter(CharacterLifeBarFSM fsm) {
        foreach (var componentsHeart in fsm.components.hearts)
            componentsHeart.gameObject.SetActive(false);

        fsm.components.elapsedTime.gameObject.SetActive(false);
        fsm.components.characterEnabledIcon.color = Colors.BLACK;
    }
}

public class FullHearts : CharacterLifeBarState {
    public override void Enter(CharacterLifeBarFSM fsm) {
        for (var i = 0; i < fsm.MaxHeart; i++)
            fsm.components.hearts[i].color = Colors.WHITE;
        fsm.components.elapsedTime.text = "";
        fsm.components.characterEnabledIcon.gameObject.SetActive(true);
        fsm.components.characterDisabledIcon.gameObject.SetActive(false);
    }

    public override void TakeHit(CharacterLifeBarFSM fsm) {
        PlayerDataV1.Instance.RemoveLife(fsm.character);
        PlayerDataV1.Instance.SetLastMatchTime(fsm.character, TimeServices.Now());
        fsm.LastDateTime = PlayerDataV1.Instance.GetLastMatchTime(fsm.character);
        fsm.ChangeState(States.SomeHearts);
        fsm.SyncAllData(typeof(StagePopup));
        fsm.SyncAllData(typeof(CharacterSelectBoxFSM));
    }
}

public class SomeHearts : CharacterLifeBarState {
    public override void Enter(CharacterLifeBarFSM fsm) {
        fsm.components.characterEnabledIcon.gameObject.SetActive(true);
        fsm.components.characterDisabledIcon.gameObject.SetActive(false);

        var hearts = PlayerDataV1.Instance.GetLife(fsm.character);
        RefreshHearts(fsm, hearts);
    }

    public override void Update(CharacterLifeBarFSM fsm) {
        fsm.UpdateTime();
    }

    public override void TakeHit(CharacterLifeBarFSM fsm) {
        PlayerDataV1.Instance.RemoveLife(fsm.character);
        var hearts = PlayerDataV1.Instance.GetLife(fsm.character);
        if (hearts == 0)
            fsm.ChangeState(States.NoHearts);
        else
            RefreshHearts(fsm, hearts);
        fsm.SyncAllData(typeof(StagePopup));
        fsm.SyncAllData(typeof(CharacterSelectBoxFSM));
    }

    public override void AddLife(CharacterLifeBarFSM fsm) {
        PlayerDataV1.Instance.AddLife(fsm.character);
        var hearts = PlayerDataV1.Instance.GetLife(fsm.character);
        if (hearts == fsm.MaxHeart)
            fsm.ChangeState(States.FullHearts);
        else
            RefreshHearts(fsm, hearts);

        fsm.SyncAllData(typeof(StagePopup));
        fsm.SyncAllData(typeof(CharacterSelectBoxFSM));
    }

    private static void RefreshHearts(CharacterLifeBarFSM fsm, int hearts) {
        for (var i = 0; i < hearts; i++)
            fsm.components.hearts[i].color = Colors.WHITE;
        for (var i = hearts; i < fsm.MaxHeart; i++)
            fsm.components.hearts[i].color = Colors.BLACK;
    }
}

public class NoHearts : CharacterLifeBarState {
    public override void Enter(CharacterLifeBarFSM fsm) {
        for (var i = 0; i < fsm.MaxHeart; i++)
            fsm.components.hearts[i].color = Colors.BLACK;

        fsm.components.characterEnabledIcon.gameObject.SetActive(false);
        fsm.components.characterDisabledIcon.gameObject.SetActive(true);
    }

    public override void Update(CharacterLifeBarFSM fsm) {
        fsm.UpdateTime();
    }

    public override void AddLife(CharacterLifeBarFSM fsm) {
        PlayerDataV1.Instance.AddLife(fsm.character);
        fsm.ChangeState(States.SomeHearts);
        fsm.SyncAllData(typeof(StagePopup));
        fsm.SyncAllData(typeof(CharacterSelectBoxFSM));
    }
}

}