using System;
using System.Collections.Generic;
using System.Globalization;
using Core.Data;
using Core.Sprites;
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
    internal CardDetailTab CurrentTab = CardDetailTab.Detail;

    protected override void Before() {
        components.updateButton.onClick.AddListener(UpdateCard);

        for (var i = 0; i < components.tabBoxes.Length; i++) {
            var selected = (CardDetailTab) i;
            components.tabButtons[i].onClick.AddListener(() => SelectTab(selected));
        }
    }

    private void SelectTab(CardDetailTab tabSelected) {
        // if (tabSelected == CurrentTab) return;
        components.tabButtons[(int) CurrentTab].interactable = true;
        components.tabBoxes[(int) CurrentTab].SetActive(false);
        components.tabLabels[(int) CurrentTab].color = Colors.PRIMARY;
        components.tabButtons[(int) CurrentTab].image.color = Colors.DARK_WOOD;

        components.tabButtons[(int) tabSelected].interactable = false;
        components.tabBoxes[(int) tabSelected].SetActive(true);
        components.tabLabels[(int) tabSelected].color = Colors.DARK_WOOD;
        components.tabButtons[(int) tabSelected].image.color = Colors.PRIMARY;

        CurrentTab = tabSelected;
    }

    public void UpdateCard() {
        var resourcesNeeded = GameMathUtils.GenerateUpdateCostByLevel(CardFSM.Level() + 1);

        if (ResourcesV1.Instance.HasEnoughResource(ResourceType.RockScroll, resourcesNeeded)) {
            CardsDataV1.Instance.IncreaseLevel(CardFSM.cardId, ResourceType.RockScroll);
            SyncAllData(typeof(CardFSM));
            SyncAllData(typeof(ResourceFSM));
            SyncAllData(typeof(CardSlotFSM));
            SyncAllData(typeof(CardDetailPopup));
        }
    }

    protected override void SyncDataBase() {
        components.currentRarityText.text = CardFSM.GetCardRarityText;
        var level = CardsDataV1.Instance.GetCardLevel(CardFSM.cardId);
        if (!CardFSM.HasLevel) {
            components.levelBox.SetActive(false);
        } else {
            components.levelBox.SetActive(true);
            components.levelText.text = CardsDataV1.Instance.GetCardLevel(CardFSM.cardId).ToString();
        }

        components.cardIcon.sprite = CardFSM.components.cardIcon.sprite;
        const float xPosition = 0.2f;
        var yPosition = 161f;

        foreach (var currentAttributeObject in currentAttributesObject) Destroy(currentAttributeObject);

        // Set Attributes
        foreach (var cardAttributesComponent in CardFSM.attributes.FindAll(a => a.ConcatValue(level) >= 0f)) {
            currentAttributesObject.Add(CardAttributeFSM.Create(CardFSM, cardAttributesComponent.attributeType,
                components.attributesBox.transform,
                cardAttributesComponent.attributeType == Sprites.CardAttributeType.Rarity
                    ? new Vector3(xPosition, -211f, 0f)
                    : new Vector3(xPosition, yPosition, 0f)));

            if (cardAttributesComponent.attributeType != Sprites.CardAttributeType.Rarity)
                yPosition -= 93f;
        }

        components.coinResourceSlider.FillResources(CardFSM, ResourceType.Coin);
        components.puzzleResourceSlider.FillResources(CardFSM, CardTypeUtils.ToResource(CardFSM.cardType));
        components.nextLevelText.text = (level + 1f).ToString(CultureInfo.InvariantCulture);

        var coinCost = GameMathUtils.GenerateUpdateCostByLevel(level + 1);
        var resourceUpdateCost =
            GameMathUtils.GenerateUpdateCostByLevel(level + 1, CardTypeUtils.ToResource(CardFSM.cardType));

        if (ResourcesV1.Instance.HasEnoughResource(ResourceType.Coin, coinCost)
            && ResourcesV1.Instance.HasEnoughResource(CardTypeUtils.ToResource(CardFSM.cardType),
                resourceUpdateCost))
            components.updateButton.interactable = true;
        else
            components.updateButton.interactable = false;


        var normal = RarityUtils.From(CardFSM.Rarity).NormalColor;
        foreach (var image in components.objectsToPaintByRarity)
            image.color = normal;
    }

    public void CardSetup(CardFSM cardFSM, CardDetailTab tab = CardDetailTab.Detail) {
        CardFSM = cardFSM;
        components.cardTypeIcon.sprite = AssetLoader.AsSprite(cardFSM.cardType);
        components.titleText.text = cardFSM.GetCardTitle;
        components.quantityText.text = CardsDataV1.Instance.GetCardQuantity(cardFSM.cardId).ToString();
        components.quantityMaxText.text = cardFSM.MaxQuantity.ToString();
        components.cardTypeText.text = cardFSM.GetCardTypeText;
        components.descriptionText.text = cardFSM.GetCardFullDetail;
        
        SelectTab(tab);

        SetupAllAbilities();
        
        SyncDataBase();
    }

    private void SetupAllAbilities() {
        float currentX = -344f, currentY = 146f;
        var availableAbilities = CardsDataV1.Instance.GetAbilitiesAvailable(CardFSM.cardId);

        foreach (var slot in components.abilitySlots) 
            slot.SetPlayer(CardFSM.cardId);

        for (var i = 0; i < availableAbilities.Count; i++) {

            var abilityCardInstance = 
                Instantiate(AssetLoader.AsGameObject(availableAbilities[i]), components.abilityBox.transform);

            abilityCardInstance.transform.localPosition = new Vector3(currentX, currentY, 0);
            abilityCardInstance.GetComponent<CardFSM>().MakeDraggableCard(components.abilityDragArea);
            
            // Next line
            if (i != 0 && (i + 1) % 5 == 0) {
                currentX = -344f;
                currentY -= 271f;
            } else 
                currentX += 174f;
            
        }

    }
}

[Serializable]
public class CardPopupComponents {

    [SerializeField] public GameObject[] tabBoxes;
    [SerializeField] public Button[] tabButtons;
    [SerializeField] public TextMeshProUGUI[] tabLabels;
    [SerializeField] public GameObject[] tabDots;

    #region Abilities

    [SerializeField] public GameObject abilityBox;
    [SerializeField] public GameObject abilityDragArea;
    [SerializeField] public List<CardAbilitySlotFSM> abilitySlots;

    #endregion
    
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

    [SerializeField] public ResourceSliderFSM coinResourceSlider;
    [SerializeField] public ResourceSliderFSM puzzleResourceSlider;
    [SerializeField] public TextMeshProUGUI nextLevelText;

    [SerializeField] public List<Image> objectsToPaintByRarity;
}

public enum CardDetailTab {
    Detail = 0,
    Abilities = 1
}

}