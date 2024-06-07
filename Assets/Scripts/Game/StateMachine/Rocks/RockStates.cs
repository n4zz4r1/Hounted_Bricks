using System;
using Core.StateMachine.Cards;
using Core.Utils;
using Core.Utils.Constants;
using Framework.Base;
using Game.Utils;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Game.StateMachine.Rocks {
public abstract class States {
    public static readonly Moving Moving = new();
    public static readonly Collected Collected = new();
    public static readonly Destroyed Destroyed = new();
}

public class Moving : State<RockFSM> {
    private const float Tolerance = 0.1f;

    public override void Enter(RockFSM fsm) {
        var position = fsm.transform.position;
        fsm.LastPositionX = position.x;
        fsm.LastPositionY = position.y;

        fsm.components.rigidBodyBouncer.AddForce(fsm.CurrentDirection * 0.1f);
    }

    public override void Update(RockFSM fsm) {
        fsm.components.rigidBodyBouncer.velocity = fsm.Speed * fsm.components.rigidBodyBouncer.velocity.normalized;
        fsm.State.Rotate(fsm); // keep rotating
    }

    public override void SpeedUp(RockFSM fsm) {
        fsm.Speed *= 2;
    }

    public override void Destroy(RockFSM fsm) {
        fsm.ChangeState(States.Destroyed);
    }

    public override void Break(RockFSM fsm) {
        // Check for multiply buffs
        if (fsm.rock is Rock.Crooked or Rock.Round or Rock.Arrowed && Balancer.Instance.HasBuff(Buff.StoneDivided))
            // Debug.Log($"Rock broke {Balancer.Instance.GetBuff(BuffItem.StoneDivided)} times");
            for (var i = 0; i < Balancer.Instance.GetBuff(Buff.StoneDivided); i++) {
                var randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
                var randomDirection = new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle), 0).normalized;
                var from = fsm.transform
                    .position; //fsm.PlayerInGame.transform.position ;var randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
                var rock = RockFSM.BuildRandomTarget(
                    fsm.rock == Rock.Crooked ? Rock.Broken : Rock.BrokeArrowed,
                    AssetLoader.AsComponent<CardFSM>(fsm.CardFSM.cardId == Card.Card_001_Crooked_Rock
                        ? Card.Card_101_Broken_Rock
                        : Card.Card_102_Broke_Arrowed),
                    from,
                    fsm.components.GameController.transform, fsm.components.GameController.components.mainCamera, 0,
                    fsm.components.GameController);

                // var rock = RockFSM.BuildWithRandomDirection(Rock.Broken,
                //     AssetLoader.AsComponent<CardFSM>(Card.Card_001_Crooked_Rock),
                //     fsm.transform.position, fsm.components.GameController.transform, fsm.components.GameController.components.mainCamera, 
                //     0, fsm.components.GameController);
                // fsm.components.GameController.CountRockDestroyed.Subtract(1);
                fsm.components.GameController.RocksInGame.Add(rock);
            }

        fsm.ChangeState(States.Destroyed);
    }

    public override void Rotate(RockFSM fsm) {
        var transform = fsm.transform;
        var position = transform.position;
        var newDir = new Vector3(position.x, position.y, 0);
        var newDirValue = Mathf.Atan2(newDir.y - fsm.LastPositionY, newDir.x - fsm.LastPositionX);
        var newDirValueDeg = (int)(57.295777f * newDirValue);

        if (newDirValueDeg != 0 && Math.Abs(fsm.components.rotationDegrees - newDirValueDeg) > Tolerance) {
            fsm.transform.rotation = Quaternion.Euler(0, 0, newDirValueDeg);
            fsm.components.rotationDegrees = newDirValueDeg;
        }

        var transform1 = fsm.transform;
        var position1 = transform1.position;
        fsm.LastPositionX = position1.x;
        fsm.LastPositionY = position1.y;
    }

    public override void Collect(RockFSM stateMachine) {
        stateMachine.ChangeState(States.Collected);
    }
}

public class Collected : State<RockFSM> {
    public override void Enter(RockFSM fsm) {
        fsm.WasCollected = true;
        // Stop ball first
        fsm.components.rigidBodyBouncer.velocity = Vector3.zero;
        fsm.components.rigidBodyBouncer.angularVelocity = 0f;
        fsm.components.rigidBodyBouncer.Sleep();
        fsm.components.fireTrace.SetActive(false);
        fsm.components.normalTrace.SetActive(false);
        fsm.components.poisonTrace.SetActive(false);
        fsm.components.spriteRenderer.color = Colors.WithAlpha(fsm.components.spriteRenderer.color, 0.5f);

        fsm.gameObject.layer = Layers.IgnoreCollision;
        fsm.components.collider.gameObject.layer = Layers.IgnoreCollision;

        var destiny = new Vector3(fsm.components.GameController.PlayerInGame.transform.position.x, -2, 0);

        var difference = destiny - fsm.transform.position;

        // Distance Between vertices
        var distance = difference.magnitude;
        Vector2 direction = difference / distance;
        direction.Normalize();

        fsm.CurrentDirection = direction;

        fsm.components.rigidBodyBouncer.AddForce(direction * 0.1f);
    }

    public override void Update(RockFSM fsm) {
        fsm.components.rigidBodyBouncer.velocity =
            fsm.SpeedCollect * fsm.components.rigidBodyBouncer.velocity.normalized;
        fsm.State.Rotate(fsm); // keep rotating
    }

    public override void Destroy(RockFSM stateMachine) {
        stateMachine.ChangeState(States.Destroyed);
    }
}

public class Destroyed : State<RockFSM> {
    private static readonly int DestroyAnim = Animator.StringToHash("destroy");

    public override void Enter(RockFSM fsm) {
        // FSM.GameController.RockDestroyed(FSM, FSM.transform.position, FSM.gameObject.layer == BMLayer.IgnoreCollision);
        if (fsm.ExplosionEffect) {
            fsm.components.rigidBodyBouncer.velocity = Vector3.zero;
            fsm.components.rigidBodyBouncer.angularVelocity = 0f;
            fsm.components.rigidBodyBouncer.Sleep();
            fsm.components.animator.SetTrigger(DestroyAnim);
        }
        else 
            Destroy(fsm);
    }

    public override void Destroy(RockFSM fsm) {
        fsm.components.GameController.DestroyRock(fsm);
        Object.Destroy(fsm.gameObject);
    }
}
}