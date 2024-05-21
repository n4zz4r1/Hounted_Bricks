using Core.Popup;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.Handler {

public class OpenPopupHandler : MonoBehaviour, IPointerClickHandler {
    [SerializeField] public Popups popup = Popups.SettingsPopup;


    public void OnPointerClick(PointerEventData eventData) {
        var popupAsset = Addressables.LoadAssetAsync<GameObject>(popup.ToString());
        popupAsset.Completed += AsyncCompleted;
    }

    private void AsyncCompleted(AsyncOperationHandle<GameObject> obj) {
        Instantiate(obj.Result, transform.root.transform);
    }
}

}