using Framework.Base;

namespace Core.StateMachine.ResourceSlider {

public abstract class States {
    public static readonly Created Created = new();
}

public class Created : State<ResourceSliderFSM> {
    public override void Enter(ResourceSliderFSM fsm) { }
}

}