using Framework.Base;

namespace Core.StateMachine.Template {
public abstract class States {
    public static readonly Preload Preload = new();
}

public class Preload : State<TemplateFSM> {
    public override void Enter(TemplateFSM fsm) { }
}
}