using System.Collections;
using Core.Data;
using Framework.Base;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Controller.Audio {
public abstract class MusicState : State<AudioController> {
    public virtual void ChangeMusic(AudioController fsm, string scene, bool forceChange = false) { }
}

public abstract class States {
    public static readonly Started Started = new();
}

public class Started : MusicState {
    public override void Enter(AudioController fsm) {
        fsm.CurrentMusic = SceneManager.GetActiveScene().name == "GameScene"
            ? Music.GAME_MUSIC
            : Music.HOME_MUSIC;

        fsm.ChangeVolumeInternal(AudioGroupType.MUSIC,
            Mathf.Lerp(-80, 0, SettingsDataV1.Instance.GetVolume(AudioGroupType.MUSIC)));
        fsm.ChangeVolumeInternal(AudioGroupType.FX,
            Mathf.Lerp(-80, 0, SettingsDataV1.Instance.GetVolume(AudioGroupType.FX)));

        fsm.musicSource[(int)fsm.CurrentMusic].Play();
    }


    public override void ChangeMusic(AudioController fsm, string scene, bool forceChange = false) {
        var nextMusic = scene == "GameScene" ? Music.GAME_MUSIC : Music.HOME_MUSIC;

        // Only proceed if `force` true or next music has changed
        if (!forceChange && nextMusic == fsm.CurrentMusic) return;

        fsm.musicSource[(int)nextMusic].Play();
        fsm.StartCoroutine(PlayWithFadeOut(fsm, fsm.CurrentMusic, nextMusic));
        fsm.CurrentMusic = nextMusic;
    }

    private static IEnumerator PlayWithFadeOut(AudioController fsm, Music currentMusic, Music nextMusic) {
        const float fadeDuration = 1f; // Duration of the cross-fade
        var elapsed = 0f;
        var maxMusicVolume = SettingsDataV1.Instance.GetVolume(AudioGroupType.MUSIC);

        while (elapsed < fadeDuration) {
            elapsed += Time.deltaTime;

            // Linear interpolation for cross-fade
            var t = elapsed / fadeDuration;
            fsm.musicSource[(int)currentMusic].volume = Mathf.Lerp(maxMusicVolume, 0, t);
            fsm.musicSource[(int)nextMusic].volume = Mathf.Lerp(0, maxMusicVolume, t);

            yield return null;
        }

        fsm.musicSource[(int)currentMusic].Stop(); // Stop the faded-out source
    }
}


public enum Music {
    GAME_MUSIC = 0,
    HOME_MUSIC = 1
}

public enum AudioGroupType {
    MUSIC,
    FX
}
}