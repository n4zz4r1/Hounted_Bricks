using Framework.Base;

namespace Game.StateMachine.AbilityPanels {
public abstract class States {
    public static readonly Preload Preload = new();
}

public class Preload : State<AbilityPanelFSM> {
    public override void Enter(AbilityPanelFSM fsm) { }
}
}