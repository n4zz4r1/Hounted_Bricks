using System;
using System.Threading.Tasks;
using Core.StateMachine.Cards;
using Core.Utils.Constants;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Core.Utils {

public abstract class AssetLoader<T, U> where U : Enum where T : Object {
    // Get T Object by Enum
    public static async Task<T> Load(U assetEnum) {
        var handle = Addressables.LoadAssetAsync<T>(typeof(U).Name + "_" + assetEnum);

        try {
            var asset = await handle.Task;
            if (asset != null)
                // Debug.Log("Asset loaded successfully.");
                // Here you can use the asset, for example, instantiate it
                return asset;

            Debug.LogError($"Failed to load asset: {asset}");
            return null;
        }
        catch (Exception ex) {
            Debug.LogError($"Failed to load asset: {ex.Message}");
            return null;
        }
    }

    public static async Task<GameObject> LoadCard(Card card) {
        var handle = Addressables.LoadAssetAsync<GameObject>(card.ToString().Replace("Card_", ""));

        try {
            var asset = await handle.Task;
            if (asset != null)
                // Debug.Log("Asset loaded successfully.");
                // Here you can use the asset, for example, instantiate it
                return asset;

            Debug.LogError($"Failed to load asset: {asset}");
            return null;
        }
        catch (Exception ex) {
            Debug.LogError($"Failed to load asset: {ex.Message}");
            return null;
        }
    }

    public static async Task<CardFSM> LoadCardFSM(Card card) {
        var handle = Addressables.LoadAssetAsync<GameObject>(card.ToString().Replace("Card_", ""));

        try {
            var asset = await handle.Task;
            if (asset != null)
                // Debug.Log("Asset loaded successfully.");
                // Here you can use the asset, for example, instantiate it
                return asset.GetComponent<CardFSM>();

            Debug.LogError($"Failed to load asset: {asset}");
            return null;
        }
        catch (Exception ex) {
            Debug.LogError($"Failed to load asset: {ex.Message}");
            return null;
        }
    }

    public static void LoadCard(CardFSM fsm, Action<GameObject, CardFSM> callback) {
        var handle = Addressables.LoadAssetAsync<GameObject>(fsm.cardId.ToString().Replace("Card_", ""));
        handle.Completed += operation => {
            if (operation.Status == AsyncOperationStatus.Succeeded) {
                var asset = operation.Result;
                // Debug.Log("Asset loaded successfully.");
                callback?.Invoke(asset, fsm);
            }
            else {
                Debug.LogError($"Failed to load asset: {operation.OperationException?.Message}");
                callback?.Invoke(null, fsm);
            }
        };
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