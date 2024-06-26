﻿using System;
using Core.Controller.Audio;
using Core.Sprites;
using Core.StateMachine.Cards;
using Core.Utils;
using Core.Utils.Constants;
using DG.Tweening;
using Framework.Base;
using Game.StateMachine.Rocks;
using Game.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.StateMachine.Monster {
public abstract class MonsterState : State<MonsterFSM> {
    public virtual void Move(MonsterFSM fsm, MonsterGrid grid) { }
    public virtual void Hit(MonsterFSM fsm, RockFSM rockFSM) { }
    public virtual void Hit(MonsterFSM fsm, MonsterFSM monsterFSM) { }
}

public abstract class States {
    public static readonly Created Created = new();
    public static readonly Idle Idle = new();
    public static readonly Moving Moving = new();
    public static readonly Dying Dying = new();
    public static readonly ReachEndLine ReachEndLine = new();
    public static readonly CastingSummon CastingSummon = new();
    public static readonly CastingFireball CastingFireball = new();
}

public class ReachEndLine : MonsterState {
    public override void Enter(MonsterFSM fsm) {
        fsm.GameController.RemoveMonster(fsm);
        fsm.transform.DOKill();
        Object.Destroy(fsm.gameObject);
    }
}

public class Created : MonsterState {
    public override void Enter(MonsterFSM fsm) {
        // When monster is corner, rotate it
        if (fsm.components.rectTransform != null && fsm.monsterType == MonsterType.Corner) {
            var randomCorner = ProbabilityUtils.Instance.RandomCornerPosition();
            fsm.components.rectTransform.Rotate(new Vector3(0f, 0f, randomCorner.radius));
            fsm.components.rectTransform.localPosition = new Vector2(randomCorner.x, randomCorner.y);
        }

        fsm.ChangeState(States.Idle);
        fsm.CurrentLife = fsm.GetLife();
    }
}

public class Idle : MonsterState {
    private static readonly int HitAnim = Animator.StringToHash("Hit");

    public override void Move(MonsterFSM fsm, MonsterGrid grid) {
        var currentPosition = fsm.transform.position;

        // For monsters coming to the game (y position = 12), move 3 squares more
        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression

        // If there is a rock in front, damage it
        if (grid.HasRockInFront(fsm)) {
            grid.MonsterInFront(fsm).RockHitByMonster(fsm);
            fsm.components.animator.SetTrigger(HitAnim);
        }

        if (!grid.CanMonsterMove(fsm)) {
            fsm.GameController.IncreaseMonstersMoved(fsm);
            // If there is a BOSS above , kills it (hua hua hua)
            if (grid.HasBossAbove(fsm)) fsm.ChangeState(States.Dying);

            return;
        }

        if (Math.Abs(currentPosition.y - 12f) < 0.1f) {
            if (fsm.IsBoss()) {
                fsm.MovementSpeed = MonsterFSM.NormalSpeed * 4;
                fsm.NextMoveDistance = 4f;
            }
            else {
                fsm.MovementSpeed = MonsterFSM.NormalSpeed * 3;
                fsm.NextMoveDistance = 3f;
            }
        }
        else {
            if (currentPosition.y < 13 && fsm.monsterType == MonsterType.Fast && !grid.HasMonsterInFront(fsm)) {
                fsm.MovementSpeed = MonsterFSM.NormalSpeed * 2;
                fsm.NextMoveDistance = 2f;

                // Jump 5 houses when there is no monster on screen and only the boss
            }
            else if (fsm.monsterType == MonsterType.Boss && !grid.HasMonstersOnScene() &&
                     grid.MonsterFSMList.Count == 1) {
                fsm.MovementSpeed = MonsterFSM.NormalSpeed * 5;
                fsm.NextMoveDistance = 5f;
            }
            else {
                fsm.MovementSpeed = MonsterFSM.NormalSpeed;
                fsm.NextMoveDistance = 1f;
            }
        }

        fsm.NextMove = new Vector3(currentPosition.x, currentPosition.y - fsm.NextMoveDistance, 0);
        fsm.ChangeState(States.Moving);
    }

    public override void Hit(MonsterFSM fsm, RockFSM rockFSM) {
        //  when Ignore first collision, return
        if (rockFSM.ignoreFirstCollision) {
            Debug.Log("Ignoring first collision");
            rockFSM.ignoreFirstCollision = false;
            return;
        }

        Hit(fsm, rockFSM.Damage());
    }

    public override void Hit(MonsterFSM fsm, MonsterFSM _) {
        // In cases of monster hitting rocks, 
        Hit(fsm, fsm.GetLife() + 1);
    }

    public override void Hit(MonsterFSM fsm, float damage) {
        fsm.components.animator.SetTrigger(HitAnim);
        fsm.CurrentLife -= damage;

        if (fsm.rockPile is RockPile.GoldMine or RockPile.DiamondMine) {
            var card = fsm.rockPile is RockPile.GoldMine
                ? Card.Card_025_Ab_Lucas_GiveMeMoney
                : Card.Card_022_Ab_Lucas_SuperWall;
            var resource = fsm.rockPile is RockPile.GoldMine
                ? ResourceType.Coin
                : ResourceType.Diamond;

            if (Dices.Roll(AssetLoader.AsComponent<CardFSM>(card).Attribute(CardAttributeType.Probability)))
                fsm.GameController.AddGameResource(fsm.transform.position, resource, 1);
        }

        if (fsm.CurrentLife <= 0) {
            Kill(fsm);
        }

        else {
            AudioController.PlayFXRandom(fsm.impactFX);
            fsm.components.healthyBar.State.Hit(fsm.components.healthyBar, damage);
        }
    }


    //     // Damage should be rockLife * its factor
    //     var damage = ;
    //     fsm.components.animator.SetTrigger(HitAnim);
    //     fsm.CurrentLife -= 1 * damage;
    //
    //     if (fsm.CurrentLife <= 0) {
    //         Kill(fsm);
    //     }
    //     else {
    //         AudioController.PlayFXRandom(fsm.impactFX);
    //         fsm.components.healthyBar.State.Hit(fsm.components.healthyBar, damage);
    //     }
    // }

    public override void Kill(MonsterFSM fsm) {
        fsm.ChangeState(States.Dying);
    }
}

public class Moving : MonsterState {
    public override void Enter(MonsterFSM fsm) {
        fsm.components.collider.enabled = false;
    }

    public override void Move(MonsterFSM fsm) {
        fsm.transform.position =
            Vector2.MoveTowards(fsm.transform.position, fsm.NextMove, fsm.MovementSpeed * Time.deltaTime);
        if (!(Vector2.Distance(fsm.transform.position, fsm.NextMove) < 0.01f)) return;

        fsm.transform.position = fsm.NextMove;
        // If hits player, take life and destroy
        if (fsm.NextMove.y < -0.5f) {
            fsm.GameController.ReduceLife(fsm);

            fsm.ChangeState(States.ReachEndLine);
        }
        else {
            fsm.ChangeState(States.Idle);
        }

        fsm.GameController.IncreaseMonstersMoved(fsm);
        fsm.components.collider.enabled = true;
    }

    public override void Update(MonsterFSM fsm) {
        Move(fsm);
    }
}

public class Dying : MonsterState {
    private static readonly int KillAnim = Animator.StringToHash("Kill");

    public override void Enter(MonsterFSM fsm) {
        fsm.GameController.RemoveMonster(fsm);
        fsm.components.animator.SetTrigger(KillAnim);
        AudioController.PlayFXRandom(fsm.dyingFX);
    }

    public override void Destroy(MonsterFSM fsm) {
        fsm.transform.DOKill();
        Object.Destroy(fsm.gameObject);
    }
}

public class CastingSummon : MonsterState { }

public class CastingFireball : MonsterState { }

[Serializable]
public enum MonsterType {
    Normal,
    Fast,
    Corner,
    Shielded,
    Shaman,
    Mage,
    Boss
}

[Serializable]
public enum MonsterResourceType {
    Monster,
    RockPile,
    Chest
}
}