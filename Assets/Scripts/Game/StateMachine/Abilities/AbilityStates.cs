using Framework.Base;

namespace Game.StateMachine.Abilities {

public abstract class States {
    public static readonly Created Created = new();
}

public class Created : State<AbilityFSM> {
    public override void Enter(AbilityFSM fsm) { }
}

}