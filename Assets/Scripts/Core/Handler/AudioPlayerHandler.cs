using Core.Controller.Audio;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Core.Handler {
public class AudioPlayerHandler : MonoBehaviour, IPointerClickHandler {
    [SerializeField] public CommonFX commonFX;
    [SerializeField] public bool checkForDisabledButton = true;

    public void OnPointerClick(PointerEventData _) {
        if (checkForDisabledButton) {
            var button = gameObject.GetComponent<Button>();
            if (button != null && button.enabled)
                AudioController.PlayFX(commonFX);
        }
        else {
            AudioController.PlayFX(commonFX);
        }
        // Checks if there is a button and it is not disabled
    }
}
}