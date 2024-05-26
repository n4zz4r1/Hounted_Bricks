using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Core.Popup {

public class BasePopup : MonoBehaviour {
    [SerializeField] public TextMeshProUGUI title;
    [SerializeField] public RectTransform rectTransform;

    public virtual void Show() {
        rectTransform.localScale = Vector3.zero;
        rectTransform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutQuad);
    }

    public void ClosePopup() {
        // rectTransform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.OutQuad).OnComplete(() => Destroy(gameObject));
        Destroy(gameObject);
    }
}

public enum Popups {
    SettingsPopup,
    CardDetailPopup,
    DeckPopup
}

}