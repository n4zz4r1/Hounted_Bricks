using Core.Sprites;
using Core.StateMachine.Abilities;
using Game.Controller.Game;

namespace Game.StateMachine.Abilities {
public class NextWaveAbility : Ability<GameController, GameState> {
    protected override void InitAction() {
        GameController.GetGameResource(ResourceType.Elixir).Increase();
        GameController.State.NextWave(GameController);
    }
}
}