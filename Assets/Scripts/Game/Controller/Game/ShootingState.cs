using Core.StateMachine.ActionButton;
using Game.StateMachine.Rocks;
using UnityEngine;

namespace Game.Controller.Game {

public class Shooting : GameState {
    public override void Enter(GameController fsm) {
        fsm.SyncAllData(typeof(ActionButtonFSM));

        fsm.components.collectButton.Active();

        // FSM.components.nextWaveActionButton.components.button.enabled = false;
        fsm.components.nextWaveActionButton.Counter.Value = 1;
        fsm.components.nextWaveActionButton.components.counter.text = "1";


        fsm.SpeedUp = false;
        fsm.GameShootingTime = 0;
        Shoot(fsm);
    }

    public override void DestroyRock(GameController fsm, RockFSM rockFSM) {
        var numberOfRocksDestroyed = fsm.countRockDestroyed.Increment();
        // only update last position if is not collected
        if (!rockFSM.WasCollected)
            fsm.nextPlayerPosition = new Vector2(rockFSM.transform.position.x, fsm.nextPlayerPosition.y);

        if (numberOfRocksDestroyed != fsm.SaveRockSlot.Length) return;

        fsm.countRockDestroyed.Value = 0;
        Stop(fsm);
    }


    public override void Shoot(GameController fsm) {
        var numberOfRocks = fsm.countNumRockLaunched.Increment();

        if (numberOfRocks - 1 < fsm.SaveRockSlot.Length) {
            fsm.playerInGame.State.Shoot(fsm.playerInGame);
            var rock = RockFSM.Build(
                fsm.SaveRockSlot[numberOfRocks - 1],
                fsm.CardPrefabDictionary[fsm.SaveRockSlot[numberOfRocks - 1]], fsm.shootReleasePosition,
                fsm.playerInGame.transform.position,
                fsm.transform, fsm.components.mainCamera, 0, fsm.abilityFactor, fsm);
            if (fsm.SpeedUp)
                rock.State.SpeedUp(rock);
            fsm.rocksInGame.Add(rock);
        }
        else {
            // When all rocks launched
            fsm.countNumRockLaunched.Value = 0;
            fsm.playerInGame.State.Stop(fsm.playerInGame);
        }
    }

    public override void Collect(GameController fsm) {
        fsm.components.collectButton.Inactive();
        fsm.playerInGame.State.Stop(fsm.playerInGame);
        fsm.countNumRockLaunched.Value = 0;
        fsm.countRockDestroyed.Value = 0;
        fsm.rocksInGame.ForEach(rock => rock.State.Collect(rock));
        fsm.ChangeState(States.MonstersTurn);
    }

    public override void Stop(GameController fsm) {
        fsm.ChangeState(States.MonstersTurn);
    }

    public override void SpeedUp(GameController fsm) {
        fsm.SpeedUp = true;
        fsm.rocksInGame.ForEach(rock => rock.State.SpeedUp(rock));
        // TODO needed?
    }

    public override void Update(GameController fsm) {
        if (fsm.SpeedUp) return;

        fsm.GameShootingTime += Time.deltaTime;
        if (fsm.GameShootingTime > fsm.SpeedUpAfterSecs && !fsm.SpeedUp)
            SpeedUp(fsm);
    }

    public override void Exit(GameController fsm) {
        // Always disable collect button when shooting end
        fsm.components.collectButton.Inactive();
        fsm.components.nextWaveActionButton.components.button.enabled = true;
    }
}

}