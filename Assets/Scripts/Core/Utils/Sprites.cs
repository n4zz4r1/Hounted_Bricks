using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization.Settings;
using Object = UnityEngine.Object;

namespace Core.Utils {

public abstract class Sprites<T, U> where U : Enum where T : Object {
    // Get T Object by Enum
    public static async Task<T> LoadAssetAsync(U assetEnum) {
        var handle = Addressables.LoadAssetAsync<T>(typeof(U).Name + "_" + assetEnum);

        try {
            var asset = await handle.Task;
            if (asset != null) {
                Debug.Log("Asset loaded successfully.");
                // Here you can use the asset, for example, instantiate it
                return asset;
            }

            Debug.LogError($"Failed to load asset: {asset}");
            return null;
        }
        catch (Exception ex) {
            Debug.LogError($"Failed to load asset: {ex.Message}");
            return null;
        }
    }

    // Get Localization by Enum
    public static async Task<string> LoadLabel(U enumType) {
        var localization =
            LocalizationSettings.StringDatabase.GetLocalizedStringAsync("BouncyLocalizer",
                typeof(T).Name + "_" + enumType);
        return await localization.Task;
    }
}

// Below, all sprites available

[Serializable]
public enum ResourceType {
    NONE = 0,
    COIN = 1,
    DIAMOND = 2,
    ROCK_SCROLL = 3,
    CHAR_SCROLL = 4,
    ABILITY_SCROLL = 5,
    CHEST = 6,
    CHEST_KEYS = 7,
    CARD = 8,
    MONEY = 9
}

}