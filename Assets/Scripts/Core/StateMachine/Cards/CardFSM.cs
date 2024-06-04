using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Data;
using Core.Handler;
using Core.Sprites;
using Core.StateMachine.Abilities;
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
    protected override State<CardFSM> GetInitialState => States.Preload;

    public bool IsDragging { get; set; }
    public long CurrentQuantity { get; private set; }
    public GameObject DragCard { get; private set; }
    public CardSlotFSM CurrentSelectedSlot { get; set; }

    protected override void Before() {
        if (!dragEnabled) components.dragHandler.enabled = false;
        if (disableCollision) components.collider.enabled = false;
        components.originalIcon = components.cardIcon.sprite;
        if (components.bwIcon == null) 
            components.bwIcon = components.cardIcon.sprite;

        if (cardType == CardType.Rock) components.boxQuantity.gameObject.SetActive(true);
        PrepareAbilityConsumables();

        GetCardTitle = LocalizationUtils.LoadText(GetCardName() + ".Title");
        GetCardFullDetail = LocalizationUtils.LoadText(GetCardName() + ".Detail");
        GetCardTypeText = LocalizationUtils.LoadText("CardType." + cardType);
        GetCardRarityText = LocalizationUtils.LoadText("CardRarity." + Rarity);
        MaxLevelLabel = LocalizationUtils.LoadText("Label.MaxLevel");
        
        PaintCard(Rarity);
    }

    public string GetTitle() => LocalizationUtils.LoadText(GetCardName() + ".Title");

    private void PrepareAbilityConsumables() {
        if (cardType != CardType.Ability) return;

        components.boxConsumable.gameObject.SetActive(true);
        components.textConsumable.text = Attribute(CardAttributeType.Consumable).ToString();
    }

    protected override async Task BeforeAsync() {
        GetCardTitle = await LocalizationUtils.LoadTextAsync(GetCardName() + ".Title");
        await LocalizationUtils.LoadTextAsync(GetCardName() + ".Description");
        GetCardFullDetail = await LocalizationUtils.LoadTextAsync(GetCardName() + ".Detail");
        GetCardTypeText = await LocalizationUtils.LoadTextAsync("CardType." + cardType);
        GetCardRarityText = await LocalizationUtils.LoadTextAsync("CardRarity." + Rarity);
        MaxLevelLabel = await LocalizationUtils.LoadTextAsync("Label.MaxLevel");
        MaxQuantity = CardsDataV1.Instance.GetCardMaxQuantity(cardId);
    }
    
    public bool HasLevel => maxLevel > 1;

    public void PaintCard(CardRarity cardRarity) {
        var normal = RarityUtils.From(cardRarity).NormalColor;
        foreach (var image in components.imagesToPaint)
            image.color = normal;
        components.cardIcon.sprite = components.originalIcon;
    }

    private void PaintCardDisabled() {
        foreach (var image in components.imagesToPaintDisabled)
            image.color = Colors.DISABLED_WOOD;
        if (components.bwIcon)
            components.cardIcon.sprite = components.bwIcon;
        
    }

    // Makes a card without info, level, and so on...
    private void MakeSimpleCard() {
        components.infoButton.gameObject.SetActive(false);
    }
    
    // Makes a card draggable info, level, and so on...
    public void MakeDraggableCard(GameObject dragArea) {
        MakeSimpleCard();
        components.dragArea = dragArea;
        dragEnabled = true;
        components.dragHandler.enabled = true;
        SyncCardColors();
    }

    public void CreateClone(CardFSM prefab) {
        if (prefab == null) return;

        var position = Camera.main!.ScreenToWorldPoint(Input.mousePosition);
        position.x += 10f;

        DragCard = CreateInstance(prefab.gameObject, components.dragArea.transform);
        DragCard.GetComponent<CardFSM>().components.boxQuantity.SetActive(false);
        DragCard.transform.position = new Vector3(position.x, position.y, 10);
        DragCard.GetComponent<CardFSM>().MakeSimpleCard();
    }

    public void UpdateCurrentAvailable(long quantity) {
        // only allow disabled card if is draggable
        CurrentQuantity = quantity;

        // if (!dragEnabled)
        //     return;

        components.textQuantity.text = quantity.ToString();
        SyncCardColors();
    }

    public bool HasAvailableCards() {
        return CurrentQuantity > 0;
    }

    private void SyncCardColors() {
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
        Destroy(DragCard);
    }

    #region Properties

    [SerializeField] public Card cardId = Card.NONE;
    [SerializeField] public CardType cardType = CardType.None;
    [SerializeField] public int maxLevel = 1;
    [SerializeField] public bool dragEnabled;
    [SerializeField] public List<CardAttributesComponent> attributes;
    // Ability Ref
    [SerializeField] public bool disableCollision;
    [SerializeField] public AbilityFSM abilityFSM;

    [SerializeField] public Components components;

    public int MaxQuantity { get; private set; }

    
    public bool IsMaxLevel() {
        return CardsDataV1.Instance.GetCardLevel(cardId) == maxLevel;
    }

    public void SetDragArea(GameObject dragArea) => components.dragArea = dragArea;

    public int Level() {
        return maxLevel <= 1 ? 1 : CardsDataV1.Instance.GetCardLevel(cardId);
    }

    private string GetCardName() {
        return cardId.ToString().Replace("Card_", "");
    }

    internal bool HasGems() {
        return attributes[(int)CardAttributeType.GemSlot].updates.Count != 0;
    }

    public string GetCardTitle { get; private set; } = string.Empty;
    public string GetCardFullDetail { get; private set; } = string.Empty;
    public string GetCardTypeText { get; private set; } = string.Empty;
    public string GetCardRarityText { get; private set; } = string.Empty;

    public CardRarity Rarity => attributes is { Count: > 0 } && attributes[(int)CardAttributeType.Rarity] != null
        ? (CardRarity)(int)attributes[(int)CardAttributeType.Rarity].ConcatValue(Level())
        : CardRarity.COMMON;

    public int Attribute(CardAttributeType attributeType) => (int)attributes[(int)attributeType].ConcatValue(Level());
    
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
    [SerializeField] public Sprite originalIcon;
    [SerializeField] public Sprite bwIcon;

    [SerializeField] public GameObject cardBox;
    [SerializeField] public Shadow cardShadow;
    [SerializeField] public CanvasGroup cardCanvasGroup;
    [SerializeField] public TextMeshProUGUI cardTitleText;
    [SerializeField] public CardTouchHandler dragHandler;
    [SerializeField] public GameObject dragArea;
    [SerializeField] public Button infoButton;
    [SerializeField] public Collider2D collider;

    #region Quantity
    [SerializeField] public GameObject boxQuantity;
    [SerializeField] public TextMeshProUGUI textQuantity;
    #endregion

    #region Consumable
    [SerializeField] public GameObject boxConsumable;
    [SerializeField] public TextMeshProUGUI textConsumable;
    #endregion

    #region LevelBox

    [SerializeField] public GameObject levelBox;
    [SerializeField] public TextMeshProUGUI levelText;
    [SerializeField] public TextMeshProUGUI levelCostText;
    [SerializeField] public Slider levelSlider;
    [SerializeField] public GameObject levelKnot;

    #endregion
}

}