using Core.Sprites;
using Core.StateMachine.Abilities;
using Core.Utils.Constants;
using Game.Controller.Game;
using Game.Utils;
using UnityEngine;

namespace Game.StateMachine.Abilities {
public abstract class BuffAbility {
    public abstract Buff Buff();
    public abstract BuffType BuffType();
    public abstract CardAttributeType CardAttributeType();

    // public virtual IEnumerator ApplyBuff<T>(GameController gameController, int quantity, T fsm)
    //     where T : StateMachine<T, State<T>> {
    //     yield return null;
    // }
}

public abstract class BuffAbilityBase<T> : Ability<GameController, GameState> where T : BuffAbility, new() {
    protected override void InitAction() {
        T ability = new T();
        // 1. Add buff to List (Balancer)
        Balancer.Instance.AddBuff(ability, CardFSM);
        // 2. Add buff frame
        GameController.AddGameBuff(ability.Buff(), CardFSM.components.originalIcon);
        AbilityDoneCallback?.Invoke();
    }
}

}