using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Framework.Base {
public class CustomTransition : MonoBehaviour {
    private static readonly int FadeOutAnim = Animator.StringToHash("FadeOut");
    private static readonly int FadeInAnim = Animator.StringToHash("FadeIn");
    private bool _hasLoading;

    private string _nextScene;
    private bool _quit;

    public void TransitionTo(string scene, bool loading = false) {
        // Debug.Log("transition to scene " + scene + " now");

        _nextScene = scene;
        _hasLoading = loading;
        animator.SetTrigger(FadeOutAnim);
    }
    
    public void FadeIn() => animator.SetTrigger(FadeInAnim);

    public void FadeOutFinished() {
        if (_quit)
            Application.Quit();
        else if (!_hasLoading)
            SceneManager.LoadScene(_nextScene);
        else
            _ = StartCoroutine(LoadAsync(_nextScene));
    }

    public void Quit() {
        _quit = true;
        animator.SetTrigger(FadeOutAnim);
    }

    private IEnumerator LoadAsync(string scene) {
        var operation = SceneManager.LoadSceneAsync(scene);
        loaderBar.gameObject.SetActive(true);
        // while (!operation.isDone) {
        while (operation is { isDone: false }) {
            var progress = Mathf.Clamp01(operation.progress / .9f);
            loaderBar.value = progress;
            yield return null;
        }
    }

    #region Components

    [SerializeField] public Slider loaderBar;
    [SerializeField] public Animator animator;

    #endregion
}
}