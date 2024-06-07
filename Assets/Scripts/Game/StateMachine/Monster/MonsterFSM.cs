using System;
using System.Collections.Generic;
using Core.Utils.Constants;
using Framework.Base;
using Game.Controller.Game;
using Game.StateMachine.MonsterHB;
using Game.StateMachine.Rocks;
using Game.Utils;
using UnityEngine;

namespace Game.StateMachine.Monster {
public class MonsterFSM : StateMachine<MonsterFSM, MonsterState> {
    [SerializeField] public MonsterResourceType monsterResourceType = MonsterResourceType.Monster;
    [SerializeField] public MonsterType monsterType = MonsterType.Normal;
    [SerializeField] public RockPile rockPile = RockPile.None;
    [SerializeField] public float baseLife = 40;
    [SerializeField] public int baseDamage = 1;
    [SerializeField] public MonsterComponents components;
    protected override MonsterFSM FSM => this;
    protected override MonsterState GetInitialState => States.Created;

    internal float GetLife() {
        return Balancer.Instance.MonsterLife(baseLife);
    }

    internal int GetDamage() {
        return Balancer.Instance.MonsterDamage(baseDamage);
    }

    internal bool IsBoss() {
        return monsterType == MonsterType.Boss;
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
        return monsterInstance;
    }


    public void HitByARock(RockFSM rock) {
        State.Hit(this, rock);
    }

    public void RockHitByMonster(MonsterFSM monsterHittingFSM) {
        State.Hit(this, monsterHittingFSM);
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