using Core.Popup;
using Core.Popup.CardDetail;
using Core.StateMachine.Cards;
using Core.Utils;
using Core.Utils.Constants;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Handler {

public class OpenPopupHandler : MonoBehaviour, IPointerClickHandler {

    [SerializeField] public bool autoClick = true;
    [SerializeField] public Popups popup = Popups.Settings;
    [SerializeField] public CardFSM cardFSMIfNeeded;
    [SerializeField] public Card cardIfNeeded = Card.NONE;
    [SerializeField] public CardDetailTab popupTabIfNeeded = CardDetailTab.Detail;

    public void OnPointerClick(PointerEventData _) {
        if (!autoClick) return;

        OpenPopup();
    }

    public void OpenPopup() {
        
        var instance = Instantiate(AssetLoader.AsGameObject(popup), transform.root.transform);
        if (popup == Popups.CardDetail) {
            instance.GetComponent<CardDetailPopup>()
                .CardSetup(cardFSMIfNeeded == null ? cardIfNeeded : cardFSMIfNeeded.cardId, popupTabIfNeeded);
        }

        instance.GetComponent<BasePopup>().Show(popup);
    }
}

}