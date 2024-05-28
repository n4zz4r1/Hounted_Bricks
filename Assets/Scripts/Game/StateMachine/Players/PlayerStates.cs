using System;
using Core.Controller.Audio;
using Framework.Base;
using UnityEngine;

namespace Game.StateMachine.Players {

public abstract class States {
    public static readonly Created Created = new();
    public static readonly Idle Idle = new();
    public static readonly Aiming Aiming = new();
    public static readonly Shooting Shooting = new();
    public static readonly Moving Moving = new();
}

public class Created : State<PlayerFSM> {
    public override void Enter(PlayerFSM fsm) {
        var position = fsm.transform.position;
        fsm.nextMove = new Vector3(position.x, position.y, 0);
        fsm.ChangeState(States.Idle);
    }
}

public class Idle : State<PlayerFSM> {
    private const double Tolerance = 0.01d;

    public override void Move(PlayerFSM fsm, float x) {
        // Only move if necessary
        var newMove = new Vector3(x, fsm.transform.position.y, 0);
        if (Math.Abs(newMove.x - fsm.nextMove.x) < Tolerance) {
            fsm.gameController.State.StopMoving(fsm.gameController);
        }
        else {
            fsm.nextMove = newMove;
            fsm.ChangeState(States.Moving);
        }
    }

    // public override void TakeHit(PlayerFSM fsm)
    // {
    //     base.TakeHit(fsm); // TODO
    // }

    public override void Aim(PlayerFSM fsm) {
        fsm.ChangeState(States.Aiming);
    }
}

public class Aiming : State<PlayerFSM> {
    private static readonly int AimAnim = Animator.StringToHash("Aim");
    private static readonly int StopAnim = Animator.StringToHash("Stop");

    public override void Enter(PlayerFSM fsm) {
        fsm.components.animator.SetTrigger(AimAnim);
    }

    public override void Aim(PlayerFSM fsm) {
        var currentPosition = fsm.transform.position;
        // var newPosition = new Vector3(currentPosition.x, currentPosition.y, 0);
        var mousePosition = fsm.gameController.components.mainCamera.ScreenToWorldPoint(Input.mousePosition);
        var newPosition = new Vector3(mousePosition.x, mousePosition.y, 0);
        // Debug.Log("from vector " + newPosition + " to vector " + newPosition);
        fsm.components.aimLineHandler.CreateDottedAim(currentPosition, newPosition);
    }

    public override void Update(PlayerFSM fsm) {
        Aim(fsm);
    }

    public override void Shoot(PlayerFSM fsm) {
        fsm.components.aimLineHandler.DestroyAllDots();
        fsm.ChangeState(States.Shooting);
    }

    public override void Stop(PlayerFSM fsm) {
        fsm.components.aimLineHandler.DestroyAllDots();
        fsm.components.animator.SetTrigger(StopAnim);
        fsm.ChangeState(States.Idle);
    }
}

public class Shooting : State<PlayerFSM> {
    private static readonly int ShootAnim = Animator.StringToHash("Shoot");
    private static readonly int StopAnim = Animator.StringToHash("Stop");

    public override void Enter(PlayerFSM fsm) {
        // Shoot when starts
        fsm.components.animator.SetTrigger(ShootAnim);
        AudioController.PlayFXRandom(fsm.throwRockFX);
    }

    public override void Shoot(PlayerFSM fsm) {
        fsm.components.animator.SetTrigger(ShootAnim);
        AudioController.PlayFXRandom(fsm.throwRockFX);
    }

    public override void Stop(PlayerFSM fsm) {
        fsm.components.animator.SetTrigger(StopAnim);
        fsm.ChangeState(States.Idle);
    }
}

public class Moving : State<PlayerFSM> {
    private static readonly int StopAnim = Animator.StringToHash("Stop");

    public override void Enter(PlayerFSM fsm) {
        fsm.components.animator.SetTrigger(fsm.transform.position.x > fsm.nextMove.x ? "MoveLeft" : "MoveRight");
    }

    public override void Update(PlayerFSM fsm) {
        if (fsm.transform.position.Equals(fsm.nextMove))
            fsm.State.Stop(fsm);

        // Move(FSM);
        fsm.transform.position =
            Vector3.MoveTowards(fsm.transform.position, fsm.nextMove, fsm.movementSpeed * Time.deltaTime);
    }

    public override void Stop(PlayerFSM fsm) {
        fsm.components.animator.SetTrigger(StopAnim);
        fsm.ChangeState(States.Idle);
    }

    // When done moving, check if menu is on
    public override void Exit(PlayerFSM fsm) {
        // Update LastRockPosition
        var position = fsm.transform.position;
        fsm.gameController.NextPlayerPosition = new Vector3(position.x, position.y, position.z);
        fsm.gameController.State.StopMoving(fsm.gameController);
    }
}

}