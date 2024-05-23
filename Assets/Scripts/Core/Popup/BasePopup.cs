using TMPro;
using UnityEngine;

namespace Core.Popup {

public class BasePopup : MonoBehaviour {
    [SerializeField] public TextMeshProUGUI title;

    public void ClosePopup() {
        Destroy(gameObject);
    }
}

public enum Popups {
    SettingsPopup,
    CardDetailPopup,
    DeckPopup,
}

}