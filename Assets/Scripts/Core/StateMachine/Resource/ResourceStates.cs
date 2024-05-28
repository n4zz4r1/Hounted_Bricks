using Framework.Base;

namespace Core.StateMachine.Resource {

public abstract class States {
    public static readonly Preload Preload = new();
}

public class Preload : State<ResourceFSM> { }

}