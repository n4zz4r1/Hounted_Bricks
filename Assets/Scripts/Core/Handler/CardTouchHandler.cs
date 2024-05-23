using Core.StateMachine.Cards;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Core.Handler {

public class CardTouchHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler,
    IPointerEnterHandler {
    // [FormerlySerializedAs("CurrentCardFSM")] [SerializeField]
    // private CardFSM currentCardFSM;
    //
    // private bool _isDraggingCard;
    // private bool _isPointerDown;
    //
    // public void OnPointerDown(PointerEventData eventData) {
    //     _isPointerDown = true;
    // }
    //
    // public void OnPointerEnter(PointerEventData eventData) { }
    //
    // public void OnPointerExit(PointerEventData eventData) {
    //     if (_isDraggingCard || !_isPointerDown || !currentCardFSM.HasAvailableCards())
    //         return;
    //
    //     currentCardFSM.State.StartDragging(currentCardFSM);
    //     _isDraggingCard = true;
    // }
    //
    // public void OnPointerUp(PointerEventData eventData) {
    //     _isPointerDown = false;
    //     if (!_isDraggingCard)
    //         return;
    //     currentCardFSM.State.StopDragging(currentCardFSM);
    //     _isDraggingCard = false;
    // }

    public void OnPointerDown(PointerEventData eventData) {
        // throw new System.NotImplementedException();
    }

    public void OnPointerUp(PointerEventData eventData) {
        // throw new System.NotImplementedException();
    }

    public void OnPointerExit(PointerEventData eventData) {
        // throw new System.NotImplementedException();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        // throw new System.NotImplementedException();
    }
}

}