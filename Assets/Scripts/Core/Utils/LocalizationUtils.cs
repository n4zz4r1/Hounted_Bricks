using System.Threading.Tasks;
using UnityEngine.Localization.Settings;

namespace Core.Utils {

public static class LocalizationUtils {
    // Method to get localized text synchronously (make sure the system is initialized before you call this)
    // ReSharper disable once StringLiteralTypo
    // public static string GetLocalizedText(string key, string table = "BouncyLocalizer") {
    //     // Check if the localization system is ready; if not, return key as fallback
    //     // if (LocalizationSettings.InitializationOperation.WaitForCompletion())
    //     return LocalizationSettings.StringDatabase.GetLocalizedString(table, key);
    //
    //     Debug.LogWarning("Localized Key `" + key + "` not found.");
    //     return key; // Return the key as a fallback
    // }
    //
    // // Asynchronous version to get localized text
    // public static IEnumerator GetLocalizedTextAsync(string table, string key, Action<string> callback) {
    //     // Wait until the localization system is ready
    //     yield return LocalizationSettings.InitializationOperation;
    //
    //     // Retrieve the localized string
    //     var result = LocalizationSettings.StringDatabase.GetLocalizedString(table, key);
    //     callback?.Invoke(result);
    // }
    //
    // // Asynchronous version to get localized text
    // public static IEnumerator WaitLocalizedToLoad() {
    //     // Wait until the localization system is ready
    //     yield return LocalizationSettings.InitializationOperation;
    // }
    
    public static async Task<string> LoadText(string key) {
        var localization = LocalizationSettings.StringDatabase.GetLocalizedStringAsync("BouncyLocalizer", key);
        return await localization.Task;
    }
}

}