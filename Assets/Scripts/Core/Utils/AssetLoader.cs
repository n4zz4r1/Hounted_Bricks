using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Utils.Constants;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Core.Utils {

public abstract class AssetLoader<TU> where TU : Enum {
    private static bool StartsWithNumber(string str) =>
        !string.IsNullOrEmpty(str) && char.IsDigit(str[0]);

    public static void LoadAssetsByLabel(string label, Action onComplete = null) {
        Addressables.LoadAssetsAsync<Object>(label, null).Completed += handle => {
            if (handle.Status == AsyncOperationStatus.Succeeded) {
                foreach (var asset in handle.Result) {
                    var parts = asset.name.Split('_');

                    if (StartsWithNumber(asset.name)) {
                        Debug.Log(cachedPrefab.ContainsKey(Enum.Parse<Card>($"Card_{asset.name}"))
                            ? $"Asset {asset.name} already loaded."
                            : $"[AssetLoader] Asset {asset.name} loaded successfully.");

                        cachedPrefab[Enum.Parse<Card>($"Card_{asset.name}")] = asset;

                    } else if (parts.Length == 2) {
                        var enumType = Type.GetType(parts[0]);
                        if (enumType is { IsEnum: true }) {

                            // Here, cards are exceptions and starts with numbers
                            if (Enum.TryParse(enumType, parts[1], out var enumValue)) {
                                Debug.Log(cachedPrefab.ContainsKey((Enum)enumValue)
                                    ? $"Asset {asset.name} already loaded."
                                    : $"[AssetLoader] Asset {asset.name} loaded successfully.");
                                cachedPrefab[(Enum)enumValue] = asset;
                            } else {
                                Debug.LogWarning($"Failed to parse enum value '{parts[1]}' for enum type '{enumType}'.");
                            }
                        } else {
                            Debug.LogWarning($"Failed to find enum type '{parts[0]}'.");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Asset name {asset.name} does not match the expected format.");
                    }
                }

                onComplete?.Invoke();
            } else {
                Debug.LogError("Failed to load assets.");
            }
        };
    }

    
    public static Dictionary<Enum, Object> cachedPrefab = new();
    
    // private static T ConvertToObject<T>(Object o) where T : Object =>
    //     o is GameObject gameObject ? gameObject.GetComponent<T>() : (T)o;
    private static string AssetName(TU assetEnum) => typeof(TU).Name == "Card" ? 
        assetEnum.ToString().Replace("Card_", "") : typeof(TU).Name + "_" + assetEnum;

    
    private static T ConvertToObject<T>(Object o) where T : Object {
        // Debug.Log($"component found from {typeof(T).Name}, object type {o.GetType()} : {o}");
        if (o is GameObject gameObject) {
            return gameObject.GetComponent<T>();
        }
        return (T) o;
    }

    /**
     * Load an asset (GameObject instance) from addressable based on its Enum
     */
    public static async Task<T> Load<T>(TU assetEnum) where T : Object {

        try {
            var asset = await Addressables.LoadAssetAsync<Object>(AssetName(assetEnum)).Task;
            if (asset != null) return ConvertToObject<T>(asset);

            Debug.LogError($"Failed to load asset from enum: {AssetName(assetEnum)}");
            return null;

        }
        catch (Exception ex) {
            Debug.LogError($"Failed to load asset: {ex.Message}");
            return null;
        }
    }

    public static async Task<GameObject> LoadAsGameObject(TU assetEnum) {

        try {
            var asset = await Addressables.LoadAssetAsync<GameObject>(AssetName(assetEnum)).Task;
            if (asset != null) return asset;

            Debug.LogError($"Failed to load asset from enum: {AssetName(assetEnum)}");
            return null;

        }
        catch (Exception ex) {
            Debug.LogError($"Failed to load asset: {ex.Message}");
            return null;
        }
    }
    
    /**
     * Load an asset (GameObject instance) from addressable based on its Enum, with callback
     */
    public static void Load<V, X>(TU assetEnum, V fsm, Action<X, V> callback) where X : Object {

        var handle = Addressables.LoadAssetAsync<Object>(AssetName(assetEnum));
        handle.Completed += operation => {
            if (operation.Status == AsyncOperationStatus.Succeeded) {
                var asset = operation.Result;
                callback?.Invoke(ConvertToObject<X>(asset), fsm);
            }
            else {
                Debug.LogError($"Failed to load asset: {operation.OperationException?.Message}");
            } 
        };
    }
    
    /**
     * Load an asset (GameObject instance) from addressable based on its Enum, with callback
     */
    public static void LoadAsGameObject<TV>(TU assetEnum, TV fsm, Action<GameObject, TV> callback) {
        var handle = Addressables.LoadAssetAsync<GameObject>(AssetName(assetEnum));
        handle.Completed += operation => {
            if (operation.Status == AsyncOperationStatus.Succeeded) 
                callback?.Invoke(operation.Result, fsm);
            else 
                Debug.LogError($"Failed to load asset: {operation.OperationException?.Message}");
        };
    }    

    public static void LoadAsGameObject(TU assetEnum, Action<GameObject> callback) {
        var handle = Addressables.LoadAssetAsync<GameObject>(AssetName(assetEnum));
        handle.Completed += operation => {
            if (operation.Status == AsyncOperationStatus.Succeeded) 
                callback?.Invoke(operation.Result);
            else 
                Debug.LogError($"Failed to load asset: {operation.OperationException?.Message}");
        };
    }    
}

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
//
// [Serializable]
// public enum CardAttribute {
//     
// }


}