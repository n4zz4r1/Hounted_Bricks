using System;
using System.Collections.Generic;
using System.Globalization;
using Core.Data;
using Core.StateMachine.CardAttribute;
using Core.StateMachine.Cards;
using Core.StateMachine.CardSlots;
using Core.StateMachine.Resource;
using Core.StateMachine.ResourceSlider;
using Core.Utils;
using Core.Utils.Constants;
using Framework.Base;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Popup.CardDetail {

public class CardDetailPopup : StateMachine<CardDetailPopup, State<CardDetailPopup>> {
    [SerializeField] public CardPopupComponents components;

    private readonly List<GameObject> currentAttributesObject = new();

    internal CardFSM CardFSM;
    protected override CardDetailPopup FSM => this;
    protected override State<CardDetailPopup> GetInitialState => States.STARTED;

    protected override void Before() {
        components.updateButton.onClick.AddListener(UpdateCard);
    }

    // public static GameObject Create(CardFSM cardFSM, Transform transform) {
    //     var detailPopupPrefab = Resources.Load("UI_CardInfoPopup") as GameObject;
    //     var detailPopup = Instantiate(detailPopupPrefab, transform);
    //
    //     detailPopup.GetComponent<CardDetailPopup>().CardSetup(cardFSM);
    //
    //     return detailPopup;
    // }

    public void UpdateCard() {
        var resourcesNeeded = GameMathUtils.GenerateUpdateCostByLevel(CardFSM.Level() + 1);

        if (ResourcesV1.Instance.HasEnoughResource(ResourceType.ROCK_SCROLL, resourcesNeeded)) {
            CardsDataV1.Instance.IncreaseLevel(CardFSM.cardId, ResourceType.ROCK_SCROLL);
            SyncAllData(typeof(CardFSM));
            SyncAllData(typeof(ResourceFSM));
            SyncAllData(typeof(CardSlotFSM));
            SyncAllData(typeof(CardDetailPopup));
        }
    }

    protected override void SyncDataBase() {
        components.currentRarityText.text = CardFSM.GetCardRarityText;
        var level = CardsDataV1.Instance.GetCardLevel(CardFSM.cardId);
        if (level == 0) {
            components.levelBox.SetActive(false);
        }
        else {
            components.levelBox.SetActive(true);
            components.levelText.text = CardsDataV1.Instance.GetCardLevel(CardFSM.cardId).ToString();
        }

        components.cardIcon.sprite = CardFSM.components.cardIcon.sprite;
        const float xPosition = 0.2f;
        var yPosition = 161f;

        foreach (var currentAttributeObject in currentAttributesObject) Destroy(currentAttributeObject);

        // Set Attributes
        foreach (var cardAttributesComponent in CardFSM.attributes.FindAll(a => a.ConcatValue(level) >= 0f)) {
            currentAttributesObject.Add(CardAttributeFSM.Create(CardFSM, cardAttributesComponent.attribute,
                components.attributesBox.transform,
                cardAttributesComponent.attribute == CardAttribute.RARITY
                    ? new Vector3(xPosition, -211f, 0f)
                    : new Vector3(xPosition, yPosition, 0f)));

            if (cardAttributesComponent.attribute != CardAttribute.RARITY)
                yPosition -= 93f;
        }

        components.coinResourceSlider.FillResources(CardFSM, ResourceType.COIN);
        components.puzzleResourceSlider.FillResources(CardFSM, CardTypeUtils.ToResource(CardFSM.cardType));
        components.nextLevelText.text = (level + 1f).ToString(CultureInfo.InvariantCulture);

        var coinCost = GameMathUtils.GenerateUpdateCostByLevel(level + 1);
        var resourceUpdateCost =
            GameMathUtils.GenerateUpdateCostByLevel(level + 1, CardTypeUtils.ToResource(CardFSM.cardType));

        if (ResourcesV1.Instance.HasEnoughResource(ResourceType.COIN, coinCost)
            && ResourcesV1.Instance.HasEnoughResource(CardTypeUtils.ToResource(CardFSM.cardType),
                resourceUpdateCost))
            components.updateButton.interactable = true;
        else
            components.updateButton.interactable = false;

        RefreshGemArea();

        var normal = RarityUtils.From(CardFSM.Rarity).NormalColor;
        foreach (var image in components.objectsToPaintByRarity)
            image.color = normal;
    }

    private void RefreshGemArea() {
        // Set Gems if needed
        if (CardFSM.HasGems()) {
            if (CardFSM.attributes[(int)CardAttribute.GEM_SLOT].ConcatValue(CardFSM.Level()) > 0) {
                components.gemsPageLockedBox.SetActive(false);
                components.gemsPageButton.gameObject.SetActive(true);
            }
            else {
                components.gemsPageLockedBox.SetActive(true);
                components.gemsPageButton.gameObject.SetActive(false);
            }
        }
        else {
            components.gemsPageLockedBox.SetActive(false);
            components.gemsPageButton.gameObject.SetActive(false);
        }
    }

    public void CardSetup(CardFSM cardFSM) {
        CardFSM = cardFSM;
        // components.cardTypeIcon.sprite = cardFSM.components.cardTypeIcon.sprite;
        components.titleText.text = cardFSM.GetCardTitle;
        components.quantityText.text = CardsDataV1.Instance.GetCardQuantity(cardFSM.cardId).ToString();
        components.quantityMaxText.text = cardFSM.MaxQuantity.ToString();
        components.cardTypeText.text = cardFSM.GetCardTypeText;
        components.descriptionText.text = cardFSM.GetCardFullDetail;
        SyncDataBase();
    }
}

[Serializable]
public class CardPopupComponents {
    [SerializeField] public Image cardIcon;
    [SerializeField] public TextMeshProUGUI titleText;
    [SerializeField] public TextMeshProUGUI descriptionText;
    [SerializeField] public TextMeshProUGUI cardTypeText;
    [SerializeField] public TextMeshProUGUI currentRarityText;
    [SerializeField] public Image cardTypeIcon;
    [SerializeField] public TextMeshProUGUI quantityText;
    [SerializeField] public TextMeshProUGUI quantityMaxText;
    [SerializeField] public TextMeshProUGUI levelText;
    [SerializeField] public GameObject levelBox;
    [SerializeField] public GameObject attributesBox;
    [SerializeField] public Button updateButton;

    [SerializeField] public GameObject detailsPageBox;
    [SerializeField] public GameObject gemsPageBox;
    [SerializeField] public Button detailsPageButton;
    [SerializeField] public Button gemsPageButton;
    [SerializeField] public GameObject gemsPageLockedBox;

    [SerializeField] public ResourceSliderFSM coinResourceSlider;
    [SerializeField] public ResourceSliderFSM puzzleResourceSlider;
    [SerializeField] public TextMeshProUGUI nextLevelText;

    [SerializeField] public List<Image> objectsToPaintByRarity;
}

}