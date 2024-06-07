using System.Collections.Generic;
using System.Linq;
using Core.Data;
using Core.Sprites;
using Core.StateMachine.Cards;
using Core.Utils;
using Core.Utils.Constants;
using Game.StateMachine.Abilities;
using UnityEngine;

namespace Game.Utils {
public class Balancer : SingletonBase<Balancer> {
    private Dictionary<Buff, CardFSM> _buffCard = new();
    private Dictionary<Buff, int> _buffQuantity = new();
    private Dictionary<Buff, BuffAbility> _buffs = new();

    private int Level { get; set; }

    private float LevelScale() {
        return 1f + Level * 0.01f;
    }

    public void RefreshLevelAndBuffs() {
        Level = (int) GameDataV1.Instance.level;
        _buffs = new Dictionary<Buff, BuffAbility>();
        _buffQuantity = new Dictionary<Buff, int>();
        _buffCard = new Dictionary<Buff, CardFSM>();
    }

    public bool HasBuff(Buff buff) {
        return _buffs.ContainsKey(buff);
    }

    public bool DiceBuff(Buff buff) {
        return _buffs.TryGetValue(buff, out var buff1) && Dices.Roll(GetBuff(buff));
    }

    public void AddBuff(BuffAbility buffAbility, CardFSM cardFSM) {
        var buff = buffAbility.Buff();
        // var buffType = buffAbility.BuffType();

        // Add buff to list
        if (!_buffs.TryAdd(buff, buffAbility)) {
            _buffQuantity[buff]++;
        }
        else {
            _buffQuantity.Add(buff, 1);
            _buffCard.Add(buff, cardFSM);
        }

        PrintBuffs();
    }

    private void PrintBuffs() {
        var buffs = _buffs.Aggregate("",
            (current, keyValuePair) =>
                current +
                $"\n Buff type {keyValuePair.Key} has {_buffQuantity[keyValuePair.Key]}x with value {GetBuff(keyValuePair.Key)}");
        Debug.Log(buffs);
    }

    // When attribute is probability, multiply, else concat.  
    public float GetBuff(Buff buff) {
        if (_buffs[buff].CardAttributeType() is CardAttributeType.Probability)
            return GameMathUtils.GetPercentageByQuantity(_buffCard[buff].Attribute(_buffs[buff].CardAttributeType()),
                _buffQuantity[buff]);
        return _buffCard[buff].Attribute(_buffs[buff].CardAttributeType()) * _buffQuantity[buff];
    }
    public string GetBuffText(Buff buff) {
        if (_buffs[buff].CardAttributeType() is CardAttributeType.Probability) {
            var result = GameMathUtils.GetPercentageByQuantity(_buffCard[buff].Attribute(_buffs[buff].CardAttributeType()),
                _buffQuantity[buff]);
            return $"{(int) result}%";
        }
        return $"+ {_buffCard[buff].Attribute(_buffs[buff].CardAttributeType()) * _buffQuantity[buff]}";
    }
    // 

    #region Stones

    public float StoneDamage(float baseWithLevelDamage, float playerBaseWithLevelDamage) {
        return baseWithLevelDamage + PlayerDamage(playerBaseWithLevelDamage);
    }

    #endregion

    #region Player

    private float PlayerDamage(float playerBaseWithLevelDamage) {
        if (_buffs.ContainsKey(Buff.PlayerDamage))
            return playerBaseWithLevelDamage * (1 + (GetBuff(Buff.PlayerDamage) / 100));
        
        return playerBaseWithLevelDamage;
    }

    #endregion


    // To balance force and health, all attributes will grow 1% per level by the stage number
    // Stage 1 = 100% damage and force
    // Stage 100 = 200% damage and force
    // Stage 400 = 400% damage and force
    // And so on...

    #region Monster

    public int MonsterDamage(int baseDamage) {
        return baseDamage;
    }

    public float MonsterLife(float baseLife) {
        return baseLife * LevelScale();
    }

    #endregion
}
}