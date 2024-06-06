using Core.StateMachine.Abilities;
using Game.Controller.Game;

namespace Game.StateMachine.Abilities {
public class RecycleAbility : Ability<GameController> {
    protected override void InitAction() {
        GameController.State.Collect(GameController);
    }
}
}