using Framework.Base;

namespace Core.StateMachine.AudioVolumeSlider {

public abstract class States {
    public static readonly Created Created = new();
}

public class Created : State<AudioVolumeSliderFSM> { }

}