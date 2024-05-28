using Framework.Base;

namespace Core.StateMachine.AudioVolumeSlider {

public abstract class States {
    public static readonly Preload Preload = new();
}

public class Preload : State<AudioVolumeSliderFSM> { }

}