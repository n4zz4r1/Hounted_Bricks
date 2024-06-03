using Core.Popup;
using Core.Popup.CardDetail;
using Core.StateMachine.Cards;
using Core.Utils;
using Core.Utils.Constants;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;

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
            if (cardFSMIfNeeded == null)
                cardFSMIfNeeded = AssetLoader.AsComponent<CardFSM>(cardIfNeeded);

            instance.GetComponent<CardDetailPopup>().CardSetup(cardFSMIfNeeded, popupTabIfNeeded);
        }
        // instance.cam
            
        instance.GetComponent<BasePopup>().Show();
    }
}

}