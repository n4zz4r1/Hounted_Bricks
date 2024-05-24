using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Controller.Bag;
using Core.Data;
using Core.Handler;
using Core.StateMachine.CardSlots;
using Core.Utils;
using Core.Utils.Constants;
using Framework.Base;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.StateMachine.Cards {

public class CardFSM : StateMachine<CardFSM, State<CardFSM>> {
    protected override CardFSM FSM => this;
    protected override State<CardFSM> GetInitialState => States.Created;

    // #region Level Box
    //
    //
    // #endregion
    //
    // #region CardAttributes
    //
    //
    // #endregion
    //
    // /// <summary>
    // ///     OLD BELOW
    // /// </summary>

    // // [FormerlySerializedAs("CardUpdate")] [SerializeField]
    // // public Card cardUpdate = Card.NONE;
    //
    // [FormerlySerializedAs("ShowQuantity")] [SerializeField]
    // public bool showQuantity;
    //
    // [FormerlySerializedAs("HideInfo")] [SerializeField]
    // public bool hideInfo;
    //
    //
    // [FormerlySerializedAs("CardAbility")] [SerializeField]
    // public CardAbilityComponent cardAbility;
    //
    // // [SerializeField] public AbilityFSM abilityFSM; TODO abilities
    // [SerializeField] public bool createAsDisable;

    public bool IsDragging { get; set; }
    public long CurrentQuantity { get; private set; }
    public GameObject DragCard { get; private set; }
    public CardSlotFSM SelectedSlot { get; set; }

    public static CardFSM GetRawDisabledCardFSM(Card fsmCardSorted) {
        throw new NotImplementedException();
    }

    public static CardFSM GetRawCardFSM(Card card) {
        throw new NotImplementedException();
    }

    public static Sprite GetCardIcon(CardFSM fsmCurrentCard) {
        throw new NotImplementedException();
    }

    protected override void Before() {
        if (!dragEnabled) components.dragHandler.enabled = false;

        // if (cardAbility.abilityType is not AbilityType.NONE) components.boxAbilitySlot.SetActive(true);

        // if (cardAbility.abilityType is AbilityType.IMPROVEMENT)
        //     components.iconAbilitySlotImprovement.SetActive(true);
        // else if (cardAbility.abilityType is AbilityType.CONSUME_PER_TURN)
        //     components.iconAbilitySlotTurnBased.SetActive(true);
        // else if (cardAbility.abilityType is AbilityType.CONSUME_ONCE)
        //     components.iconAbilitySlotConsumable.SetActive(true);

        PaintCard(Rarity);
    }

    protected override async Task BeforeAsync() {
        GetCardTitle = await LocalizationUtils.LoadText(GetCardName() + ".Title");
        GetCardDescription = await LocalizationUtils.LoadText(GetCardName() + ".Description");
        GetCardFullDetail = await LocalizationUtils.LoadText(GetCardName() + ".Detail");
        GetCardTypeText = await LocalizationUtils.LoadText("CardType." + cardType);
        GetCardRarityText = await LocalizationUtils.LoadText("CardRarity." + Rarity);
        MaxLevelLabel = await LocalizationUtils.LoadText("Label.MaxLevel");
        MaxQuantity = CardsDataV1.Instance.GetCardMaxQuantity(cardId);
    }

    public static Sprite GetCardIcon(Card card) {
        var cardObject = Resources.Load(card.ToString().Replace("Card_", "")) as GameObject;
        if (cardObject == null)
            throw new Exception("Card doesnt exist");

        return cardObject.GetComponent<CardFSM>().components.cardIcon.sprite;
    }
    //
    // public static CardFSM GetRawCardFSM(Card card) {
    //     var cardPrefab = Resources.Load(card.ToString().Replace("Card_", "")) as GameObject;
    //     if (cardPrefab == null)
    //         throw new Exception("Card " + card + " doesnt exist");
    //
    //     var cardObjectInstance = Instantiate(cardPrefab);
    //     var cardFSM = cardObjectInstance.GetComponent<CardFSM>();
    //     cardFSM.hideInfo = true;
    //     cardFSM.dragEnabled = false;
    //     cardFSM.showQuantity = false;
    //     cardFSM.createAsDisable = false;
    //     cardFSM.components.boxQuantity.SetActive(false);
    //     return cardFSM;
    // }
    //
    // public static CardFSM GetRawDisabledCardFSM(Card card) {
    //     var cardPrefab = Resources.Load(card.ToString().Replace("Card_", "")) as GameObject;
    //     if (cardPrefab == null)
    //         throw new Exception("Card " + card + " doesnt exist");
    //
    //     var cardObjectInstance = Instantiate(cardPrefab);
    //     var cardFSM = cardObjectInstance.GetComponent<CardFSM>();
    //     cardFSM.hideInfo = true;
    //     cardFSM.dragEnabled = false;
    //     cardFSM.showQuantity = false;
    //     cardFSM.createAsDisable = true;
    //     return cardFSM;
    // }
    //
    //
    // public void DestroyInfoButton() {
    //     Destroy(components.infoButton.gameObject);
    // }
    //
    // public void DestroyCardBox() {
    //     Destroy(components.boxCard.gameObject);
    // }
    //
    // public void DestroyCardNotFoundBox() {
    //     Destroy(components.boxCardNotFound.gameObject);
    // }
    //
    // public static Sprite GetIconFromCard(CardFSM FSM) {
    //     return FSM.components.cardIcon.sprite;
    // }
    //
    // public static Sprite GetIconFromCard(Card card) {
    //     var cardFSM = GetRawCardFSM(card);
    //     return cardFSM.components.cardIcon.sprite;
    // }
    //
    //

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

    public void CreateClone(GameObject prefab) {
        if (prefab == null) return;

        var whereTo = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var dragArea = gameObject.transform.root.GetComponentInChildren<BagController>()?.components
            .dragArea.transform;
        DragCard = CreateInstance(prefab, dragArea);
        DragCard.GetComponent<CardFSM>().components.boxQuantity.SetActive(false);
        DragCard.transform.position = new Vector3(whereTo.x, whereTo.y, 10);
        DragCard.GetComponent<CardFSM>().components.infoButton.gameObject.SetActive(false);
    }


    public void UpdateCurrentAvailable(long quantity) {
        // only allow disable card if is draggable
        if (!dragEnabled)
            return;

        CurrentQuantity = quantity;
        components.textQuantity.text = quantity.ToString();
        SyncCardColors();
    }

    //
    //
    // public string GetCardDetail2() {
    //     return LocalizationUtils.GetLocalizedText(GetCardName() + ".Detail2");
    // }
    //
    // public string GetCardDetail3() {
    //     return LocalizationUtils.GetLocalizedText(GetCardName() + ".Detail3");
    // }
    //
    public bool HasAvailableCards() {
        return CurrentQuantity > 0;
    }

    //
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

    //
    public void DestroyDragCard() {
        // var dragArea = gameObject.transform.root.GetComponentInChildren<BagController>()?.components
        //     .dragArea.transform;
        Destroy(DragCard);
    }

    #region Card Properties

    [SerializeField] public Card cardId = Card.NONE;
    [SerializeField] public CardType cardType = CardType.NONE;
    [SerializeField] public int maxLevel = 1;
    [SerializeField] public bool dragEnabled;
    [SerializeField] public List<CardAttributesComponent> attributes;
    [SerializeField] public Components components;

    public int MaxQuantity { get; private set; }

    public bool IsMaxLevel() {
        return CardsDataV1.Instance.GetCardLevel(cardId) == maxLevel;
    }

    public int Level() {
        return maxLevel <= 1 ? 1 : CardsDataV1.Instance.GetCardLevel(cardId);
    }

    private string GetCardName() {
        return cardId.ToString().Replace("Card_", "");
    }

    internal bool HasGems() {
        return attributes[(int)CardAttribute.GEM_SLOT].updates.Count != 0;
    }

    public string GetCardTitle { get; private set; } = string.Empty;
    public string GetCardDescription { get; private set; } = string.Empty;
    public string GetCardFullDetail { get; private set; } = string.Empty;
    public string GetCardTypeText { get; private set; } = string.Empty;
    public string GetCardRarityText { get; private set; } = string.Empty;

    public CardRarity Rarity => attributes is { Count: > 0 } && attributes[(int)CardAttribute.RARITY] != null
        ? (CardRarity)(int)attributes[(int)CardAttribute.RARITY].ConcatValue(Level())
        : CardRarity.COMMON;

    internal string MaxLevelLabel = string.Empty;

    #endregion
}

[Serializable]
public class Components {
    // Images to paint Rarity
    [SerializeField] public List<Image> imagesToPaint;
    [SerializeField] public List<Image> imagesToPaintDisabled;

    // Game Objects
    [SerializeField] public Image cardIcon;
    [SerializeField] public GameObject cardBox;
    [SerializeField] public Shadow cardShadow;
    [SerializeField] public CanvasGroup cardCanvasGroup;
    [SerializeField] public TextMeshProUGUI cardTitleText;
    [SerializeField] public CardTouchHandler dragHandler;
    [SerializeField] public Button infoButton;

    #region Quantity

    [SerializeField] public GameObject boxQuantity;

    [SerializeField] public TextMeshProUGUI textQuantity;
    // [SerializeField] public Image boxQuantityProgress;
    // [SerializeField] public GameObject areaQuantity;
    // [SerializeField] public TextMeshProUGUI currentQuantity;

    #endregion

    #region LevelBox

    [SerializeField] public GameObject levelBox;
    [SerializeField] public TextMeshProUGUI levelText;
    [SerializeField] public TextMeshProUGUI levelCostText;
    [SerializeField] public Slider levelSlider;
    [SerializeField] public GameObject levelKnot;

    #endregion

    // /// <summary>
    // ///     OLD
    // /// </summary>
    //
    // [SerializeField] public Image boxCardIconImage;


    //
    // [FormerlySerializedAs("TotalQuantity")]
    // public TextMeshProUGUI totalQuantity;
    //

    //
    // [FormerlySerializedAs("CardDescrText")]
    // public TextMeshProUGUI cardDescriptionText;
    //
    //
    //
    //
    // [FormerlySerializedAs("BoxAbilitySlot")]
    // public GameObject boxAbilitySlot;
    //
    // [FormerlySerializedAs("IconAbilitySlotConsumable")]
    // public GameObject iconAbilitySlotConsumable;
    //
    // [FormerlySerializedAs("IconAbilitySlotTurnBased")]
    // public GameObject iconAbilitySlotTurnBased;
    //
    // [FormerlySerializedAs("IconAbilitySlotImprovement")]
    // public GameObject iconAbilitySlotImprovement;
    //
    // [FormerlySerializedAs("IconCardForNotFound")]
    // public Image iconCardForNotFound;
    //
    // [FormerlySerializedAs("BoxCardImage")] public Image boxCardImage;
    //
    //
    // [FormerlySerializedAs("BoxCardDescImage")]
    // public Image boxCardDescImage;
    //
    //
    // [SerializeField] public GameObject cardUpdateBox;
    // [SerializeField] public List<Image> cardUpdateBoxSlots = new();
    // [SerializeField] public List<Sprite> cardUpdateSprites = new();
    //
    // // All components to print
    // [SerializeField] public List<Image> imagesToPaint;
    // [SerializeField] public List<Image> imagesToPaintDisabled;
    //
    //
    //

    //
    // #region CardImages
    //
    // [SerializeField] public Image cardTypeIcon;
    // [SerializeField] public List<Sprite> cardTypeIcons;
    //
    // #endregion
    //
    // #region Gems
    //
    // [SerializeField] public GameObject gemsArea;
    // [SerializeField] public List<Image> gemsSlot;
    // [SerializeField] public List<Image> gemsSlotIcon;
    //
    // #endregion
    //
    // #region Other Components
    //
    // [SerializeField] public GameObject boxCard;
    // [SerializeField] public GameObject boxCardNotFound;
    //
    // #endregion
}

}