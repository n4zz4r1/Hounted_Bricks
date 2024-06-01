using System;
using System.Collections.Generic;
using Framework.Base;
using Game.Controller.Game;
using Game.StateMachine.Rocks;
using StateMachine.MonsterHB;
using UnityEngine;

namespace Game.StateMachine.Monster {

public class MonsterFSM : StateMachine<MonsterFSM, MonsterState> {
    [SerializeField] public MonsterResourceType monsterResourceType = Monster.MonsterResourceType.Monster;
    [SerializeField] public MonsterType monsterType = MonsterType.NORMAL;
    [SerializeField] public float life = 40;
    [SerializeField] public int damage = 1;
    [SerializeField] public MonsterComponents components;
    [SerializeField] public float forceFactor = 1f;
    [SerializeField] public float rockPileFactor = 1f;
    protected override MonsterFSM FSM => this;
    protected override MonsterState GetInitialState => States.Created;

    internal int GetLife() {
        return (int)(life * forceFactor);
    }

    internal bool IsBoss() {
        return monsterType == MonsterType.BOSS;
    }

    public void DestroyMonster() {
        State.Destroy(FSM);
    }

    // Sounds

    #region Sound

    [SerializeField] public List<AudioClip> impactFX;
    [SerializeField] public List<AudioClip> dyingFX;

    #endregion

    #region Static Build

    public static GameObject Create(GameObject prefab, Vector2 where, Transform area) {
        var monsterInstance = Instantiate(prefab, where, Quaternion.Euler(0.0f, 0.0f, 0.0f), area);
        // MonsterFSM monster = Instantiate(Resources.Load<MonsterFSM>(prefab), where, Quaternion.Euler(0.0f, 0.0f, 0.0f), monsterController.transform);
        // monster.forceFactor *= forceFactor;
        // monster.wasSummoned = isSummon;
        // monster.AuraType = auraType;
        // If monster is corner, rotate

        return monsterInstance;
    }

    public void HitByARock(RockFSM rock) {
        State.Hit(this, rock);
    }

    public void HitByAMonster(MonsterFSM monsterFSM) {
        State.Hit(this, monsterFSM);
    }

    protected override void Before() {
        GameController = GetComponentInParent<GameController>();
    }

    #endregion

    #region Parameters

    internal float CurrentLife { get; set; }
    internal GameController GameController { get; set; }
    public const float NormalSpeed = 5f;
    internal float MovementSpeed { get; set; } = NormalSpeed;
    internal Vector3 NextMove { get; set; }
    internal float NextMoveDistance { get; set; } = 1f;

    #endregion
}

[Serializable]
public class MonsterComponents {
    [SerializeField] public GameObject[] onFireBoxes;
    [SerializeField] public GameObject[] onPoisonBoxes;
    [SerializeField] public Animator animator;
    [SerializeField] public MonsterHBFSM healthyBar;
    [SerializeField] public Collider2D collider;
    [SerializeField] public RectTransform rectTransform;

    // [SerializeField] public Animator summonAnimator;
}

}