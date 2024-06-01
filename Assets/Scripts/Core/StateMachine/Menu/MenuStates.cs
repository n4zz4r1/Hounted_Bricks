using Core.Utils.Constants;
using DG.Tweening;
using Framework.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Core.StateMachine.Menu {

public abstract class States {
    public static readonly Started Started = new();
}

public class Started : State<MenuFSM> {
    public override void Before(MenuFSM fsm) {
        // Set Buttons
        for (var i = 0; i < fsm.menuButtons.Count; i++) {
            var index = i;
            fsm.menuButtons[i].onClick.AddListener(() => { ChangePanel(fsm, index); });
        }

        fsm.menuButtons[fsm.CurrentPanelIndex].enabled = false;
        SyncPosition(fsm);
    }

    private static void ChangePanel(MenuFSM fsm, int newPanelIndex) {
        // First. Move new panel to main anchor and enable it
        fsm.panels[newPanelIndex].anchoredPosition = new Vector2(0, 0);
        fsm.panels[newPanelIndex].gameObject.SetActive(true);

        // Second. Disable 
        fsm.panels[fsm.CurrentPanelIndex].gameObject.SetActive(false);

        // Third, Disable current scene button
        fsm.menuButtons[newPanelIndex].enabled = false;
        fsm.menuButtons[fsm.CurrentPanelIndex].enabled = true;

        // Finally, change current panel and re-sync
        fsm.CurrentPanelIndex = newPanelIndex;
        SyncPosition(fsm);
    }

    private static void SyncPosition(MenuFSM fsm) {
        const float anchorFactor = 1f / 6f;
        var currentAnchor = 0f;
        const float effectDurationTime = 0.35f;

        for (var i = 0; i < fsm.menuButtons.Count; i++) {
            var rectTransform = fsm.menuButtons[i].GetComponent<RectTransform>();
            var newAnchorMin = new Vector2(currentAnchor, rectTransform.anchorMin.y);

            var widthFactor = fsm.PanelByIndex[fsm.CurrentPanelIndex] == fsm.PanelByIndex[i]
                ? anchorFactor * 2
                : anchorFactor;
            currentAnchor += widthFactor;
            var newAnchorMax = new Vector2(currentAnchor, rectTransform.anchorMax.y);

            DOTween.To(() => rectTransform.anchorMin, x => rectTransform.anchorMin = x, newAnchorMin, effectDurationTime)
                .SetEase(Ease.InOutQuad);
            DOTween.To(() => rectTransform.anchorMax, x => rectTransform.anchorMax = x, newAnchorMax, effectDurationTime)
                .SetEase(Ease.InOutQuad);

            var isSelected = fsm.PanelByIndex[fsm.CurrentPanelIndex] == fsm.PanelByIndex[i];
            
            rectTransform.GetComponent<Image>().DOColor(isSelected ? Colors.PRIMARY : Colors.DARK_WOOD, effectDurationTime);
            fsm.icons[i].DOLocalMoveY(isSelected ? 80f : 60f, effectDurationTime, true);
            // fsm.labels[i].DOFade(isSelected ? 1f : 5f, effectDurationTime);
            fsm.labels[i].color = isSelected ? Colors.DARK_WOOD : Colors.PRIMARY;
            fsm.labels[i].fontSizeMax = isSelected ? 50f : 20f;

        }
    }
}

}