using Core.Controller.Audio;
using Game.Utils;

namespace Game.Controller.Game {

public class MonstersTurn : GameState {
    // TODO
    public override void Enter(GameController fsm) {
        AudioController.PlayFXRandom(fsm.components.walkClip);

        fsm.playerInGame.State.Move(fsm.playerInGame, fsm.nextPlayerPosition.x);
        fsm.components.aimTouchArea.SetActive(false);

        // TODO victory and lose
        if (fsm.monstersInGame.Count == 0) {
            fsm.ChangeState(States.Victory);
        }
        else {
            // Set monster grid for guidance
            fsm.monsterGrid = new MonsterGrid(fsm.monstersInGame);

            fsm.MonsterMovementBegin();
            // Move all monsters
            foreach (var monsterFSM in fsm.monstersInGame.ToList())
                monsterFSM.State.Move(monsterFSM, fsm.monsterGrid);
        }
    }

    public override void Next(GameController fsm) {
        // var amount = FSM.components.nextWaveActionButton.Counter.Subtract(1);
        // TODO remove print grid
        fsm.monsterGrid.PrintGrid();

        // Consume a wave, if its -1, consider it as zero
        var amount = fsm.components.nextWaveActionButton.Counter.Subtract(1);
        // Debug.Log("Consumed wave " + amount + " moved "  + FSM.MonsterMoved() + " with monsters in game " + FSM.monstersInGame.Count);

        // Victory if no monsters alive
        if (fsm.monstersInGame.Count <= 0 && fsm.playerLife.Value > 0)
            fsm.ChangeState(States.Victory);

        // When all moved
        else if (fsm.playerLife.Value <= 0)
            fsm.ChangeState(States.Defeat);


        if (amount <= 0) {
            // Debug.Log("changing to player turn");

            fsm.components.nextWaveActionButton.Counter.Value = 0;
            fsm.components.nextWaveActionButton.components.counter.text = "0";
            fsm.ChangeState(States.PlayerTurn);
        }
        else {
            // Debug.Log("keep moving");
            fsm.SetAllMonstersMoved();
            fsm.components.nextWaveActionButton.components.counter.text = amount.ToString();
            Enter(fsm);
        }
    }

    // public override void NextWave(GameController FSM)
    // {
    //     // var newWave = FSM.components.nextWaveActionButton.Counter.Add(1);
    //     // FSM.components.nextWaveActionButton.components.counter.text = newWave.ToString();
    // }
}

}