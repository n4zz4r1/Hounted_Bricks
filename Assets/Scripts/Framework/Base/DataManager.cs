using System;
using System.IO;
using UnityEngine;

namespace Framework.Base {

public static class DataManager {
    private static readonly object LockObject = new();
    private static readonly string SaveFolderPath = Path.Combine(Application.persistentDataPath, "saves");

    public static bool UnitTest = false;

    public static T Load<T>() where T : new() {
        lock (LockObject) {
            try {
                var path = Path.Combine(SaveFolderPath, typeof(T).Name + "-data.json");
                if (!File.Exists(path)) {
                    Save(new T());
                    return new T();
                }

                var json = File.ReadAllText(path);
                return JsonUtility.FromJson<T>(json);
            }
            catch (Exception ex) {
                Debug.LogError($"Failed to load data: {ex.Message}");
                return default;
            }
        }
    }

    public static void Save<T>(T instance) {
        lock (LockObject) {
            if (UnitTest) return;

            try {
                if (!Directory.Exists(SaveFolderPath)) Directory.CreateDirectory(SaveFolderPath);

                var path = Path.Combine(SaveFolderPath, instance.GetType().Name + "-data.json");
                var json = JsonUtility.ToJson(instance, true);
                File.WriteAllText(path, json);
            }
            catch (Exception ex) {
                Debug.LogError($"Failed to save data: {ex.Message}");
            }
        }
    }

    public static void Clean() {
        lock (LockObject) {
            try {
                if (Directory.Exists(SaveFolderPath)) Directory.Delete(SaveFolderPath, true);
            }
            catch (Exception ex) {
                Debug.LogError($"Failed to clean data: {ex.Message}");
            }
        }
    }

    public static void ExecuteUnderLock(Action action) {
        lock (LockObject) {
            action();
        }
    }
}

[Serializable]
public abstract class Data<T> where T : Data<T>, new() {
    private static readonly Lazy<T> LazyInstance = new(DataManager.Load<T>);

    public static T Instance => LazyInstance.Value;

    protected static void Save() {
        DataManager.Save(LazyInstance.Value);
    }

    protected static void Transaction(Action action) {
        DataManager.ExecuteUnderLock(() => {
            action();
            Save();
        });
    }
}

}