using System;
using Core.Utils;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Core.Popup {

public class BasePopup : MonoBehaviour {
    [SerializeField] public TextMeshProUGUI title;
    [SerializeField] public RectTransform rectTransform;
    [SerializeField] public Canvas popupCanvas;

    public virtual void Show() {
        rectTransform.localScale = Vector3.zero;
        rectTransform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutQuad);
        popupCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        popupCanvas.worldCamera = Camera.main;
        popupCanvas.sortingLayerID = 4;
        popupCanvas.sortingLayerName = "Above All";
        popupCanvas.sortingOrder = 100;



    }

    public void ClosePopup() {
        Destroy(gameObject);
    }
}

[Serializable]
public enum Popups {
    Settings,
    CardDetail,
    Deck
}

}