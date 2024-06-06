using Framework.Base;

namespace Core.StateMachine.Abilities {
public abstract class States {
    public static readonly Ready Ready = new();
    public static readonly InProgress InProgress = new();
    public static readonly Executed Executed = new();
}

public class Ready : State<AbilityFSM> { }

public class Executed : State<AbilityFSM> { }

public class InProgress : State<AbilityFSM> { }
}