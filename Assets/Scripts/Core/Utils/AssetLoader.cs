using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Core.Utils {

public abstract class AssetLoader<U> where U : Enum {

    private static string AssetName(U assetEnum) => typeof(U).Name == "Card" ? 
        assetEnum.ToString().Replace("Card_", "") : typeof(U).Name + "_" + assetEnum;

    /**
     * Load an asset (GameObject) storing on cache and returning it
     */
    public static async Task<T> Load<T>(U assetEnum) where T : Object {
        // // 1. Tries to get from cache.
        // if (AssetCache.TryGetValue(assetEnum, out var cachedAsset)) {
        //     CustomLog($"Asset {assetEnum} fetched from preloaded cache.");
        //     return ConvertToObject<T>(cachedAsset);
        // }

        try {
            // 2. Check if asset exists at all
            var asset = await Addressables.LoadAssetAsync<Object>(AssetName(assetEnum)).Task;
            if (asset == null) {
                Debug.LogError($"Failed to load asset from enum: {AssetName(assetEnum)}");
                return null;
            }
            //
            // // 3. Stores on cache and return
            // AssetCache[assetEnum] = asset;
            // CustomLog($"Asset {assetEnum} loaded successfully.");

            return ConvertToObject<T>(asset);
        }
        catch (Exception ex) {
            Debug.LogError($"Failed to load asset: {ex.Message}");
            return null;
        }
    }

    private static T ConvertToObject<T>(Object o) where T : Object {
        Debug.Log($"component found from {typeof(T).Name}, object type {o.GetType()} : {o}");
        // if (o is Sprite sprite && typeof(T) == typeof(Object)) {
        //     return sprite as T;
        // }
        // If is a subclass of GameObject, should return component T, else cast normally
        if (o is GameObject gameObject) {
            return gameObject.GetComponent<T>();
        }
        return (T) o;
    }

    //

    /**
     * Load an asset (GameObject) storing on cache and returning it as callback
     */
    public static void Load<V, X>(U assetEnum, V fsm, Action<X, V> callback) where X : Object {

        // 1. Tries to get from cache.
        // if (AssetCache.TryGetValue(assetEnum, out var cachedAsset)) {
        //     CustomLog($"Asset {assetEnum} fetched from preloaded cache.");
        //     callback?.Invoke(ConvertToObject<X>(cachedAsset), fsm);
        //     return;
        // }

        // 2. Once request completed, store asset on cache and execute callback 
        var handle = Addressables.LoadAssetAsync<X>(AssetName(assetEnum));
        handle.Completed += operation => {
            if (operation.Status == AsyncOperationStatus.Succeeded) {
                var asset = operation.Result;
                // AssetCache[assetEnum] = asset;
                // CustomLog($"Asset {assetEnum} loaded successfully.");
                callback?.Invoke(ConvertToObject<X>(asset), fsm);
            }
            else {
                Debug.LogError($"Failed to load asset: {operation.OperationException?.Message}");
            } 
        };
    }

    private static void CustomLog(string message) {
        Debug.Log(message); // You can customize this method to log in different ways.
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