using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.StateMachine.Cards;
using Core.StateMachine.Stages;
using Core.Utils;
using Core.Utils.Constants;
using DG.Tweening;
using Framework.Base;
using Game.Popup.GameMenu;
using Game.StateMachine.ActionButton;
using Game.StateMachine.GameResources;
using Game.StateMachine.Monster;
using Game.StateMachine.Players;
using Game.StateMachine.Rocks;
using Game.Utils;
using TMPro;
using UnityEngine;

namespace Game.Controller.Game {
public class GameController : Controller<GameController, GameState> {

    [SerializeField] public List<GameResourceFSM> gameResourcesAtStart;
    private readonly Dictionary<GameResource, GameResourceFSM> _gameResourcesDictionary = new();

    [SerializeField] public Components components;
    public List<RockFSM> rocksInGame = new();
    private readonly AtomicInt _monstersMoved = new(0);
    internal readonly AbilityFactor AbilityFactor = new();
    internal readonly Dictionary<Monsters.MonsterBoss, GameObject> BossesPrefab = new();
    public readonly AtomicInt CountNunRockLaunched = new(0);

    // Atomic Properties
    public readonly AtomicInt CountRockDestroyed = new(0);
    public readonly AtomicInt CurrentStars = new(1);
    internal readonly AtomicList<MonsterFSM> MonstersInGame = new();
    internal readonly Dictionary<Monsters.Monster, GameObject> MonstersPrefab = new();
    // internal readonly AtomicInt PlayerLife = new(3);
    internal readonly AtomicList<MonsterFSM> RockPileInGame = new();
    internal ActionButtonFSM[] ActionButtons;
    internal MonsterGrid MonsterGrid;
    internal Vector2 NextPlayerPosition = PlayerFSM.PlayerStartPosition;
    internal PlayerFSM PlayerInGame;
    internal CardFSM PlayerCardInGame;
    internal Vector2 ShootReleasePosition;

    protected override GameController FSM => this;
    protected override GameState GetInitialState => States.CreatingGame;

    public GameResourceFSM GetGameResource(GameResource gameResource) => _gameResourcesDictionary[gameResource];

    public void AddGameResource(Vector3 from, GameResource gameResource, int quantity) {
        if (!_gameResourcesDictionary.ContainsKey(gameResource))
            _gameResourcesDictionary.Add(gameResource, GameResourceFSM.Build(FSM, gameResource, _gameResourcesDictionary.Count - 2));
        
        _gameResourcesDictionary[gameResource].IncreaseWithEffect(from, quantity);
    }

    // Internal Properties
    internal StageFSM CurrentStage { get; set; }
    internal Card[] SaveRockSlot { get; set; }
    internal bool SpeedUp { get; set; }
    internal float GameShootingTime { get; set; } // time counter from start of first shoot to last
    internal float SpeedUpAfterSecs { get; set; } = 2f; // speed up starts after seconds 

    public void RemoveMonster(MonsterFSM monsterFSM) {
        switch (monsterFSM.monsterResourceType) {
            case MonsterResourceType.Monster:
                MonstersInGame.Remove(monsterFSM);

                if (MonstersInGame.Count == 0) {
                    DOVirtual.DelayedCall(1.5f, Victory);
                }
                    
                break;
            case MonsterResourceType.RockPile:
                RockPileInGame.Remove(monsterFSM);
                break;
            case MonsterResourceType.Chest:
                break;
        }
    }
    private void Victory() => ChangeState(States.Victory);
    private void Defeat() => ChangeState(States.Defeat);
    
    public void UpdateGrid() => MonsterGrid = new MonsterGrid(MonstersInGame, RockPileInGame);

    internal int MonsterMoved() {
        return _monstersMoved.Value;
    }

    internal void SetAllMonstersMoved() {
        _monstersMoved.Value = 0;
    }

    internal void MonsterMovementBegin() {
        FSM._monstersMoved.Value = FSM.MonstersInGame.Count;
    }

    public void ReduceLife(MonsterFSM monsterFSM) {
        MonstersInGame.Remove(monsterFSM);
        var defeated = GetGameResource(GameResource.Heart).Decrease(monsterFSM.damage);
        if (defeated) 
            DOVirtual.DelayedCall(0.2f, Defeat);
    }

    public void IncreaseMonstersMoved(MonsterFSM monsterFSM) {
        if (monsterFSM.monsterResourceType != MonsterResourceType.Monster) return;

        var moved = _monstersMoved.Decrement();
        // Debug.Log("["+ monstersMoved.Value +"] Monster "+ monsterFSM.monsterType +" moved to " + monsterFSM.gameObject.transform.position);

        // When all moved
        if (moved == 0)
            State.Next(FSM);
    }

    protected override void Before() {
        ActionButtons = GetComponentsInChildren<ActionButtonFSM>();

        foreach (var gameResourceFSM in gameResourcesAtStart) 
            _gameResourcesDictionary.Add(gameResourceFSM.type, gameResourceFSM);
    }

    // Monster type can be added at one or more stages
    public void AddRockPile(RockPile rockPile, Vector2 position) {
        RockPileInGame.Add(MonsterFSM.Create(AssetLoader.AsGameObject(rockPile), position, FSM.components.areaMonsters.transform)
            .GetComponent<MonsterFSM>());
        MonsterGrid = new MonsterGrid(MonstersInGame, RockPileInGame); // Uptate grid
    }

    // TODO Remove

    public override void ChangeStateBase() {
        components.currentState.text = State.ToString();
    }

    public void AimStart() {
        PlayerInGame.State.Aim(PlayerInGame);
    }

    public void AimExit() {
        PlayerInGame.State.Stop(PlayerInGame);
    }

    public void HideGameUI() {
        components.gameUIBox.SetActive(false);
    }

    public void ShowGameUI() {
        components.gameUIBox.SetActive(true);
    }


    public void StartShooting(Vector2 to) {
        if (PlayerInGame.State != StateMachine.Players.States.Aiming) return;

        // shootReleasePosition = Input.mousePosition;
        FSM.ShootReleasePosition = to;
        ChangeState(States.Shooting);
        PlayerInGame.State.Shoot(PlayerInGame);
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

    public MonsterFSM MonsterAtPosition(Vector2 position) {
        UpdateGrid();
        return MonsterGrid.MonsterAtPosition(position);
    }
}

[Serializable]
public class Components {
    [SerializeField] public GameObject areaMonsters;
    [SerializeField] public Transform gameResourceArea;
    [SerializeField] public GameObject areaPlayer;
    [SerializeField] public GameObject areaEndLine;
    [SerializeField] public GameMenuFSM gameMenu;
    [SerializeField] public GameObject gameUIBox;
    [SerializeField] public Camera mainCamera;
    [SerializeField] public GameObject aimTouchArea;

    // [SerializeField] public List<Image> hearts;
    // [SerializeField] public ActionButtonFSM nextWaveActionButton;
    // [SerializeField] public ActionButtonFSM collectButton;

    [SerializeField] public List<AudioClip> walkClip;

    // TODO DEV AREA, REMOVE
    [SerializeField] public TextMeshProUGUI currentState;
    internal GameObject GameArea;
    internal GameObject PlayerArea;
}
}