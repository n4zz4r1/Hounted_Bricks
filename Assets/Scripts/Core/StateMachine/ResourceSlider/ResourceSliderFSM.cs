using System;
using Core.Data;
using Core.Sprites;
using Core.StateMachine.Cards;
using Core.Utils;
using DG.Tweening;
using Framework.Base;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.StateMachine.ResourceSlider {
public class ResourceSliderFSM : StateMachine<ResourceSliderFSM, State<ResourceSliderFSM>> {
    [SerializeField] public ResourceType resourceType = ResourceType.None;
    [SerializeField] public Components components;

    private CardFSM _cardFSM;
    protected override ResourceSliderFSM FSM => this;
    protected override State<ResourceSliderFSM> GetInitialState => States.Preload;

    protected override void Before() {
        components.resourceImage.sprite = AssetLoader.AsSprite(resourceType);
    }

    public void FillResources(CardFSM cardFSM, ResourceType type) {
        resourceType = type;
        FillResources(cardFSM);
    }

    private void FillResources(CardFSM cardFSM) {
        _cardFSM = cardFSM;
        SyncDataBase();
    }

    protected override void SyncDataBase() {
        const float effectsDuration = 0.25f;
        var maxValue = GameMathUtils.GenerateUpdateCostByLevel(_cardFSM.Level() + 1, resourceType);
        components.resourceSlider.maxValue = maxValue;
        var currentAmount = (int)ResourcesV1.Instance.GetResourcesAmount(resourceType);
        if (currentAmount > components.resourceSlider.maxValue)
            currentAmount = (int)components.resourceSlider.maxValue;

        components.resourceSlider.DOValue(currentAmount, effectsDuration).SetEase(Ease.OutQuad);

        var currentValue = 0;

        DOTween.To(() => currentValue, x => currentValue = x, currentAmount, effectsDuration)
            .OnUpdate(() =>
                components.requiredResourcesCounter.text =
                    currentValue.ToString("N0") + " / " + maxValue.ToString("N0"))
            .SetEase(Ease.OutQuad);
    }
}

[Serializable]
public class Components {
    [SerializeField] public Slider resourceSlider;
    [SerializeField] public Image resourceImage;
    [SerializeField] public TextMeshProUGUI requiredResourcesCounter;
}
}