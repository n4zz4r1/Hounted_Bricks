
using Core.Data;

using Framework.Base;


namespace Core.StateMachine.Cards {

public abstract class States {
    public static readonly Created Created = new();
    public static readonly NotFound NotFound = new();
    public static readonly Found Found = new();
}

public class Created : State<CardFSM> {
    public override void Before(CardFSM fsm) {
        fsm.components.cardTitleText.text = fsm.GetCardTitle;
        // TODO Type Icon
        // fsm.components.cardTypeIcon.sprite = fsm.components.cardTypeIcons[(int)fsm.cardType];
        fsm.SyncData(typeof(CardFSM));
    }
    //

    public override void Enter(CardFSM fsm) {
        // If card quantity > 0, set State to FOUND, else NOT_FOUND
        fsm.PaintCard(fsm.Rarity);
    
        // if (fsm.createAsDisable || !CardsDataV1.Instance.cards.Contains(fsm.cardId)) { TODO check create as disable
        if (!CardsDataV1.Instance.cards.Contains(fsm.cardId)) {
            fsm.ChangeState(States.NotFound);
            return;
        }
        fsm.ChangeState(States.Found);

        // var quantity = CardsDataV1.Instance.GetCardQuantity(fsm.cardId);
        float quantity = CardsDataV1.Instance.GetCardQuantity(fsm.cardId);
        fsm.components.textQuantity.text = quantity.ToString();
    
        //
        // // Show card quantity if needed
        // if (quantity > 1) {
        //     fsm.components.boxQuantity.SetActive(true);
        //     // float maxQuantity = fsm.maxQuantity;
        //     // fsm.components.textQuantity.text = quantity + " / " + maxQuantity;
        //     // fsm.components.boxQuantityProgress.rectTransform.anchorMax = new Vector2(quantity / maxQuantity, 1);
        // }
        // else {
        //     fsm.components.boxQuantity.SetActive(false);
        // }
    

    }
}

public class NotFound : State<CardFSM> {
    public override void Enter(CardFSM fsm) {
        // fsm.DestroyCardBox();
        fsm.components.cardBox.SetActive(false);
        // fsm.components.iconCardForNotFound.sprite = fsm.components.cardIcon.sprite;
        // fsm.components.areaQuantity.SetActive(false);
    
        // fsm.DestroyInfoButton();
        // fsm.components.cardTitleText.transform.localPosition = new Vector3(0, -1, 0);
    }
}

public class Found : State<CardFSM> {
    // // TODO improve performance by removing Resources.Load
    // public override void OpenPopup(CardFSM fsm) {
    //     CardDetailPopup.Create(fsm, fsm.transform.root);
    // }
    //
    // public override void Hide(CardFSM fsm) {
    //     fsm.components.cardCanvasGroup.alpha = 0f;
    // }
    //
    // public override void Show(CardFSM fsm) {
    //     fsm.components.cardCanvasGroup.alpha = 1f;
    // }
    //
    public override void Enter(CardFSM fsm) {

        // Hide information if needed
        // if (fsm.hideInfo) {
            // fsm.DestroyInfoButton();
            // fsm.components.cardTitleText.transform.localPosition = new Vector3(0, -1, 0);
        // }
    
        if (fsm.maxLevel <= 1)
            fsm.components.levelBox.SetActive(false);

        if (fsm.MaxQuantity <= 1)
            fsm.components.boxQuantity.SetActive(false);

        SyncData(fsm);
    }
    //
    public override void SyncData(CardFSM fsm) {
    //     var currentLevel = fsm.Level();
    //     if (fsm.maxLevel > 1) {
    //         fsm.components.levelText.text = currentLevel.ToString();
    //         if (fsm.IsMaxLevel()) {
    //             // fill with max level
    //             fsm.components.levelCostText.text = LocalizationUtils.GetLocalizedText("Label.MaxLevel");
    //             fsm.components.levelSlider.maxValue = 1;
    //             fsm.components.levelSlider.value = 1;
    //         }
    //         else {
    //             var coinsNextLevel = GameMathUtils.GenerateUpdateCostByLevel(currentLevel + 1);
    //             var totalOfUpgradeRes = ResourcesV1.Instance.GetResourcesAmount(fsm.cardType);
    //
    //             fsm.components.levelCostText.text = totalOfUpgradeRes + " / " + coinsNextLevel;
    //             fsm.components.levelSlider.maxValue = coinsNextLevel;
    //             if (totalOfUpgradeRes >= coinsNextLevel) {
    //                 fsm.components.levelSlider.value = coinsNextLevel;
    //                 fsm.components.levelKnot.SetActive(true);
    //             }
    //             else {
    //                 fsm.components.levelSlider.value = totalOfUpgradeRes;
    //                 fsm.components.levelKnot.SetActive(false);
    //             }
    //         }
    //     }
    //     else {
    //         fsm.components.levelBox.SetActive(false);
    //     }
    //
    //     // Set Total and current on save
        var cardQuantity = CardsDataV1.Instance.GetCardQuantity(fsm.cardId);
        fsm.components.textQuantity.text = cardQuantity.ToString();
        fsm.UpdateCurrentAvailable(cardQuantity - PlayerDataV1.Instance.RockCardCounter(fsm.cardId));
    
    //     // Update Colors
    //     fsm.SyncCardColors();
    //
    //     // Gems Area 
    //     if (!fsm.HasGems()) fsm.components.gemsArea.SetActive(false);
    //     // TODO add gems here
    }
    //
    // // Collision check session
    //
    // public override void OnCollisionEnter(CardFSM fsm, Collider2D collider) {
    //     var slotCollider = collider.GetComponent<CardSlotFSM>();
    //     // ReSharper disable once Unity.NoNullPropagation
    //     if (slotCollider == null || slotCollider.State == CardSlots.States.Disabled ||
    //         (slotCollider.State is WithRock && slotCollider.SelectedCardFSM?.cardId == fsm.cardId))
    //         return;
    //
    //     fsm.SelectedSlot = slotCollider;
    //     fsm.SelectedSlot.TemporaryIconSprite = fsm.components.cardIcon.sprite;
    //     fsm.SelectedSlot.TemporaryCard = fsm;
    //     fsm.SelectedSlot.ChangeState(CardSlots.States.OnHover);
    // }
    //
    // public override void OnCollisionExit(CardFSM fsm, Collider2D collider) {
    //     var slotCollider = collider.GetComponent<CardSlotFSM>();
    //     // ReSharper disable once Unity.NoNullPropagation
    //     if (slotCollider.State == CardSlots.States.Disabled ||
    //         (slotCollider.State is WithRock && slotCollider.SelectedCardFSM?.cardId == fsm.cardId))
    //         return;
    //
    //     if (PlayerDataV1.Instance.saveRockSlot[fsm.SelectedSlot.Index] != Card.NONE)
    //         fsm.SelectedSlot.ChangeState(CardSlots.States.WithRock);
    //     else
    //         fsm.SelectedSlot.ChangeState(CardSlots.States.Empty);
    //
    //     fsm.SelectedSlot = null;
    // }
    //
    // // Dragging cards Session
    //
    // public override void StartDragging(CardFSM fsm) {
    //     fsm.UpdateCurrentAvailable(fsm.CurrentQuantity - 1);
    //     fsm.CreateClone();
    //
    //     fsm.IsDragging = true;
    // }
    //
    // public override void Update(CardFSM fsm) {
    //     if (!fsm.IsDragging || Camera.main == null) return;
    //     var whereTo = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //     fsm.DragCard.transform.position = new Vector3(whereTo.x, whereTo.y, 10);
    // }
    //
    //
    // public override void StopDragging(CardFSM fsm) {
    //     if (fsm.DragCard.GetComponent<CardFSM>().SelectedSlot != null)
    //         PlayerDataV1.Instance.ChangeRockSlot(fsm.DragCard.GetComponent<CardFSM>().SelectedSlot.Index,
    //             fsm.cardId);
    //     else
    //         fsm.UpdateCurrentAvailable(fsm.CurrentQuantity + 1);
    //
    //     fsm.DestroyDragCard();
    //     fsm.IsDragging = false;
    //
    //     fsm.SyncAllData(typeof(CardFSM));
    //     fsm.SyncAllData(typeof(BagController));
    // }
}

}