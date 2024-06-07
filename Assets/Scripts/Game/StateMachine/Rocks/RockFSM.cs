using System;
using Core.Controller.Audio;
using Core.Sprites;
using Core.StateMachine.Cards;
using Core.Utils;
using Core.Utils.Constants;
using Framework.Base;
using Game.Controller.Game;
using Game.StateMachine.Monster;
using Game.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.StateMachine.Rocks {
public class RockFSM : StateMachine<RockFSM, State<RockFSM>> {
    // Parameters
    [SerializeField] public Rock rock;

    // Number of collisions
    [SerializeField] internal int collisionCounter;
    [SerializeField] internal bool ignoreFirstCollision;

    [SerializeField] public RockComponents components;
    private float _baseDamage;
    private int _baseLife = 1;
    private float _playerBaseDamage;
    internal bool ExplosionEffect;
    internal float LastPositionX;
    internal float LastPositionY;
    protected override RockFSM FSM => this;
    protected override State<RockFSM> GetInitialState => States.Moving;

    public CardFSM CardFSM { get; set; }
    public bool WasCollected { get; set; }

    internal Vector3 CurrentDirection { get; set; }
    internal float CurrentRotation { get; set; }
    internal bool OnFire { get; set; }
    internal bool OnPoison { get; set; }

    internal float Speed { get; set; } = 9f;
    internal float SpeedCollect { get; set; } = 27f;

    public static RockFSM BuildWithRandomDirection(Rock rock, CardFSM cardFSM, Vector3 from, Transform parent,
        Camera mainCamera, float rotationFactor, GameController gameController) {
        // Build initial rotation
        var target = mainCamera.ScreenToWorldPoint(from);

        // Generate a random angle in radians
        var randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;

        // Calculate the random direction vector
        var randomDirection = new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle), 0).normalized;

        // Calculate the target position based on the random direction
        var to = target + randomDirection;

        // Rotation calculation based on the random direction
        var rotationZ = Mathf.Atan2(randomDirection.y, randomDirection.x) * Mathf.Rad2Deg;

        // Include the rotation factor
        rotationZ += rotationFactor;

        var rockObject = Instantiate(AssetLoader.AsComponent<RockFSM>(rock), to, Quaternion.Euler(0.0f, 0.0f, 0f),
            parent);
        rockObject.components.GameController = gameController;
        rockObject.components.spriteRenderer.sprite = cardFSM.components.cardIcon.sprite;
        rockObject.CardFSM = cardFSM;

        // Set base stone + player damages
        rockObject._baseDamage = cardFSM.Attribute(CardAttributeType.Power);
        rockObject._baseLife = cardFSM.Attribute(CardAttributeType.Resistance);
        rockObject._playerBaseDamage = gameController.PlayerCardInGame.Attribute(CardAttributeType.Power);

        rockObject.CurrentDirection = randomDirection.RotateVector(rotationFactor);
        // Set from and to attrs
        rockObject.CurrentRotation = rotationZ;

        return rockObject;
    }


    public static RockFSM BuildRandomTarget(Rock rock, CardFSM cardFSM, Vector3 from, Transform parent,
        Camera mainCamera, float rotationFactor, GameController gameController) {
        // Generate a random angle between 0 and 360 degrees
        var angle = Random.Range(0f, 360f);
        var direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0).normalized;
        var rotationZ = angle + rotationFactor;

        var rockObject = Instantiate(AssetLoader.AsComponent<RockFSM>(rock), from, Quaternion.Euler(0.0f, 0.0f, 0f),
            parent);
        rockObject.components.GameController = gameController;
        rockObject.components.spriteRenderer.sprite = cardFSM.components.cardIcon.sprite;
        rockObject.CardFSM = cardFSM;

        // Set base stone + player damages
        rockObject._baseDamage = cardFSM.Attribute(CardAttributeType.Power);
        rockObject._baseLife = cardFSM.Attribute(CardAttributeType.Resistance);
        rockObject._playerBaseDamage = gameController.PlayerCardInGame.Attribute(CardAttributeType.Power);

        rockObject.CurrentDirection = direction.RotateVector(rotationFactor);
        // Set from and to attrs
        rockObject.CurrentRotation = rotationZ;

        return rockObject;
    }


    // #region Static Build
    //
    public static RockFSM Build(Rock rock, CardFSM cardFSM, Vector3 end, Vector3 from, Transform parent,
        Camera mainCamera, float rotationFactor, GameController gameController) {
        // Build initial rotation
        var target = mainCamera.ScreenToWorldPoint(end);
        var difference = target - from;
        var rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        // Distance Between vertices
        var distance = difference.magnitude;
        var direction = difference / distance;
        direction.Normalize();

        // Include the rotation factor
        rotationZ += rotationFactor;

        var rockObject = Instantiate(AssetLoader.AsComponent<RockFSM>(rock), from, Quaternion.Euler(0.0f, 0.0f, 0f),
            parent);
        rockObject.components.GameController = gameController;
        rockObject.components.spriteRenderer.sprite = cardFSM.components.cardIcon.sprite;
        rockObject.CardFSM = cardFSM;

        // Set base stone + player damages
        rockObject._baseDamage = cardFSM.Attribute(CardAttributeType.Power);
        rockObject._baseLife = cardFSM.Attribute(CardAttributeType.Resistance);
        rockObject._playerBaseDamage = gameController.PlayerCardInGame.Attribute(CardAttributeType.Power);

        rockObject.CurrentDirection = direction.RotateVector(rotationFactor);
        // Set from and to attrs
        rockObject.CurrentRotation = rotationZ;

        return rockObject;
    }

    public static RockFSM BuildSecond(Rock rock, CardFSM cardFSM, Vector3 from, Vector3 to, Transform parent,
        Camera mainCamera, float rotationFactor, GameController gameController) {
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

        var rockObject = Instantiate(AssetLoader.AsComponent<RockFSM>(rock), to, Quaternion.Euler(0.0f, 0.0f, 0f),
            parent);
        rockObject.components.GameController = gameController;
        rockObject.components.spriteRenderer.sprite = cardFSM.components.cardIcon.sprite;
        rockObject.CardFSM = cardFSM;

        // Set base stone + player damages
        rockObject._baseDamage = cardFSM.Attribute(CardAttributeType.Power);
        rockObject._baseLife = cardFSM.Attribute(CardAttributeType.Resistance);
        rockObject._playerBaseDamage = gameController.PlayerCardInGame.Attribute(CardAttributeType.Power);

        rockObject.CurrentDirection = direction.RotateVector(rotationFactor);
        // Set from and to attrs
        rockObject.CurrentRotation = rotationZ;

        return rockObject;
    }

    internal float Damage() {
        return Balancer.Instance.StoneDamage(_baseDamage, _playerBaseDamage);
    }
    //
    // public void Collided(Collider2D other) {
    //     Debug.Log("OnTriggerEnter2D with " + other);
    //     if (other.CompareTag(Tags.EndLine))
    //         State.Destroy(this);
    //
    //     else if (other.CompareTag(Tags.Monster))
    //         // TODO hit monster
    //         HitMonster(other.gameObject);
    // }

    public void HitMonster(GameObject monster) {
        var monsterFSM = monster.GetComponent<MonsterFSM>();

        if (monsterFSM == null)
            monsterFSM = monster.GetComponentInParent<MonsterFSM>();

        // For R04_Bomb: also hit near by monster
        if (Rock.Bomb.Equals(rock)) {
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
        // if (Rock.Crooked.Equals(rock) || Rock.Bomb.Equals(rock) ||
        //     Rock.Arrowed.Equals(rock))
        if (--_baseLife <= 0) {
            Speed = 0f;
            ClearTrace();
            ExplosionEffect = true;
            State.Break(this);
        }
    }

    private void ClearTrace() {
        components.normalTrace.SetActive(false);
        components.fireTrace.SetActive(false);
        components.poisonTrace.SetActive(false);
    }

    public void DestroyRock() => State.Destroy(this);

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
}