using Game.Controller.Game;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Handler {

public class GameTouchHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler,
    IPointerEnterHandler {
    [SerializeField] public GameController gameController;
    private bool _isPointerDown;

    public void OnPointerDown(PointerEventData eventData) {
        if (!CanStartShooting()) return;

        _isPointerDown = true;
        gameController.AimStart();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (!CanStartShooting()) return;

        if (_isPointerDown) gameController.AimStart();
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (!CanStartShooting()) return;

        if (_isPointerDown) gameController.AimExit();
    }

    public void OnPointerUp(PointerEventData eventData) {
        if (!CanStartShooting()) return;

        _isPointerDown = false;
        gameController.StartShooting(eventData.position);
    }

    private bool CanStartShooting() {
        return gameController.State == States.PlayerTurn;
    }
}

}