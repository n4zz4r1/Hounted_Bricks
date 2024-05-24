using Core.Popup;
using Core.Popup.CardDetail;
using Core.StateMachine.Cards;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.Handler {

public class OpenPopupHandler : MonoBehaviour, IPointerClickHandler {
    [SerializeField] public Popups popup = Popups.SettingsPopup;
    [SerializeField] public CardFSM cardFSMIfNeeded;

    public void OnPointerClick(PointerEventData eventData) {
        var popupAsset = Addressables.LoadAssetAsync<GameObject>(popup.ToString());
        popupAsset.Completed += AsyncCompleted;
    }

    private void AsyncCompleted(AsyncOperationHandle<GameObject> obj) {
        var instance = Instantiate(obj.Result, transform.root.transform);
        if (popup == Popups.CardDetailPopup)
            instance.GetComponent<CardDetailPopup>().CardSetup(cardFSMIfNeeded);
    }
}

}