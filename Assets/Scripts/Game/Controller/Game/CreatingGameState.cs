using System;
using System.Collections.Generic;
using Core.Controllers.Game;
using Core.Data;
using Core.StateMachine.Stages;
using Core.Utils.Constants;
using Game.StateMachine.Monster;
using Game.StateMachine.Players;
using UnityEngine;

namespace Game.Controller.Game {

public class CreatingGame : GameState {
    public override void Before(GameController fsm) {
        fsm.CurrentStage = StageFSM.GetCurrentStage();

        // Add player
        fsm.components.playerArea = new GameObject("Area: Player");
        fsm.components.playerArea.transform.SetParent(fsm.transform);
        fsm.components.gameArea = new GameObject("Area: Game");
        fsm.components.gameArea.transform.SetParent(fsm.transform);
        fsm.playerInGame = PlayerFSM.Build(BMCharacter.Preset[PlayerDataV1.Instance.selectedCharacter],
            fsm.components.playerArea.transform);
        fsm.SaveRockSlot = PlayerDataV1.Instance.GetSavedRocks();

        CreateMonsters(fsm);

        Enter(fsm);
    }

    private static void CreateMonsters(GameController FSM) {
        // Preload all Prefabs
        // TODO to improve performance, load only prefabs within the game
        foreach (var monster in Enum.GetValues(typeof(Monsters.Monster)))
            FSM.monstersPrefab[(Monsters.Monster)monster] = Resources.Load<GameObject>(monster.ToString());
        foreach (var monster in Enum.GetValues(typeof(Monsters.MonsterBoss)))
            FSM.bossesPrefab[(Monsters.MonsterBoss)monster] = Resources.Load<GameObject>(monster.ToString());

        // Create Monsters
        if (FSM.CurrentStage.properties.autoGenerate)
            CreateMonstersGeneric(FSM);
        else
            CreateMonstersFromProperties(FSM);
    }

    private static void CreateMonstersFromProperties(GameController FSM) {
        var lastWave = 0f;
        // add wave of monsters
        foreach (var wave in FSM.CurrentStage.properties.waves) {
            if (wave.positionY > lastWave)
                lastWave = wave.positionY;

            foreach (var positionX in wave.positionX)
                FSM.monstersInGame.Add(MonsterFSM.Create(
                    FSM.monstersPrefab[wave.monster],
                    new Vector2(positionX, wave.positionY),
                    FSM.components.areaMonsters.transform).GetComponent<MonsterFSM>());
        }

        // Add boss if exist
        if (FSM.CurrentStage.properties.boss.boss != Monsters.MonsterBoss.NONE)
            FSM.monstersInGame.Add(MonsterFSM.Create(
                FSM.bossesPrefab[FSM.CurrentStage.properties.boss.boss],
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
                FSM.monstersInGame.Add(MonsterFSM.Create(
                    FSM.monstersPrefab[monster],
                    new Vector2(positionX, positionY),
                    FSM.components.areaMonsters.transform).GetComponent<MonsterFSM>());
        }

        // 4. Check for Boss and proceed if has
        var boss = StageGenerator.GetBossFromLevel(level);
        if (boss == Monsters.MonsterBoss.NONE) return;

        var bossPositionY = waves + 12;
        var bossPositionX = StageGenerator.ShuffleListBoss();
        FSM.monstersInGame.Add(MonsterFSM.Create(
            FSM.bossesPrefab[boss],
            new Vector2(bossPositionX, bossPositionY),
            FSM.components.areaMonsters.transform).GetComponent<MonsterFSM>());
    }

    public override void Enter(GameController FSM) {
        FSM.ChangeState(States.PlayerTurn);
    }
}

// TODO REMOVER
public static class BMCharacter {
    public static Dictionary<Card, string> Preset { get; } = new() {
        { Card.Card_005_Char_Lucas, "Player Lucas" },
        { Card.Card_006_Char_Lisa, "Player Lisa" },
        { Card.Card_007_Char_Bill, "Player Billy" }
    };
}

}