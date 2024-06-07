using System;
using System.Collections;
using Core.StateMachine.Cards;
using Core.Utils;
using Core.Utils.Constants;
using Framework.Base;
using Game.StateMachine.ActionButton;
using Game.StateMachine.Rocks;
using Game.Utils;
using UnityEngine;

namespace Game.Controller.Game {
public class Shooting : GameState {
    public override void Enter(GameController fsm) {
        fsm.SyncAllData(typeof(ActionButtonFSM));
        fsm.RockThrowCounter.Value = 0;
        fsm.RocksInGame = new AtomicList<RockFSM>();

        fsm.SpeedUp = false;
        fsm.GameShootingTime = 0;
        Shoot(fsm);
    }

    public override void DestroyRock(GameController fsm, RockFSM rockFSM) {
        var rocksLeft = fsm.RocksInGame.RemoveCounter(rockFSM);
        // only update last position if is not collected
        if (!rockFSM.WasCollected)
            fsm.NextPlayerPosition = new Vector2(rockFSM.transform.position.x, fsm.NextPlayerPosition.y);

        Debug.Log($"Rock Destroyed. Left {rocksLeft} and thrown value: {fsm.RockThrowCounter.Value}");
        if (rocksLeft == 0 && fsm.RockThrowCounter.Value == fsm.SaveRockSlot.Length + 1)
            Stop(fsm);
        
    }

    public override void Shoot(GameController fsm) {
        // var numberOfRocks = fsm.CountNunRockLaunched.Increment();
        var rockIndex = fsm.RockThrowCounter.Increment() - 1;

        if (rockIndex < fsm.SaveRockSlot.Length) {
            fsm.PlayerInGame.State.Shoot(fsm.PlayerInGame);
            var rock = RockFSM.Build(
                (Rock)fsm.SaveRockSlot[rockIndex],
                AssetLoader.AsComponent<CardFSM>(fsm.SaveRockSlot[rockIndex]),
                fsm.ShootReleasePosition,
                fsm.PlayerInGame.transform.position,
                fsm.transform, fsm.components.mainCamera, 0, fsm);
            fsm.RocksInGame.Add(rock);

            // For Multistrike, throw more rocks than usual
            if (Balancer.Instance.DiceBuff(Buff.LucasMultiStrike)) {
                var newRock = RockFSM.Build(
                    (Rock)fsm.SaveRockSlot[rockIndex],
                    AssetLoader.AsComponent<CardFSM>(fsm.SaveRockSlot[rockIndex]),
                    Dices.RotateRandomly(fsm.PlayerInGame.transform.position, fsm.ShootReleasePosition),
                    fsm.PlayerInGame.transform.position,
                    fsm.transform, fsm.components.mainCamera, 0, fsm);

                fsm.RocksInGame.Add(newRock);
            }
            
        }
        else {
            // When all rocks launched
            fsm.PlayerInGame.State.Stop(fsm.PlayerInGame);
        }
    }

    public override void Collect(GameController fsm) {
        // fsm.components.collectButton.Inactive();
        fsm.PlayerInGame.State.Stop(fsm.PlayerInGame);
        fsm.RocksInGame.ToList().ForEach(rock => rock.State.Collect(rock));
        fsm.ChangeState(States.MonstersTurn);
    }

    public override void Stop(GameController fsm) {
        fsm.ChangeStateWithCoroutine(States.MonstersTurn);
    }

    public override void SpeedUp(GameController fsm) {
        fsm.SpeedUp = true;
        fsm.RocksInGame.ToList().ForEach(rock => rock.State.SpeedUp(rock));
        // TODO needed?
    }

    public override void Update(GameController fsm) {
        if (fsm.SpeedUp) return;

        fsm.GameShootingTime += Time.deltaTime;
        // if (fsm.GameShootingTime > fsm.SpeedUpAfterSecs && !fsm.SpeedUp)
        // SpeedUp(fsm);
    }

    public override IEnumerator ExitAsync(GameController fsm, Action callback) {
        yield return new WaitForSeconds(0.2f);
        callback?.Invoke();
    }
}
}