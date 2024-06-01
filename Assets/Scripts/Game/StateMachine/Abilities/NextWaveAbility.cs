using Core.Handler;
using Core.StateMachine.Abilities;
using Framework.Base;
using Game.Controller.Game;
using UnityEngine;

namespace Game.StateMachine.Abilities {

public class NextWaveAbility : Ability<GameController> {

    protected override void InitAction() =>
        GameController.State.NextWave(GameController);
    
}

}