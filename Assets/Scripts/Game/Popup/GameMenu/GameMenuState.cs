using Core.Data;
using Framework.Base;
using UnityEngine;

namespace Game.Popup.GameMenu {

public abstract class States {
    public static readonly Playing Playing = new();
    public static readonly Paused Paused = new();
    public static readonly Victory Victory = new();
    public static readonly Defeat Defeat = new();
}

public class Playing : State<GameMenuFSM> {
    public override void Enter(GameMenuFSM fsm) {
        fsm.components.menuBox.SetActive(false);
    }

    public override void Win(GameMenuFSM fsm, int stars) {
        fsm.components.gameController.CurrentStars.Value = stars;
        fsm.ChangeState(States.Victory);
    }

    public override void Pause(GameMenuFSM fsm) {
        fsm.ChangeState(States.Paused);
    }
}

public class Paused : State<GameMenuFSM> {
    public override void Enter(GameMenuFSM fsm) {
        fsm.components.gameMenuTitle.text = fsm.GamePausedLabel;
        Time.timeScale = 0f;
        fsm.components.menuBox.SetActive(true);
    }

    public override void Unpause(GameMenuFSM fsm) {
        fsm.ChangeState(States.Playing);
    }

    public override void Leave(GameMenuFSM fsm) {
        Time.timeScale = 1f;
        if (fsm.components.gameController.CurrentStage.isMapStage)
            PlayerDataV1.Instance.RemoveLifeFromCurrentPlayer();

        fsm.components.gameController.TransitionWithEffectTo(fsm.components.gameController.CurrentStage.isMapStage
            ? "MapScene"
            : "PreloadScene");
    }

    public override void Exit(GameMenuFSM fsm) {
        Time.timeScale = 1f;
        fsm.components.menuBox.SetActive(false);
    }
}

public class Victory : State<GameMenuFSM> {
    public override void Enter(GameMenuFSM fsm) {
        fsm.components.menuBox.SetActive(true);
        fsm.components.buttonUnpause.gameObject.SetActive(false);
        fsm.components.buttonRestart.gameObject.SetActive(false);
        if (!fsm.stageFSM.isMapStage) {
            fsm.components.buttonNextLevel.gameObject.SetActive(true);
        }
        fsm.components.gameMenuTitle.text = fsm.YouWonLabel;
        GameDataV1.Instance.CompleteStage(fsm.components.gameController.CurrentStage,
            fsm.components.gameController.CurrentStars.Value);
        fsm.stageFSM.SetNextLevel();
    }

    public override void Leave(GameMenuFSM fsm) {
        Time.timeScale = 1f;
        fsm.components.gameController.TransitionWithEffectTo(fsm.components.gameController.CurrentStage.isMapStage
            ? "MapScene"
            : "PreloadScene");
    }
}

public class Defeat : State<GameMenuFSM> {
    public override void Enter(GameMenuFSM fsm) {
        fsm.components.menuBox.SetActive(true);
        fsm.components.buttonUnpause.gameObject.SetActive(false);
        fsm.components.gameMenuTitle.text = fsm.YouLoseLabel;
    }

    public override void Leave(GameMenuFSM fsm) {
        Time.timeScale = 1f;
        if (fsm.components.gameController.CurrentStage.isMapStage)
            PlayerDataV1.Instance.RemoveLifeFromCurrentPlayer();
        fsm.components.gameController.TransitionWithEffectTo(fsm.components.gameController.CurrentStage.isMapStage
            ? "MapScene"
            : "PreloadScene");
    }
}

}