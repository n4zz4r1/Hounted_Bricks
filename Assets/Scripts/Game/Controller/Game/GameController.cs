using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.StateMachine.ActionButton;
using Core.StateMachine.Cards;
using Core.StateMachine.Stages;
using Core.Utils;
using Core.Utils.Constants;
using Framework.Base;
using Game.Popup.GameMenu;
using Game.StateMachine.Monster;
using Game.StateMachine.Players;
using Game.StateMachine.Rocks;
using Game.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Controller.Game {

public class GameController : Controller<GameController, GameState> {
    [SerializeField] public Components components;
    public List<RockFSM> rocksInGame = new();
    internal readonly AbilityFactor abilityFactor = new();
    internal readonly Dictionary<Monsters.MonsterBoss, GameObject> bossesPrefab = new();
    public readonly AtomicInt countNumRockLaunched = new(0);

    // Atomic Properties
    public readonly AtomicInt countRockDestroyed = new(0);
    public readonly AtomicInt currentStars = new(1);
    internal readonly AtomicList<MonsterFSM> monstersInGame = new();
    private readonly AtomicInt monstersMoved = new(0);
    internal readonly Dictionary<Monsters.Monster, GameObject> monstersPrefab = new();
    internal readonly AtomicInt playerLife = new(3);
    internal ActionButtonFSM[] actionButtons;
    internal MonsterGrid monsterGrid;
    internal Vector2 nextPlayerPosition = PlayerFSM.PLAYER_START_POSITION;
    internal PlayerFSM playerInGame;
    internal Vector2 shootReleasePosition;

    protected override GameController FSM => this;
    protected override GameState GetInitialState => States.CreatingGame;

    // Internal Properties
    internal StageFSM CurrentStage { get; set; }
    internal Card[] SaveRockSlot { get; set; }
    internal bool SpeedUp { get; set; }
    internal float GameShootingTime { get; set; } // time counter from start of first shoot to last
    internal float SpeedUpAfterSecs { get; set; } = 2f; // speed up starts after seconds 
    public Dictionary<Card, CardFSM> CardPrefabDictionary { get; set; } = new();

    internal int MonsterMoved() {
        return monstersMoved.Value;
    }

    internal void SetAllMonstersMoved() {
        monstersMoved.Value = 0;
    }

    internal void MonsterMovementBegin() {
        FSM.monstersMoved.Value = FSM.monstersInGame.Count;
    }

    public void ReduceLife(MonsterFSM monsterFSM) {
        var reduced = playerLife.Subtract(monsterFSM.damage);
        monstersInGame.Remove(monsterFSM);
        // monstersAlive.Decrement();
        for (var i = 0; i < components.hearts.Count; i++)
            if (i >= reduced)
                components.hearts[i].color = Color.black;
    }

    public void IncreaseMonstersMoved(MonsterFSM monsterFSM) {
        var moved = monstersMoved.Decrement();
        // Debug.Log("["+ monstersMoved.Value +"] Monster "+ monsterFSM.monsterType +" moved to " + monsterFSM.gameObject.transform.position);

        // When all moved
        if (moved == 0)
            State.Next(FSM);
    }

    protected override async Task BeforeAsync() {
        CardPrefabDictionary.Add(Card.Card_001_Crooked_Rock,
            await AssetLoader<CardFSM, ResourceType>.LoadCardFSM(Card.Card_001_Crooked_Rock));
        CardPrefabDictionary.Add(Card.Card_002_Rounded_Rock,
            await AssetLoader<CardFSM, ResourceType>.LoadCardFSM(Card.Card_002_Rounded_Rock));
        CardPrefabDictionary.Add(Card.Card_003_Arrowed_Rock,
            await AssetLoader<CardFSM, ResourceType>.LoadCardFSM(Card.Card_003_Arrowed_Rock));
        CardPrefabDictionary.Add(Card.Card_004_Bomb_Rock,
            await AssetLoader<CardFSM, ResourceType>.LoadCardFSM(Card.Card_004_Bomb_Rock));

        actionButtons = GetComponentsInChildren<ActionButtonFSM>();

        components.nextWaveActionButton.components.button.onClick.AddListener(() => {
            var amount = components.nextWaveActionButton.Counter.Add(1);
            components.nextWaveActionButton.components.counter.text = amount.ToString();
            State.NextWave(FSM);
        });
        components.collectButton.components.button.onClick.AddListener(() => State.Collect(FSM));
    }

    // protected override void Before()
    // {
    //
    // }

    // TODO Remove
    public override void ChangeStateBase() {
        components.currentState.text = State.ToString();
    }

    public void AimStart() {
        playerInGame.State.Aim(playerInGame);
    }

    public void AimExit() {
        // TODO implement
        playerInGame.State.Stop(playerInGame);
    }

    public void StartShooting(Vector2 to) {
        if (playerInGame.State != StateMachine.Players.States.Aiming) return;

        // shootReleasePosition = Input.mousePosition;
        FSM.shootReleasePosition = to;
        ChangeState(States.Shooting);
        playerInGame.State.Shoot(playerInGame);
        // // TODO implement
        // throw new NotImplementedException();
    }

    // public void DestroyRock(RockFSM FSM, Vector3 transformPosition, bool b)
    public void NextShoot() {
        State.Shoot(FSM);
    }

    public void DestroyRock(RockFSM rockFSM) {
        State.DestroyRock(FSM, rockFSM);
    }
}

[Serializable]
public class Components {
    [SerializeField] public GameObject areaMonsters;
    [SerializeField] public GameObject areaPlayer;
    [SerializeField] public GameObject areaEndLine;
    [SerializeField] public GameMenuFSM gameMenu;
    [SerializeField] public Camera mainCamera;
    [SerializeField] public GameObject aimTouchArea;
    [SerializeField] public List<Image> hearts;
    [SerializeField] public ActionButtonFSM nextWaveActionButton;
    [SerializeField] public ActionButtonFSM collectButton;

    [SerializeField] public List<AudioClip> walkClip;

    // TODO DEV AREA, REMOVE
    [SerializeField] public TextMeshProUGUI currentState;
    internal GameObject gameArea;

    internal GameObject playerArea;
}

}