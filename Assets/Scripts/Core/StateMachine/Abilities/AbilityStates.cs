using Framework.Base;

namespace Core.StateMachine.Abilities {

public abstract class States {
    public static readonly Preload Preload = new();
    public static readonly InProgress InProgress = new();
    public static readonly Executed Executed = new();
    public static readonly Available Available = new();
    public static readonly Unavailable Unavailable = new();
}

public class Preload : State<AbilityFSM> {
    
}
public class InProgress : State<AbilityFSM> { }
public class Executed : State<AbilityFSM> { }
public class Available : State<AbilityFSM> { }
public class Unavailable : State<AbilityFSM> { }

}