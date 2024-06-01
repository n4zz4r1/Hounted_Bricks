using Game.StateMachine.ActionButton;
using Game.StateMachine.Rocks;
using UnityEngine;

namespace Game.Controller.Game {

public class Shooting : GameState {
    public override void Enter(GameController fsm) {
        fsm.SyncAllData(typeof(ActionButtonFSM));

        // fsm.components.collectButton.Active();
        //
        // // FSM.components.nextWaveActionButton.components.button.enabled = false;
        // fsm.components.nextWaveActionButton.Counter.Value = 1;
        // fsm.components.nextWaveActionButton.components.counter.text = "1";


        fsm.SpeedUp = false;
        fsm.GameShootingTime = 0;
        Shoot(fsm);
    }

    public override void DestroyRock(GameController fsm, RockFSM rockFSM) {
        var numberOfRocksDestroyed = fsm.CountRockDestroyed.Increment();
        // only update last position if is not collected
        if (!rockFSM.WasCollected)
            fsm.NextPlayerPosition = new Vector2(rockFSM.transform.position.x, fsm.NextPlayerPosition.y);

        if (numberOfRocksDestroyed != fsm.SaveRockSlot.Length) return;

        fsm.CountRockDestroyed.Value = 0;
        Stop(fsm);
    }


    public override void Shoot(GameController fsm) {
        var numberOfRocks = fsm.CountNunRockLaunched.Increment();

        if (numberOfRocks - 1 < fsm.SaveRockSlot.Length) {
            fsm.PlayerInGame.State.Shoot(fsm.PlayerInGame);
            var rock = RockFSM.Build(
                fsm.SaveRockSlot[numberOfRocks - 1],
                fsm.CardPrefabs[fsm.SaveRockSlot[numberOfRocks - 1]], fsm.ShootReleasePosition,
                fsm.PlayerInGame.transform.position,
                fsm.transform, fsm.components.mainCamera, 0, fsm.AbilityFactor, fsm);
            if (fsm.SpeedUp)
                rock.State.SpeedUp(rock);
            fsm.rocksInGame.Add(rock);
        }
        else {
            // When all rocks launched
            fsm.CountNunRockLaunched.Value = 0;
            fsm.PlayerInGame.State.Stop(fsm.PlayerInGame);
        }
    }

    public override void Collect(GameController fsm) {
        // fsm.components.collectButton.Inactive();
        fsm.PlayerInGame.State.Stop(fsm.PlayerInGame);
        fsm.CountNunRockLaunched.Value = 0;
        fsm.CountRockDestroyed.Value = 0;
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
        // fsm.components.collectButton.Inactive();
        // fsm.components.nextWaveActionButton.components.button.enabled = true;
    }
}

}