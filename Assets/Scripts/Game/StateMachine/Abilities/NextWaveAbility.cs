using Core.StateMachine.Abilities;
using Core.Utils.Constants;
using Game.Controller.Game;

namespace Game.StateMachine.Abilities {
public class NextWaveAbility : Ability<GameController> {
    protected override void InitAction() {
        GameController.GetGameResource(GameResource.Drop).Increase(1);
        GameController.State.NextWave(GameController);
    }
}
}