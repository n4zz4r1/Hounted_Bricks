using System.Collections;
using Framework.Base;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Core.Controller.Preload {

/**
 * Refactor: ok
 */
public abstract class States {
    public static readonly Created Created = new();
}

public class Created : State<PreloadController> {
    public override void Enter(PreloadController fsm) {
        fsm.StartCoroutine(LoadAllAssets(fsm));
    }

    private static IEnumerator LoadAllAssets(PreloadController fsm) {
        float totalProgress = 0;

        // Step 1: Load all Addressable assets with a specific label
        var handle = Addressables.LoadAssetsAsync<GameObject>("MainSceneAddressable", null);
        yield return handle;
        UpdateProgressBar(fsm, handle.PercentComplete, ref totalProgress);

        // Step 2: Preload the main scene but do not activate it yet
        var sceneHandle = Addressables.LoadSceneAsync("MainScene", LoadSceneMode.Single, false);
        while (!sceneHandle.IsDone) {
            UpdateProgressBar(fsm, sceneHandle.PercentComplete, ref totalProgress);
            yield return null;
        }

        // Step 3: Load localization data (assuming you have a method to do this)
        // Removed for now, TODO check if its needed

        // Once all done, activate the scene
        yield return sceneHandle.Result.ActivateAsync();

        fsm.TransitionWithEffectTo("MainScene");
    }

    private static void UpdateProgressBar(PreloadController fsm, float progress, ref float totalProgress) {
        totalProgress += progress;
        fsm.progressBar.value = totalProgress / 2; // Dividing by the number of steps (3 in this case)
    }
}

}