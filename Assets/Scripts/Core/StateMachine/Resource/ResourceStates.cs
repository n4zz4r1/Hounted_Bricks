using Framework.Base;

namespace Core.StateMachine.Resource {

/**
 * Refactor: TODO
 */
public abstract class States {
    public static readonly Created Created = new();
}

public class Created : State<ResourceFSM> { }

}