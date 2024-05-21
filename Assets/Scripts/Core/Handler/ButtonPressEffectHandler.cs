using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Core.Handler {

public class ButtonPressEffectHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public float moveDistance = 5f; // Distance to move the children

    public void OnPointerDown(PointerEventData eventData) => MoveDown();

    public void OnPointerUp(PointerEventData eventData) => MoveUp();
    public void MoveUp() => MoveChildren(new Vector3(0, moveDistance, 0));
    public void MoveDown() => MoveChildren(new Vector3(0, -moveDistance, 0));

    private void MoveChildren(Vector3 movement) {

        // only move if button is not disabled
        if (GetComponent<Button>() == null || !GetComponent<Button>().enabled) return;
        
        foreach (Transform child in transform)
            child.localPosition += movement;
    }
    
}

}