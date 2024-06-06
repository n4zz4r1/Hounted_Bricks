using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.Utils {
public static class LocalizationUtils {
    private static StringTable _localizedStringTable;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize() {
        PreloadLocalizationTable();
    }

    private static void PreloadLocalizationTable() {
        var tableLoadingOperation =
            LocalizationSettings.StringDatabase.GetTableAsync("BouncyLocalizer"); // Replace with your table name
        tableLoadingOperation.Completed += HandleTableLoaded;
    }

    private static void HandleTableLoaded(AsyncOperationHandle<StringTable> handle) {
        if (handle.Status == AsyncOperationStatus.Succeeded) {
            _localizedStringTable = handle.Result;
            Debug.Log("Localization table preloaded successfully.");
        }
        else {
            Debug.LogError("Failed to preload localization table.");
        }
    }

    public static string LoadText(string key) {
        if (_localizedStringTable == null)
            // Debug.LogError("Localization table not loaded.");
            return string.Empty;

        var entry = _localizedStringTable.GetEntry(key);
        if (entry != null) return entry.GetLocalizedString();

        Debug.LogWarning($"Key '{key}' not found in localization table.");
        return string.Empty;
    }

    public static async Task<string> LoadTextAsync(string key) {
        var localization = LocalizationSettings.StringDatabase.GetLocalizedStringAsync("BouncyLocalizer", key);
        return await localization.Task;
    }

    public static string From(Enum enumerator) {
        return LoadText(enumerator.GetType().Name + "." + enumerator);
    }
}
}