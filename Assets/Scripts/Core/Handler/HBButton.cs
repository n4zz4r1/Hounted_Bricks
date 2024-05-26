using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace Core.Handler {

public class HBButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Button States")]
    public Sprite normalSprite;
    public Sprite disabledSprite;
    public Sprite pressedSprite;

    [Header("Settings")]
    public bool isDisabledByDefault = false;

    [Header("Events")]
    public UnityEvent onClick;

    private Image buttonImage;
    private Vector3 originalPosition;

    public Dropdown color;
    
    
    public void OnPointerDown(PointerEventData eventData) {
        if (isDisabledByDefault) return;
        
        buttonImage.sprite = pressedSprite;
        transform.localPosition = originalPosition + new Vector3(0, -5f, 0);
    }

    public void OnPointerUp(PointerEventData eventData) {
        if (isDisabledByDefault) return;
        
        buttonImage.sprite = normalSprite;
        transform.localPosition = originalPosition;
        onClick?.Invoke(); // Trigger onClick event on mouse release
    }
    
    // private void Awake()
    // {
    //     buttonImage = GetComponent<Image>();
    //     originalPosition = transform.localPosition;
    //
    //     if (colorDropdown != null)
    //     {
    //         colorDropdown.onValueChanged.AddListener(OnColorChanged);
    //     }
    //
    //     if (buttonText != null && !string.IsNullOrEmpty(localizationKey))
    //     {
    //         buttonText.text = LocalizationSettings.StringDatabase.GetLocalizedString("YourLocalizationTable", localizationKey);
    //     }
    //
    //     if (isDisabledByDefault)
    //     {
    //         DisableButton();
    //     }
    //     else
    //     {
    //         EnableButton();
    //     }
    // }
    //
    // public void OnPointerDown(PointerEventData eventData)
    // {
    //     if (!isDisabledByDefault)
    //     {
    //         buttonImage.sprite = pressedSprite;
    //         transform.localPosition = originalPosition + new Vector3(0, -5f, 0);
    //     }
    // }
    //
    // public void OnPointerUp(PointerEventData eventData)
    // {
    //     if (!isDisabledByDefault)
    //     {
    //         buttonImage.sprite = normalSprite;
    //         transform.localPosition = originalPosition;
    //     }
    // }
    //
    // public void OnPointerClick(PointerEventData eventData)
    // {
    //     if (!isDisabledByDefault && onClick != null)
    //     {
    //         onClick.Invoke();
    //     }
    // }
    //
    // public void EnableButton()
    // {
    //     isDisabledByDefault = false;
    //     buttonImage.sprite = normalSprite;
    //     buttonImage.color = GetSelectedColor();
    // }
    //
    // public void DisableButton()
    // {
    //     isDisabledByDefault = true;
    //     buttonImage.sprite = disabledSprite;
    // }
    //
    // private void OnColorChanged(int index)
    // {
    //     buttonImage.color = GetSelectedColor();
    // }
    //
    // private Color GetSelectedColor()
    // {
    //     switch (colorDropdown.value)
    //     {
    //         case 0:
    //             return color1;
    //         case 1:
    //             return color2;
    //         case 2:
    //             return color3;
    //         default:
    //             return color1;
    //     }
    // }
}

}
