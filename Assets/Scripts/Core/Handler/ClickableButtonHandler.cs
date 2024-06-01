using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Core.Handler {

public class ClickableButtonHandler : MonoBehaviour, IPointerClickHandler {
    // Define an action to hold the callback
    public UnityAction<Vector2> onClickCallback;

    public void OnPointerClick(PointerEventData eventData) {
        var screenPosition = eventData.position;
        // World Position
        if (Camera.main == null) return;

        var worldPosition =
            Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y,
                Camera.main.nearClipPlane));
        onClickCallback?.Invoke(worldPosition);
    }
}

}