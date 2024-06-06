using Core.Controller.Audio;
using Core.Data;
using Framework.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Core.StateMachine.AudioVolumeSlider {
public class AudioVolumeSliderFSM : StateMachine<AudioVolumeSliderFSM, State<AudioVolumeSliderFSM>> {
    [SerializeField] public Slider slider;
    [SerializeField] public AudioGroupType audioGroupType = AudioGroupType.MUSIC;
    protected override AudioVolumeSliderFSM FSM => this;
    protected override State<AudioVolumeSliderFSM> GetInitialState => States.Preload;

    protected override void Before() {
        var currentVolume = SettingsDataV1.Instance.GetVolume(audioGroupType);
        slider.value = currentVolume;
        slider.onValueChanged.AddListener(ChangeVolume);
    }

    private void ChangeVolume(float volume) {
        AudioController.ChangeVolume(audioGroupType, Mathf.Lerp(-80, 0, volume));
        SettingsDataV1.Instance.SetVolume(audioGroupType, volume);
    }
}
}