using System;
using System.Diagnostics.CodeAnalysis;
using Core.Controller.Audio;
using Framework.Base;
using UnityEngine;

namespace Core.Data {
[Serializable]
public class SettingsDataV1 : Data<SettingsDataV1> {
    public BMLanguage GetLanguage() {
        return language;
    }

    public float GetVolume(AudioGroupType groupType) {
        return groupType switch {
            AudioGroupType.MUSIC => musicVolume,
            AudioGroupType.FX => fxVolume,
            _ => 0f
        };
    }

    public void SetVolume(AudioGroupType groupType, float volume) {
        switch (groupType) {
            case AudioGroupType.MUSIC:
                musicVolume = volume;
                break;
            case AudioGroupType.FX:
                fxVolume = volume;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(groupType), groupType, null);
        }

        Save();
    }

    public void ChangeLanguage(BMLanguage lang) {
        language = lang;
        Save();
    }

    #region Properties

    // TODO Get Language from system
    [SerializeField] private BMLanguage language = BMLanguage.English;
    [SerializeField] private float musicVolume = 0.2f; // TODO change for having audio on 1f
    [SerializeField] private float fxVolume = 1f;

    #endregion
}

[Serializable]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum BMLanguage {
    Portuguese, // = 28,
    English // = 10
}
}