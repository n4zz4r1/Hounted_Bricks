using System.Collections.Generic;
using System.Linq;
using Framework.Base;
using Game.StateMachine.Monster;
using UnityEngine;

namespace Game.Utils {

public class MonsterGrid {
    private MonsterFSM[][] _monsterGrid;

    private readonly Dictionary<int, Dictionary<int, MonsterFSM>> _monsterGridDictionary = new();

    private readonly int _nextRowWithMonster;

    // private int _wavesTotal;
    private readonly int _wavesLeft;

    public MonsterGrid(AtomicList<MonsterFSM> monsterFSMList) {
        if (monsterFSMList.Count == 0)
            return;

        // 1. Find waves left number
        _wavesLeft = (int)monsterFSMList.ToList().Max(fsm => fsm.transform.position.y) + 1;
        _nextRowWithMonster = (int)monsterFSMList.ToList().Min(fsm => fsm.transform.position.y) + 1;

        // 2. Set null for all grid
        for (var i = 0; i < _wavesLeft; i++) {
            Dictionary<int, MonsterFSM> xAxis = new();
            for (var j = 0; j < 6; j++) xAxis.Add(j, null);

            _monsterGridDictionary.Add(i, xAxis);
        }

        MonsterFSMList = monsterFSMList.ToList();
        // 3. insert monster on current grid
        foreach (var monsterFSM in MonsterFSMList) {
            var position = monsterFSM.transform.position;
            _monsterGridDictionary[(int)position.y][(int)position.x] = monsterFSM;
        }

        // 
    }

    public List<MonsterFSM> MonsterFSMList { get; set; }

    public bool HasMonsterInFront(MonsterFSM monsterFSM) {
        var position = monsterFSM.transform.position;
        return _monsterGridDictionary[(int)position.y - 1][(int)position.x] != null;
    }

    public bool HasMonstersOnScene() {
        return _nextRowWithMonster <= 10;
    }

    public bool HasMonstersOnRow(int row) {
        for (var i = 0; i < 6; i++)
            if (_monsterGridDictionary[row][i] != null)
                return true;

        return false;
    }

    public void PrintGrid() {
        var grid = "[GRID LOG] \n";

        for (var i = 1; i < _wavesLeft; i++) {
            grid += "Line (y) : " + i + " - ";
            for (var j = 0; j < 6; j++) {
                grid += _monsterGridDictionary[i][j] == null ? "-" : "X";
                grid += " ";
            }

            grid += "\n";
        }

        grid += "\n Waves left: " + _wavesLeft + "";
        grid += "\n next wave with monster: " + _nextRowWithMonster + "";
        Debug.Log(grid);
    }
}

}