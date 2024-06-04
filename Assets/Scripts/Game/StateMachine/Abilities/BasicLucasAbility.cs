using Core.StateMachine.Abilities;
using Game.Controller.Game;

namespace Game.StateMachine.Abilities {

public class BasicLucasAbility : Ability<GameController> {

    // Increase aim factor * 2
    protected override void InitAction() {
        GameController.PlayerInGame.AimFactor*=2;
        AbilityDoneCallback?.Invoke();
    }
    
}

}