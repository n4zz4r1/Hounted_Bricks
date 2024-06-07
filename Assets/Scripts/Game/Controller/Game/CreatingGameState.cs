using System;
using Core.Data;
using Core.Sprites;
using Core.StateMachine.Cards;
using Core.StateMachine.Stages;
using Core.Utils;
using Core.Utils.Constants;
using Game.StateMachine.Monster;
using Game.StateMachine.Players;
using Game.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Controller.Game {
public class CreatingGame : GameState {
    public override void Before(GameController fsm) {
        fsm.CurrentStage = StageFSM.GetCurrentStage();
        fsm.SaveRockSlot = PlayerDataV1.Instance.GetSavedRocks();

        // Preload All Game Assets
        // TODO Only preload assets that is going to be used
        AssetLoader.LoadAssetsByLabel("GameAssets", AfterPreload, fsm);
    }

    private static void AfterPreload(GameController fsm) {
        var playerStartPosition = new Vector2(3, -0.6f);
        var playerStartRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

        // Add player
        fsm.components.PlayerArea = new GameObject("Area: Player");
        fsm.components.PlayerArea.transform.SetParent(fsm.transform);
        fsm.components.GameArea = new GameObject("Area: Game");
        fsm.components.GameArea.transform.SetParent(fsm.transform);

        fsm.PlayerInGame = Object.Instantiate(AssetLoader
                .AsComponent<PlayerFSM>(PlayerDataV1.Instance.GetSelectedCharacter()),
            playerStartPosition, playerStartRotation, fsm.components.PlayerArea.transform);

        fsm.PlayerCardInGame = AssetLoader.AsComponent<CardFSM>(PlayerDataV1.Instance.GetSelectedCharacterCard());

        // Prepare Consumables
        fsm.AddGameResource(ResourceType.Heart, fsm.PlayerCardInGame.Attribute(CardAttributeType.Health));
        fsm.AddGameResource(ResourceType.Elixir, fsm.PlayerCardInGame.Attribute(CardAttributeType.Consumable));
        // fsm.gameResourcesAtStart.ForEach(r => r.Prepare(fsm));

        // Set Monsters
        CreateMonsters(fsm);

        // Reset level and buffers
        Balancer.Instance.RefreshLevelAndBuffs();

        fsm.FadeIn();
        fsm.ChangeState(States.PlayerTurn);
    }

    private static void CreateMonsters(GameController fsm) {
        // Preload all Prefabs
        // TODO to improve performance, load only prefabs within the game
        foreach (var monster in Enum.GetValues(typeof(Monsters.Monster)))
            fsm.MonstersPrefab[(Monsters.Monster)monster] = Resources.Load<GameObject>(monster.ToString());
        foreach (var monster in Enum.GetValues(typeof(Monsters.MonsterBoss)))
            fsm.BossesPrefab[(Monsters.MonsterBoss)monster] = Resources.Load<GameObject>(monster.ToString());

        // Create Monsters
        if (fsm.CurrentStage.properties.autoGenerate)
            CreateMonstersGeneric(fsm);
        else
            CreateMonstersFromProperties(fsm);

        // Set Monster grid
        fsm.MonsterGrid = new MonsterGrid(fsm.MonstersInGame, fsm.RockPileInGame);
    }

    private static void CreateMonstersFromProperties(GameController fsm) {
        var lastWave = 0f;
        // add wave of monsters
        foreach (var wave in fsm.CurrentStage.properties.waves) {
            if (wave.positionY > lastWave)
                lastWave = wave.positionY;

            foreach (var positionX in wave.positionX)
                fsm.MonstersInGame.Add(MonsterFSM.Create(
                    fsm.MonstersPrefab[wave.monster],
                    new Vector2(positionX, wave.positionY),
                    fsm.components.areaMonsters.transform).GetComponent<MonsterFSM>());
        }

        // Add boss if exist
        if (fsm.CurrentStage.properties.boss.boss != Monsters.MonsterBoss.NONE)
            fsm.MonstersInGame.Add(MonsterFSM.Create(
                fsm.BossesPrefab[fsm.CurrentStage.properties.boss.boss],
                new Vector2(fsm.CurrentStage.properties.boss.positionX, lastWave + 2), // last wave + two blocks
                fsm.components.areaMonsters.transform).GetComponent<MonsterFSM>());
    }

    private static void CreateMonstersGeneric(GameController fsm) {
        // TODO revisit here when balancing

        var level = fsm.CurrentStage.Level;

        // 1. Get number of waves
        var waves = StageGenerator.GetNumberOfWavesByLevel(level);

        // 2. Get difficulty, stageType and monsters
        // var difficulty = StageGenerator.GetDifficultyByLevel(level);
        var stageType = StageGenerator.GetStageTypeByLevel(level);

        // 3. get easy, medium and hard monsters by stage
        var monstersEasy = StageGenerator.monstersByLevelAndDifficulty[stageType][StageDifficultyType.EASY];
        var monstersMid = StageGenerator.monstersByLevelAndDifficulty[stageType][StageDifficultyType.MEDIUM];
        var monstersHard = StageGenerator.monstersByLevelAndDifficulty[stageType][StageDifficultyType.HARD];

        for (var i = 0; i < waves; i++) {
            // First wave y == 9, then 12 and so on
            var positionY = i == 0 ? 9 : 11 + i;
            var numberOfMonsters = StageGenerator.GetNumberOfMonstersOnWave(level);
            var monster = StageGenerator.ChooseMonster(level, monstersEasy, monstersMid, monstersHard);
            var positionsX = StageGenerator.ShuffleList(numberOfMonsters);
            foreach (var positionX in positionsX)
                fsm.MonstersInGame.Add(MonsterFSM.Create(
                    fsm.MonstersPrefab[monster],
                    new Vector2(positionX, positionY),
                    fsm.components.areaMonsters.transform).GetComponent<MonsterFSM>());
        }

        // 4. Check for Boss and proceed if has
        var boss = StageGenerator.GetBossFromLevel(level);
        if (boss == Monsters.MonsterBoss.NONE) return;

        var bossPositionY = waves + 12;
        var bossPositionX = StageGenerator.ShuffleListBoss();
        fsm.MonstersInGame.Add(MonsterFSM.Create(
            fsm.BossesPrefab[boss],
            new Vector2(bossPositionX, bossPositionY),
            fsm.components.areaMonsters.transform).GetComponent<MonsterFSM>());
    }
}
}