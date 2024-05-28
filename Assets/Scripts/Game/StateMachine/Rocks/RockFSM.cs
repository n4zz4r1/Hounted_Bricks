using System;
using Core.Controller.Audio;
using Core.StateMachine.Cards;
using Core.Utils;
using Core.Utils.Constants;
using Framework.Base;
using Game.Controller.Game;
using Game.StateMachine.Monster;
using Game.Utils;
using UnityEngine;

namespace Game.StateMachine.Rocks {

public enum RockType {
    R01_ROUND,
    R02_CROOKED,
    R03_ARROW,
    R04_BOMB,
    E1_FIREBALL
}

public class RockFSM : StateMachine<RockFSM, State<RockFSM>> {
    // Parameters
    [SerializeField] public RockType rockType;
    [SerializeField] public float rockDamage;
    [SerializeField] public float life = 1;

    // Number of collisions
    [SerializeField] internal int collisionCounter;

    [SerializeField] public RockComponents components;
    internal bool explosionEffect;

    internal float lastPositionX;
    internal float lastPositionY;
    protected override RockFSM FSM => this;
    protected override State<RockFSM> GetInitialState => States.Moving;

    public Card Card { get; set; }
    public bool WasCollected { get; set; }

    internal Vector3 CurrentDirection { get; set; }
    internal float CurrentRotation { get; set; }
    private SpriteRenderer SpriteRenderer { get; }
    internal bool OnFire { get; set; }
    internal bool OnPoison { get; set; }

    internal float Speed { get; set; } = 9f;
    internal float SpeedCollect { get; set; } = 27f;

    private void OnCollisionEnter(Collision other) {
        Debug.Log("OnCollisionEnter with " + other);

        throw new NotImplementedException();
    }

    // #region Static Build
    //
    public static RockFSM Build(Card card, CardFSM cardFSM, Vector3 from, Vector3 to, Transform parent,
        Camera mainCamera, float rotationFactor, AbilityFactor abilityFactor, GameController gameController) {
        // Build initial rotation
        var target = mainCamera.ScreenToWorldPoint(from);
        var difference = target - to;
        var rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        // Distance Between vertices
        var distance = difference.magnitude;
        var direction = difference / distance;
        direction.Normalize();

        // Include the rotation factor
        rotationZ += rotationFactor;

        // BillyAcidBomb effect
        // TODO acid 
        // if (abilityFactor.AcidBombEffect && BMPrefabName.From(card) == "Rock04 Bomb")
        //     prefab = "Rock04 Bomb Acid";

        var rock = Instantiate(Resources.Load<RockFSM>(BMPrefabName.From(card)), to, Quaternion.Euler(0.0f, 0.0f, 0f),
            parent);
        rock.components.GameController = gameController;
        rock.components.spriteRenderer.sprite = cardFSM.components.cardIcon.sprite;
        rock.Card = card;

        // Change layer if rock party
        if (abilityFactor.RockPartyEffect)
            rock.gameObject.layer = Layers.RockParty;

        rock.CurrentDirection = direction.RotateVector(rotationFactor);
        // Set from and to attrs
        rock.CurrentRotation = rotationZ;

        if (abilityFactor.FireBallEffect && rotationFactor == 0) rock.transform.localScale = new Vector2(0.8f, 0.8f);

        return rock;
    }

    // Total damage = Rock Damage + Player Factor 
    internal float Damage() {
        return rockDamage * components.GameController.AbilityFactor.DamageFactor;
    }

    public void Collided(Collider2D other) {
        Debug.Log("OnTriggerEnter2D with " + other);
        if (other.CompareTag(Tags.EndLine))
            State.Destroy(this);

        else if (other.CompareTag(Tags.Monster))
            // TODO hit monster
            HitMonster(other.gameObject);
    }

    public void HitMonster(GameObject monster) {
        var monsterFSM = monster.GetComponent<MonsterFSM>();

        if (monsterFSM == null)
            monsterFSM = monster.GetComponentInParent<MonsterFSM>();

        // Combo fire effect, GALisaFireCombo
        // TODO
        // if (GameController.abilityFactor.ComboFireEffect && collisionCounter++ > 4)
        //     SetOnFire();

        // For R04_Bomb: also hit near by monster
        if (RockType.R04_BOMB.Equals(rockType)) {
            var monsterInRange =
                Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y),
                    1.5f); // TODO Layer not working
            foreach (var monsterCollider in monsterInRange)
                if (monsterCollider.gameObject.CompareTag(Tags.Monster) &&
                    monsterCollider.GetComponent<MonsterFSM>() != null)
                    monsterCollider.GetComponent<MonsterFSM>().HitByARock(this);
        }
        else {
            monsterFSM.HitByARock(this);
        }

        // // For R04_Bomb and R02_Crooked: Destroy after hit
        if (RockType.R02_CROOKED.Equals(rockType) || RockType.R04_BOMB.Equals(rockType) ||
            RockType.R03_ARROW.Equals(rockType))
            if (--life <= 0) {
                Speed = 0f;
                ClearTrace();
                explosionEffect = true;
                State.Destroy(this);
            }
    }

    internal bool IsOnFire() {
        return OnFire = components.GameController.AbilityFactor.RockOnFire ||
                        ProbabilityUtils.Instance.Between100(components.GameController.AbilityFactor
                            .RockOnFireFactor);
    }

    internal bool IsOnPoison() {
        return OnPoison = components.GameController.AbilityFactor.RockOnPoisonTurn ||
                          ProbabilityUtils.Instance.Between100(components.GameController.AbilityFactor
                              .RockOnPoisonFactor);
    }

    public void ClearTrace() {
        components.normalTrace.SetActive(false);
        components.fireTrace.SetActive(false);
        components.poisonTrace.SetActive(false);
    }

    public void DestroyRock() {
        State.Destroy(this);
    }

    internal void SetOnFire() {
        OnFire = true;
        components.normalTrace.SetActive(false);
        components.fireTrace.SetActive(true);
        components.poisonTrace.SetActive(false);
        if (rockType != RockType.R04_BOMB)
            SpriteRenderer.color = Colors.FIRE;
    }

    internal void SetOnPoison() {
        OnPoison = true;
        components.normalTrace.SetActive(false);
        components.fireTrace.SetActive(false);
        components.poisonTrace.SetActive(true);
        if (rockType != RockType.R04_BOMB)
            SpriteRenderer.color = Colors.ROCK_ON_POISON;
    }

    public void WallSound() {
        AudioController.PlayFX(components.hitWallFX);
    }
}

[Serializable]
public class RockComponents {
    [SerializeField] public SpriteRenderer spriteRenderer;
    [SerializeField] public Rigidbody2D rigidBodyBouncer;
    [SerializeField] public Animator animator;
    [SerializeField] public GameObject normalTrace;
    [SerializeField] public GameObject fireTrace;
    [SerializeField] public GameObject poisonTrace;
    [SerializeField] public Collider2D collider;

    [SerializeField] public AudioClip hitWallFX;

    // rotation
    [SerializeField] internal float rotationDegrees = -1f;
    internal GameController GameController;
}

// TODO REMOVE
public static class BMPrefabName {
    public static string From(Card card) {
        // ReSharper disable once ConvertSwitchStatementToSwitchExpression
        switch (card) {
            case Card.Card_001_Crooked_Rock:
                return "Rock01 Crooked";
            case Card.Card_002_Rounded_Rock:
                return "Rock02 Round";
            case Card.Card_003_Arrowed_Rock:
                return "Rock03 Arrow";
            case Card.Card_004_Bomb_Rock:
                return "Rock04 Bomb";
            default:
                throw new Exception("Prefab for card " + card + " not found");
        }
    }
}

}