using System;
using System.Collections.Generic;
using Framework.Base;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Core.Controller.Audio {

public class AudioController : PersistentStateMachine<AudioController, MusicState> {
    [SerializeField] public List<AudioSource> musicSource;
    [SerializeField] public AudioSource fxSource;
    [SerializeField] public AudioMixer audioMixer;
    [SerializeField] public AudioListener audioListener;
    [SerializeField] public AudioDefaultClips audioDefaultClips;

    internal Music CurrentMusic;
    protected override AudioController FSM => this;
    protected override MusicState GetInitialState => States.Started;

    public override void BeforeChangeScene() {
        State.ChangeMusic(FSM, SceneManager.GetActiveScene().name);
    }

    public static void PlayFX(AudioClip clip) {
        var instance = (AudioController)Instance;
        instance.fxSource.PlayOneShot(clip);
    }

    public static void PlayFXRandom(List<AudioClip> clips) {
        if (clips == null || clips.Count == 0)
            return;

        var instance = (AudioController)Instance;
        instance.fxSource.PlayOneShot(clips[Random.Range(0, clips.Count)]);
    }

    public void ChangeVolumeInternal(AudioGroupType groupType, float volume) {
        audioMixer.SetFloat(groupType.ToString(), volume);
    }

    public static void ChangeVolume(AudioGroupType groupType, float volume) {
        var instance = (AudioController)Instance;
        instance.audioMixer.SetFloat(groupType.ToString(), volume);
    }

    public static void PlayFX(CommonFX commonFX) {
        var instance = (AudioController)Instance;
        switch (commonFX) {
            case CommonFX.CLICK_BUTTON_FX:
                PlayFXRandom(instance.audioDefaultClips.clickButtonFX);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(commonFX), commonFX, null);
        }
    }
}

public enum CommonFX {
    CLICK_BUTTON_FX
}

[Serializable]
public class AudioDefaultClips {
    [SerializeField] public List<AudioClip> clickButtonFX;
}

}