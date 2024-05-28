using Framework.Base;
using Game.StateMachine.ActionButton;
using Game.StateMachine.Rocks;

namespace Game.Controller.Game {

public abstract class GameState : State<GameController> {
    public virtual void DestroyRock(GameController fsm, RockFSM rockFSM) { }
}

public abstract class States {
    public static readonly CreatingGame CreatingGame = new();
    public static readonly PlayerTurn PlayerTurn = new();
    public static readonly Shooting Shooting = new();
    public static readonly MonstersTurn MonstersTurn = new();
    public static readonly Victory Victory = new();
    public static readonly Defeat Defeat = new();
}

public class PlayerTurn : GameState {
    public override void Enter(GameController fsm) {
        fsm.SyncAllData(typeof(ActionButtonFSM));
        fsm.components.aimTouchArea.SetActive(true);
        // FSM.monstersMoved.Value = 0;
        // TODO
        // fsm.components.nextWaveActionButton.components.button.enabled = true;
        // fsm.components.collectButton.Inactive();
    }

    public override void NextWave(GameController fsm) {
        fsm.ChangeState(States.MonstersTurn);
    }

    public override void Exit(GameController fsm) {
        // FSM.components.nextWaveButton.enabled = false;
    }
}

public class Victory : GameState {
    public override void Enter(GameController fsm) {
        fsm.components.gameMenu.ChangeState(Popup.GameMenu.States.Victory);
    }
}

public class Defeat : GameState {
    public override void Enter(GameController fsm) {
        fsm.components.gameMenu.ChangeState(Popup.GameMenu.States.Defeat);
    }
}

}