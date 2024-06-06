using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Data;
using Core.Popup;
using Core.Sprites;
using Core.StateMachine.Abilities;
using Core.StateMachine.Cards;
using Core.Utils.Constants;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Core.Utils {
public abstract class AssetLoader {
    private static readonly Dictionary<Enum, Object> CachedObjects = new();

    private static bool StartsWithNumber(string str) {
        return !string.IsNullOrEmpty(str) && char.IsDigit(str[0]);
    }

    public static Sprite AsSprite(Enum enumerator) {
        var texture = (Texture2D)CachedObjects[enumerator];
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    public static GameObject AsGameObject(Enum enumerator) {
        return (GameObject)CachedObjects[enumerator];
    }

    public static T AsComponent<T>(Enum enumerator) {
        return ((GameObject)CachedObjects[enumerator]).GetComponent<T>();
    }

    // TODO Improve this
    private static Enum GetEnumByName(string assetName) {
        if (StartsWithNumber(assetName))
            return Enum.Parse<Card>($"Card_{assetName}");

        if (assetName.Contains("CardAttributeType_"))
            return Enum.Parse<CardAttributeType>($"{assetName.Split("_")[1]}");

        if (assetName.Contains("Popups_"))
            return Enum.Parse<Popups>($"{assetName.Split("_")[1]}");

        if (assetName.Contains("GameResourceType_"))
            return Enum.Parse<GameResourceType>($"{assetName.Split("_")[1]}");

        if (assetName.Contains("GameResource_"))
            return Enum.Parse<GameResource>($"{assetName.Split("_")[1]}");

        if (assetName.Contains("ResourceType_"))
            return Enum.Parse<ResourceType>($"{assetName.Split("_")[1]}");

        if (assetName.Contains("AbilityPanel_"))
            return Enum.Parse<AbilityPanel>($"{assetName.Split("_")[1]}");

        if (assetName.Contains("Player_"))
            return Enum.Parse<Player>($"{assetName.Split("_")[1]}");

        if (assetName.Contains("RockPile_"))
            return Enum.Parse<RockPile>($"{assetName.Split("_")[1]}");

        if (assetName.Contains("CardType_"))
            return Enum.Parse<CardType>($"{assetName.Split("_")[1]}");

        if (assetName.Contains("Rock_"))
            return Enum.Parse<Rock>($"{assetName.Split("_")[1]}");

        throw new Exception($"Enum not found for: {assetName}");
    }

    public static void LoadAssetsByLabel<TX>(string label, Action<TX> callback, TX param) {
        Addressables.LoadAssetsAsync<Object>(label, null).Completed += handle => {
            var debug = "\n";
            int onCache = 0, loads = 0;
            if (handle.Status == AsyncOperationStatus.Succeeded) {
                foreach (var asset in handle.Result) {
                    // if (StartsWithNumber(asset.name)) {
                    var assetname = GetEnumByName(asset.name);


                    if (CachedObjects.ContainsKey(assetname)) {
                        debug += $"Asset {asset.name} already loaded.\n";
                        onCache++;
                    }
                    else {
                        debug += $"Asset {asset.name} loaded successfully.\n";
                        loads++;
                    }

                    CachedObjects[assetname] = asset;
                }

                Debug.Log($"[AssetLoader] {loads} New assets preloaded, as {onCache} fetched from cache {debug} ");

                callback?.Invoke(param);
            }
            else {
                Debug.LogError("Failed to load assets.");
            }
        };
    }
    
    public static void LoadAssetsByLabel(string label) {
        Addressables.LoadAssetsAsync<Object>(label, null).Completed += handle => {
            var debug = "\n";
            int onCache = 0, loads = 0;
            if (handle.Status == AsyncOperationStatus.Succeeded) {
                foreach (var asset in handle.Result) {
                    // if (StartsWithNumber(asset.name)) {
                    var assetname = GetEnumByName(asset.name);


                    if (CachedObjects.ContainsKey(assetname)) {
                        debug += $"Asset {asset.name} already loaded.\n";
                        onCache++;
                    }
                    else {
                        debug += $"Asset {asset.name} loaded successfully.\n";
                        loads++;
                    }

                    CachedObjects[assetname] = asset;
                }

                Debug.Log($"[AssetLoader] {loads} New assets preloaded, as {onCache} fetched from cache {debug} ");

            }
            else {
                Debug.LogError("Failed to load assets.");
            }
        };
    }
}

public abstract class AssetLoader<TU> where TU : Enum {
    // private static T ConvertToObject<T>(Object o) where T : Object =>
    //     o is GameObject gameObject ? gameObject.GetComponent<T>() : (T)o;
    private static string AssetName(TU assetEnum) {
        return typeof(TU).Name == "Card"
            ? assetEnum.ToString().Replace("Card_", "")
            : typeof(TU).Name + "_" + assetEnum;
    }


    private static T ConvertToObject<T>(Object o) where T : Object {
        // Debug.Log($"component found from {typeof(T).Name}, object type {o.GetType()} : {o}");
        if (o is GameObject gameObject) return gameObject.GetComponent<T>();
        return (T)o;
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
    public static void Load<TV, TX>(TU assetEnum, TV fsm, Action<TX, TV> callback) where TX : Object {
        var handle = Addressables.LoadAssetAsync<Object>(AssetName(assetEnum));
        handle.Completed += operation => {
            if (operation.Status == AsyncOperationStatus.Succeeded) {
                var asset = operation.Result;
                callback?.Invoke(ConvertToObject<TX>(asset), fsm);
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
}