using System;
using System.Collections.Generic;
using Core.Controller.Bag;
using Core.Data;
using Core.Handler;
using Core.StateMachine.CardSlots;
using Core.Utils;
using Core.Utils.Constants;
using Framework.Base;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Core.StateMachine.Cards {

public class CardFSM : StateMachine<CardFSM, State<CardFSM>> {
    #region Level Box

    [SerializeField] public int maxLevel = 10;

    #endregion

    #region CardAttributes

    [SerializeField] public List<CardAttributesComponent> attributes;

    #endregion

    /// <summary>
    ///     OLD BELOW
    /// </summary>
    [FormerlySerializedAs("CardId")] [SerializeField]
    public Card cardId = Card.NONE;

    [FormerlySerializedAs("CardType")] [SerializeField]
    public CardType cardType = CardType.NONE;

    // [FormerlySerializedAs("CardUpdate")] [SerializeField]
    // public Card cardUpdate = Card.NONE;

    [FormerlySerializedAs("ShowQuantity")] [SerializeField]
    public bool showQuantity;

    [FormerlySerializedAs("HideInfo")] [SerializeField]
    public bool hideInfo;

    [FormerlySerializedAs("DragEnabled")] [SerializeField]
    public bool dragEnabled;

    [FormerlySerializedAs("CardAbility")] [SerializeField]
    public CardAbilityComponent cardAbility;

    // [SerializeField] public AbilityFSM abilityFSM; TODO abilities
    [SerializeField] public bool createAsDisable;

    [FormerlySerializedAs("Internal")] [SerializeField]
    public Components components;

    protected override CardFSM FSM => this;
    protected override State<CardFSM> GetInitialState => States.Created;

    public bool IsDragging { get; set; }
    public long CurrentQuantity { get; private set; }
    public GameObject DragCard { get; private set; }
    public CardSlotFSM SelectedSlot { get; set; }

    public CardRarity Rarity => (CardRarity)(int)attributes[(int)CardAttribute.RARITY].ConcatValue(Level());

    public static Sprite GetCardIcon(Card card) {
        var cardObject = Resources.Load(card.ToString().Replace("Card_", "")) as GameObject;
        if (cardObject == null)
            throw new Exception("Card doesnt exist");

        return cardObject.GetComponent<CardFSM>().components.cardIcon.sprite;
    }

    public static CardFSM GetRawCardFSM(Card card) {
        var cardPrefab = Resources.Load(card.ToString().Replace("Card_", "")) as GameObject;
        if (cardPrefab == null)
            throw new Exception("Card " + card + " doesnt exist");

        var cardObjectInstance = Instantiate(cardPrefab);
        var cardFSM = cardObjectInstance.GetComponent<CardFSM>();
        cardFSM.hideInfo = true;
        cardFSM.dragEnabled = false;
        cardFSM.showQuantity = false;
        cardFSM.createAsDisable = false;
        cardFSM.components.boxQuantity.SetActive(false);
        return cardFSM;
    }

    public static CardFSM GetRawDisabledCardFSM(Card card) {
        var cardPrefab = Resources.Load(card.ToString().Replace("Card_", "")) as GameObject;
        if (cardPrefab == null)
            throw new Exception("Card " + card + " doesnt exist");

        var cardObjectInstance = Instantiate(cardPrefab);
        var cardFSM = cardObjectInstance.GetComponent<CardFSM>();
        cardFSM.hideInfo = true;
        cardFSM.dragEnabled = false;
        cardFSM.showQuantity = false;
        cardFSM.createAsDisable = true;
        return cardFSM;
    }

    internal bool HasGems() {
        return attributes[(int)CardAttribute.GEM_SLOT].updates.Count != 0;
    }

    public void DestroyInfoButton() {
        Destroy(components.infoButton.gameObject);
    }

    public void DestroyCardBox() {
        Destroy(components.boxCard.gameObject);
    }

    public void DestroyCardNotFoundBox() {
        Destroy(components.boxCardNotFound.gameObject);
    }

    public static Sprite GetIconFromCard(CardFSM FSM) {
        return FSM.components.cardIcon.sprite;
    }

    public static Sprite GetIconFromCard(Card card) {
        var cardFSM = GetRawCardFSM(card);
        return cardFSM.components.cardIcon.sprite;
    }

    public int Level() {
        var cardLevel = CardsDataV1.Instance.GetCardLevel(cardId);
        return maxLevel <= 1 ? 1 : cardLevel;
    }

    public bool IsMaxLevel() {
        return CardsDataV1.Instance.GetCardLevel(cardId) == maxLevel;
    }

    protected override void Before() {
        // Set Button Callback
        // components.upgradeButton.onClick.AddListener(IncreaseLevel);
        components.infoButton.onClick.AddListener(OpenPopup);

        if (!dragEnabled) components.dragHandler.enabled = false;

        if (cardAbility.abilityType is not AbilityType.NONE) components.boxAbilitySlot.SetActive(true);

        if (cardAbility.abilityType is AbilityType.IMPROVEMENT)
            components.iconAbilitySlotImprovement.SetActive(true);
        else if (cardAbility.abilityType is AbilityType.CONSUME_PER_TURN)
            components.iconAbilitySlotTurnBased.SetActive(true);
        else if (cardAbility.abilityType is AbilityType.CONSUME_ONCE)
            components.iconAbilitySlotConsumable.SetActive(true);

        // PaintCard(Rarity);
    }
    // 2560x1440

    public void PaintCard(CardRarity cardRarity) {
        var normal = RarityUtils.From(cardRarity).NormalColor;

        // TODO check colors
        // var withAlpha = ColorUtils.WithAlpha(normal, 0.5f);

        foreach (var image in components.imagesToPaint)
            image.color = normal;
    }

    public void PaintCardDisabled() {
        foreach (var image in components.imagesToPaintDisabled)
            image.color = Colors.DISABLED;
    }

    private string GetCardName() {
        return cardId.ToString().Replace("Card_", "");
    }

    private void SetupCardOnly() {
        components.infoButton.gameObject.SetActive(false);
    }

    public void CreateClone() {
        var cardToDrag = Resources.Load(GetCardName()) as GameObject;
        if (cardToDrag != null) {
            cardToDrag.GetComponent<CardFSM>().showQuantity = false;
            var whereTo = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            cardToDrag.transform.position = new Vector3(whereTo.x, whereTo.y, 10);
            cardToDrag.GetComponent<CardFSM>().components.areaQuantity.SetActive(false);

            var dragArea = gameObject.transform.root.GetComponentInChildren<BagController>()?.components
                .dragArea.transform;
            DragCard = CreateInstance(cardToDrag, dragArea);
        }

        DragCard.GetComponent<CardFSM>().SetupCardOnly();
    }

    public string GetCardTitle() {
        return LocalizationUtils.GetLocalizedText(GetCardName() + ".Title");
    }

    public string GetCardDescription() {
        return LocalizationUtils.GetLocalizedText(GetCardName() + ".Description");
    }

    public void UpdateCurrentAvailable(long quantity) {
        // only allow disable card if is draggable
        if (!dragEnabled)
            return;

        CurrentQuantity = quantity;
        components.currentQuantity.text = quantity.ToString();
        SyncCardColors();
    }

    public string GetCardFullDetail() {
        return LocalizationUtils.GetLocalizedText(GetCardName() + ".Detail");
    }

    public string GetCardDetail2() {
        return LocalizationUtils.GetLocalizedText(GetCardName() + ".Detail2");
    }

    public string GetCardDetail3() {
        return LocalizationUtils.GetLocalizedText(GetCardName() + ".Detail3");
    }

    public bool HasAvailableCards() {
        return CurrentQuantity > 0;
    }

    internal void SyncCardColors() {
        if (!HasAvailableCards() && dragEnabled) {
            components.cardShadow.enabled = false;
            PaintCardDisabled();
        }
        else {
            components.cardShadow.enabled = true;
            PaintCard(Rarity);
        }
    }

    public void DestroyDragCard() {
        DestroyImmediate(DragCard, true);
    }
}

[Serializable]
public class Components {
    /// <summary>
    ///     OLD
    /// </summary>
    [FormerlySerializedAs("CardIcon")] public Image cardIcon;

    [FormerlySerializedAs("InfoButton")] public Button infoButton;

    [FormerlySerializedAs("CurrentQuantity")]
    public TextMeshProUGUI currentQuantity;

    [FormerlySerializedAs("TotalQuantity")]
    public TextMeshProUGUI totalQuantity;

    [FormerlySerializedAs("CardTitleText")]
    public TextMeshProUGUI cardTitleText;

    [FormerlySerializedAs("CardDescrText")]
    public TextMeshProUGUI cardDescriptionText;

    [FormerlySerializedAs("BoxCard")] public GameObject boxCard;

    [FormerlySerializedAs("BoxCardNotFound")]
    public GameObject boxCardNotFound;

    [FormerlySerializedAs("CardShadow")] public Shadow cardShadow;

    [FormerlySerializedAs("DragHandler")] public CardTouchHandler dragHandler;

    [FormerlySerializedAs("BoxAbilitySlot")]
    public GameObject boxAbilitySlot;

    [FormerlySerializedAs("IconAbilitySlotConsumable")]
    public GameObject iconAbilitySlotConsumable;

    [FormerlySerializedAs("IconAbilitySlotTurnBased")]
    public GameObject iconAbilitySlotTurnBased;

    [FormerlySerializedAs("IconAbilitySlotImprovement")]
    public GameObject iconAbilitySlotImprovement;

    [FormerlySerializedAs("IconCardForNotFound")]
    public Image iconCardForNotFound;

    [FormerlySerializedAs("BoxCardImage")] public Image boxCardImage;

    [FormerlySerializedAs("BoxCardIconImage")]
    public Image boxCardIconImage;

    [FormerlySerializedAs("BoxCardDescImage")]
    public Image boxCardDescImage;

    [SerializeField] public GameObject cardUpdateBox;
    [SerializeField] public List<Image> cardUpdateBoxSlots = new();
    [SerializeField] public List<Sprite> cardUpdateSprites = new();

    // All components to print
    [SerializeField] public List<Image> imagesToPaint;
    [SerializeField] public List<Image> imagesToPaintDisabled;

    // Card Canvas Group
    [SerializeField] public CanvasGroup cardCanvasGroup;

    #region LevelBox

    [SerializeField] public GameObject levelBox;
    [SerializeField] public TextMeshProUGUI levelText;
    [SerializeField] public TextMeshProUGUI levelCostText;
    [SerializeField] public Slider levelSlider;
    [SerializeField] public GameObject levelKnot;

    #endregion

    #region Quantity

    [SerializeField] public GameObject boxQuantity;
    [SerializeField] public Image boxQuantityProgress;
    [SerializeField] public TextMeshProUGUI textQuantity;
    [SerializeField] public GameObject areaQuantity;

    #endregion

    #region CardImages

    [SerializeField] public Image cardTypeIcon;
    [SerializeField] public List<Sprite> cardTypeIcons;

    #endregion

    #region Gems

    [SerializeField] public GameObject gemsArea;
    [SerializeField] public List<Image> gemsSlot;
    [SerializeField] public List<Image> gemsSlotIcon;

    #endregion
}

}