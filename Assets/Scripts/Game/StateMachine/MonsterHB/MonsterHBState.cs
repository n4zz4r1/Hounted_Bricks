using DG.Tweening;
using Framework.Base;
using Game.StateMachine.Monster;
using UnityEngine;

namespace Game.StateMachine.MonsterHB {
public abstract class States {
    public static readonly Full FULL = new();
    public static readonly Damaged DAMAGED = new();
    public static readonly Destroyed DESTROYED = new();
}

public class Full : State<MonsterHBFSM> {
    public override void Enter(MonsterHBFSM FSM) {
        FSM.rawImage.enabled = false;
    }

    public override void Hit(MonsterHBFSM FSM, float damage) {
        FSM.ChangeState(States.DAMAGED);
        FSM.State.Hit(FSM, damage);
    }

    public override void Destroy(MonsterHBFSM FSM) {
        FSM.ChangeState(States.DESTROYED);
    }
}

public class Damaged : State<MonsterHBFSM> {
    public override void Enter(MonsterHBFSM FSM) {
        // Debug.Log("Bar Enter (Damaged)");
        FSM.rawImage.enabled = true;
    }

    public override void Hit(MonsterHBFSM FSM, float damage) {
        if (FSM.monsterFSM.CurrentLife <= 0) {
            FSM.ChangeState(States.DESTROYED);
        }
        else {
            var newLife = FSM.monsterFSM.CurrentLife / FSM.monsterFSM.GetLife();

            FSM.rect.DOSizeDelta(new Vector2(newLife, FSM.rect.sizeDelta.y), 0.1f); // Adjust the duration (0.5f) as needed

            // FSM.rect.sizeDelta = new Vector2(newLife, FSM.rect.sizeDelta.y);

            if (FSM.monsterFSM.monsterResourceType == MonsterResourceType.Monster) {
                if (newLife > 0.25 && newLife < 0.60)
                    FSM.rawImage.color = Color.yellow;
                else if (newLife <= 0.25) FSM.rawImage.color = Color.red;
            }
        }
    }

    public override void Destroy(MonsterHBFSM stateMachine) {
        stateMachine.ChangeState(States.DESTROYED);
    }
}

public class Destroyed : State<MonsterHBFSM> {
    public override void Enter(MonsterHBFSM stateMachine) {
        Object.Destroy(stateMachine.gameObject);
    }
}
}