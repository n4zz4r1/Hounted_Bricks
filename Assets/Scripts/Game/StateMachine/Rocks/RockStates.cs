using System;
using Core.Utils;
using Core.Utils.Constants;
using Framework.Base;
using UnityEngine;
using Object = UnityEngine.Object;

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
        fsm.lastPositionX = position.x;
        fsm.lastPositionY = position.y;

        // for rocks different than arrow, check if is on fire
        if (fsm.rockType != RockType.R03_ARROW) {
            // Bomb Fire Effect, GALisaFireBomb
            if (fsm.IsOnFire() || (fsm.components.GameController.abilityFactor.FireBombEffect &&
                                   fsm.rockType == RockType.R04_BOMB))
                fsm.SetOnFire();

            // Poison Fire Effect, Billy
            else if (fsm.IsOnPoison() || (fsm.components.GameController.abilityFactor.AcidBombEffect &&
                                          fsm.rockType == RockType.R04_BOMB))
                fsm.SetOnPoison();
        }

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

    public override void Rotate(RockFSM fsm) {
        var transform = fsm.transform;
        var position = transform.position;
        var newDir = new Vector3(position.x, position.y, 0);
        var newDirValue = Mathf.Atan2(newDir.y - fsm.lastPositionY, newDir.x - fsm.lastPositionX);
        var newDirValueDeg = (int)(57.295777f * newDirValue);

        if (newDirValueDeg != 0 && Math.Abs(fsm.components.rotationDegrees - newDirValueDeg) > Tolerance) {
            fsm.transform.rotation = Quaternion.Euler(0, 0, newDirValueDeg);
            fsm.components.rotationDegrees = newDirValueDeg;
        }

        var transform1 = fsm.transform;
        var position1 = transform1.position;
        fsm.lastPositionX = position1.x;
        fsm.lastPositionY = position1.y;
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

        var destiny = new Vector3(fsm.components.GameController.playerInGame.transform.position.x, -2, 0);

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
        fsm.components.GameController.DestroyRock(fsm);
        if (fsm.explosionEffect) {
            fsm.components.rigidBodyBouncer.velocity = Vector3.zero;
            fsm.components.rigidBodyBouncer.angularVelocity = 0f;
            fsm.components.rigidBodyBouncer.Sleep();
            fsm.components.animator.SetTrigger(DestroyAnim);
        }
        else {
            Object.Destroy(fsm.gameObject);
        }
    }

    public override void Destroy(RockFSM fsm) {
        Object.Destroy(fsm.gameObject);
    }
}

}