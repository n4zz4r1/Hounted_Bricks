using Framework.Base;

namespace Core.StateMachine.ResourceSlider {

public abstract class States {
    public static readonly Preload Preload = new();
}

public class Preload : State<ResourceSliderFSM> {
    public override void Enter(ResourceSliderFSM fsm) { }
}

}