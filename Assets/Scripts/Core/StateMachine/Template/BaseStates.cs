using Framework.Base;

namespace Core.StateMachine.Template {

/**
 * Refactor: ok
 */
public abstract class States {
    public static readonly Created Created = new();
}

public class Created : State<TemplateFSM> {
    public override void Enter(TemplateFSM fsm) { }
}

}