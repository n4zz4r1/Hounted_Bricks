using System.Collections.Generic;
using System.Linq;
using Framework.Base;
using Game.StateMachine.Monster;
using UnityEngine;

namespace Game.Utils {
public class MonsterGrid {
    private readonly Dictionary<int, Dictionary<int, MonsterFSM>> _monsterGridDictionary = new();

    private readonly int _nextRowWithMonster;

    // private int _wavesTotal;
    private readonly int _wavesLeft;
    private MonsterFSM[][] _monsterGrid;

    public MonsterGrid(AtomicList<MonsterFSM> monsterFSMListParam, AtomicList<MonsterFSM> rockPileList) {
        var monsterFSMList = new List<MonsterFSM>();
        monsterFSMList.AddRange(monsterFSMListParam.ToList());
        monsterFSMList.AddRange(rockPileList.ToList());

        if (monsterFSMList.Count == 0)
            return;

        // 1. Find waves left number
        _wavesLeft = (int)monsterFSMList.ToList().Max(fsm => fsm.transform.position.y) + 1;
        _nextRowWithMonster = (int)monsterFSMList.ToList().Min(fsm => fsm.transform.position.y) + 1;

        // 2. Set null for all grid
        for (var i = 0; i < _wavesLeft + 1; i++) {
            Dictionary<int, MonsterFSM> xAxis = new();
            for (var j = 0; j < 6; j++) xAxis.Add(j, null);
            _monsterGridDictionary.Add(i, xAxis);
        }

        MonsterFSMList = monsterFSMList.ToList();

        // 3. insert monster on current grid
        foreach (var monsterFSM in MonsterFSMList) {
            var position = monsterFSM.transform.position;
            if (monsterFSM.monsterType != MonsterType.BOSS) {
                _monsterGridDictionary[(int)position.y][(int)position.x] = monsterFSM;
            }
            else {
                _monsterGridDictionary[(int)position.y][(int)position.x] = monsterFSM;
                _monsterGridDictionary[(int)position.y][(int)position.x + 1] = monsterFSM;
                _monsterGridDictionary[(int)position.y + 1][(int)position.x] = monsterFSM;
                _monsterGridDictionary[(int)position.y + 1][(int)position.x + 1] = monsterFSM;
            }
        }

        // Print grid 
    }

    public List<MonsterFSM> MonsterFSMList { get; set; }

    public bool HasMonsterInFront(MonsterFSM monsterFSM) {
        var position = monsterFSM.transform.position;
        return _monsterGridDictionary.ContainsKey((int)position.y - 1)
               && _monsterGridDictionary?[(int)position.y - 1]?[(int)position.x] != null;
    }
    
    public MonsterFSM MonsterAtPosition(Vector2 position) {
        return _monsterGridDictionary.ContainsKey((int)position.y) 
               && _monsterGridDictionary?[(int)position.y]?[(int)position.x] != null 
            ? _monsterGridDictionary?[(int)position.y]?[(int)position.x] : null;
    }
    public MonsterFSM MonsterInFront(MonsterFSM monsterFSM) {
        var position = monsterFSM.transform.position;
        return _monsterGridDictionary?[(int)position.y - 1]?[(int)position.x];
    }

    public bool HasRockInFront(MonsterFSM monsterFSM) {
        var position = monsterFSM.transform.position;
        return _monsterGridDictionary.ContainsKey((int)position.y - 1)
               && _monsterGridDictionary?[(int)position.y - 1]?[(int)position.x] != null
               && _monsterGridDictionary?[(int)position.y - 1]?[(int)position.x].monsterResourceType ==
               MonsterResourceType.RockPile;
    }

    public bool HasBossAbove(MonsterFSM monsterFSM) {
        var position = monsterFSM.transform.position;
        return monsterFSM.monsterType != MonsterType.BOSS && _monsterGridDictionary.ContainsKey((int)position.y + 1)
                                                          && _monsterGridDictionary?[(int)position.y + 1]?[
                                                              (int)position.x] != null
                                                          && _monsterGridDictionary?[(int)position.y + 1]?[
                                                              (int)position.x].monsterType == MonsterType.BOSS;
    }

    public bool HasMonstersOnScene() {
        return _nextRowWithMonster <= 10;
    }

    // Monsters that are type `RockPile` or are blocked by one, should not move.
    public bool CanMonsterMove(MonsterFSM fsm) {
        if (fsm.monsterResourceType == MonsterResourceType.RockPile) return false;
        if (fsm.monsterType == MonsterType.BOSS) return true;

        return !IsMonsterBlockedByRock(fsm);
    }

    private bool IsMonsterBlockedByRock(MonsterFSM fsm) {
        if (HasRockInFront(fsm)) return true;
        // if there is no rock, check if next the same for next monster
        return HasMonsterInFront(fsm) && IsMonsterBlockedByRock(MonsterInFront(fsm));
    }

    public bool HasMonstersOnRow(int row) {
        for (var i = 0; i < 6; i++)
            if (_monsterGridDictionary[row][i] != null)
                return true;

        return false;
    }

    public void PrintGrid() {
        var grid = $"[GRID LOG] Waves left: {_wavesLeft}, Next With Monsters: {_nextRowWithMonster} \n";

        for (var i = 10; i >= 0; i--) {
            if (!_monsterGridDictionary.ContainsKey(i)) continue;
            grid += $"Line {i} : ";
            for (var j = 0; j < 6; j++) {
                grid += _monsterGridDictionary[i][j] == null ? "-" :
                    _monsterGridDictionary[i][j].monsterResourceType == MonsterResourceType.RockPile ? "0" :
                    _monsterGridDictionary[i][j].monsterType == MonsterType.BOSS ? "B" : "X";
                grid += " ";
            }

            grid += "\n";
        }

        //
        // grid += "\n" + _wavesLeft + "";
        // grid += "\n next wave with monster: " + _nextRowWithMonster + "";
        Debug.Log(grid);
    }

    // public void FullPrintGrid() {
    //     var grid = "[GRID LOG] \n";
    //
    //     for (var i = 1; i < _wavesLeft; i++) {
    //         grid += "Line (y) : " + i + " - ";
    //         for (var j = 1; j < 6; j++) {
    //             grid += _monsterGridDictionary[i][j] == null ? "-" : 
    //                 _monsterGridDictionary[i][j].monsterResourceType == MonsterResourceType.RockPile ? "0" : "X";
    //             grid += " ";
    //         }
    //
    //         grid += "\n";
    //     }
    //
    //     grid += "\n Waves left: " + _wavesLeft + "";
    //     grid += "\n next wave with monster: " + _nextRowWithMonster + "";
    //     Debug.Log(grid);
    // }
}
}