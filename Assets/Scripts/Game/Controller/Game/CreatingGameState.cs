using System;
using System.Collections.Generic;
using Core.Controllers.Game;
using Core.Data;
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

        // Add player
        fsm.components.PlayerArea = new GameObject("Area: Player");
        fsm.components.PlayerArea.transform.SetParent(fsm.transform);
        fsm.components.GameArea = new GameObject("Area: Game");
        fsm.components.GameArea.transform.SetParent(fsm.transform);
        //
        // fsm.PlayerInGame = PlayerFSM.Build(BMCharacter.Preset[PlayerDataV1.Instance.selectedCharacter],
        //     fsm.components.PlayerArea.transform);

        fsm.SaveRockSlot = PlayerDataV1.Instance.GetSavedRocks();
        
        // Set Monsters
        CreateMonsters(fsm);
        

        // Set Player
        AssetLoader<Player>.Load<GameController, PlayerFSM>(PlayerDataV1.Instance.GetSelectedCharacter(), fsm, SetPlayer);
        // Enter(fsm);
    }

    private void SetPlayer(PlayerFSM prefab, GameController fsm) {
        var playerStartPosition = new Vector2(3, -0.6f);
        var playerStartRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

        fsm.PlayerInGame = Object.Instantiate(prefab, playerStartPosition, playerStartRotation, fsm.components.PlayerArea.transform);

        Enter(fsm);
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

    private static void CreateMonstersFromProperties(GameController FSM) {
        var lastWave = 0f;
        // add wave of monsters
        foreach (var wave in FSM.CurrentStage.properties.waves) {
            if (wave.positionY > lastWave)
                lastWave = wave.positionY;

            foreach (var positionX in wave.positionX)
                FSM.MonstersInGame.Add(MonsterFSM.Create(
                    FSM.MonstersPrefab[wave.monster],
                    new Vector2(positionX, wave.positionY),
                    FSM.components.areaMonsters.transform).GetComponent<MonsterFSM>());
        }

        // Add boss if exist
        if (FSM.CurrentStage.properties.boss.boss != Monsters.MonsterBoss.NONE)
            FSM.MonstersInGame.Add(MonsterFSM.Create(
                FSM.BossesPrefab[FSM.CurrentStage.properties.boss.boss],
                new Vector2(FSM.CurrentStage.properties.boss.positionX, lastWave + 2), // last wave + two blocks
                FSM.components.areaMonsters.transform).GetComponent<MonsterFSM>());
    }

    private static void CreateMonstersGeneric(GameController FSM) {
        // TODO revisit here when balancing

        var level = FSM.CurrentStage.Level;

        // 1. Get number of waves
        var waves = StageGenerator.GetNumberOfWavesByLevel(level);

        // 2. Get difficulty, stageType and monsters
        var difficulty = StageGenerator.GetDifficultyByLevel(level);
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
                FSM.MonstersInGame.Add(MonsterFSM.Create(
                    FSM.MonstersPrefab[monster],
                    new Vector2(positionX, positionY),
                    FSM.components.areaMonsters.transform).GetComponent<MonsterFSM>());
        }

        // 4. Check for Boss and proceed if has
        var boss = StageGenerator.GetBossFromLevel(level);
        if (boss == Monsters.MonsterBoss.NONE) return;

        var bossPositionY = waves + 12;
        var bossPositionX = StageGenerator.ShuffleListBoss();
        FSM.MonstersInGame.Add(MonsterFSM.Create(
            FSM.BossesPrefab[boss],
            new Vector2(bossPositionX, bossPositionY),
            FSM.components.areaMonsters.transform).GetComponent<MonsterFSM>());
    }

    public override void Enter(GameController FSM) {
        FSM.ChangeState(States.PlayerTurn);
    }
}

}