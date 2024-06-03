using Core.Controller.Bag;
using Core.Data;
using Core.StateMachine.CardSlots;
using Core.Utils;
using Core.Utils.Constants;
using Framework.Base;
using UnityEngine;

namespace Core.StateMachine.Cards {

public abstract class States {
    public static readonly Preload Preload = new();
    public static readonly NotFound NotFound = new();
    public static readonly Found Found = new();
}

public class Preload : State<CardFSM> {
    public override void Before(CardFSM fsm) {
        fsm.components.cardTitleText.text = fsm.GetCardTitle;
        fsm.SyncData(typeof(CardFSM));
        fsm.PaintCard(fsm.Rarity);

        if (!CardsDataV1.Instance.cards.Contains(fsm.cardId)) {
            fsm.ChangeState(States.NotFound);
            return;
        }

        fsm.ChangeState(States.Found);
    }
}

public class NotFound : State<CardFSM> {
    public override void Enter(CardFSM fsm) {
        fsm.components.cardBox.SetActive(false);
    }
}

public class Found : State<CardFSM> {
    public override void Hide(CardFSM fsm) {
        fsm.components.cardCanvasGroup.alpha = 0f;
    }

    public override void Show(CardFSM fsm) {
        fsm.components.cardCanvasGroup.alpha = 1f;
    }

    public override void Enter(CardFSM fsm) {
        if (fsm.maxLevel <= 1)
            fsm.components.levelBox.SetActive(false);

        if (fsm.MaxQuantity <= 1)
            fsm.components.boxQuantity.SetActive(false);

        SyncData(fsm);
    }

    public override void SyncData(CardFSM fsm) {
        var currentLevel = fsm.Level();
        if (fsm.maxLevel > 1) {
            fsm.components.levelText.text = currentLevel.ToString();
            if (fsm.IsMaxLevel()) {
                // fill with max level
                fsm.components.levelCostText.text = fsm.MaxLevelLabel;
                fsm.components.levelSlider.maxValue = 1;
                fsm.components.levelSlider.value = 1;
            }
            else {
                var coinsNextLevel = GameMathUtils.GenerateUpdateCostByLevel(currentLevel + 1);
                var totalOfUpgradeRes = ResourcesV1.Instance.GetResourcesAmount(fsm.cardType);

                fsm.components.levelCostText.text = totalOfUpgradeRes + " / " + coinsNextLevel;
                fsm.components.levelSlider.maxValue = coinsNextLevel;
                if (totalOfUpgradeRes >= coinsNextLevel) {
                    fsm.components.levelSlider.value = coinsNextLevel;
                    fsm.components.levelKnot.SetActive(true);
                }
                else {
                    fsm.components.levelSlider.value = totalOfUpgradeRes;
                    fsm.components.levelKnot.SetActive(false);
                }
            }
        }
        else {
            fsm.components.levelBox.SetActive(false);
        }

        // Set Total and current on save
        var cardQuantity = CardsDataV1.Instance.GetCardQuantity(fsm.cardId);
        fsm.components.textQuantity.text = cardQuantity.ToString();
        fsm.UpdateCurrentAvailable(cardQuantity - PlayerDataV1.Instance.RockCardCounter(fsm.cardId) - CardsDataV1.Instance.AbilityUsed(fsm.cardId));
    }

    //
    // // Collision check session
    // TODO Refactor here, make slot responsible for persist slots
    //
    public override void OnCollisionEnter(CardFSM fsm, Collider2D collider) {
        var slotCollider = collider.GetComponent<CardSlotFSM>();

        if (slotCollider == null || slotCollider.State == CardSlots.States.Disabled ||
            (slotCollider.State is WithCard && slotCollider.SelectedCardFSM?.cardId == fsm.cardId))
            return;

        fsm.CurrentSelectedSlot = slotCollider;
        slotCollider.TemporaryIconSprite = fsm.components.cardIcon.sprite;
        slotCollider.TemporaryCard = fsm;
        slotCollider.ChangeState(CardSlots.States.OnHover);
    }

    public override void OnCollisionExit(CardFSM fsm, Collider2D collider) {
        fsm.CurrentSelectedSlot = null;

        if (collider.GetComponent<CardRockSlotFSM>() != null) 
            CollidedWithSlot(fsm, collider.GetComponent<CardRockSlotFSM>());
        else if (collider.GetComponent<CardAbilitySlotFSM>() != null) 
            CollidedWithSlot(fsm, collider.GetComponent<CardAbilitySlotFSM>());

    }

    private static void CollidedWithSlot<T>(CardFSM fsm, T slotCollider) where T : CardSlotFSM {
        if (slotCollider.State == CardSlots.States.Disabled ||
            (slotCollider.State is WithCard && slotCollider.SelectedCardFSM?.cardId == fsm.cardId))
            return;

        if (slotCollider.type == CardSlotType.Rock && PlayerDataV1.Instance.saveRockSlot[slotCollider.index] != Card.NONE)
            slotCollider.ChangeState(CardSlots.States.WithCard);
        else if (slotCollider.type == CardSlotType.Ability && CardsDataV1.Instance.GetPlayerAbilityAtPosition(slotCollider) != Card.NONE)
            slotCollider.ChangeState(CardSlots.States.WithCard);
        else
            slotCollider.ChangeState(CardSlots.States.Empty);

        slotCollider.Sync();
        
    }

    public override void StartDragging(CardFSM fsm) {
        fsm.UpdateCurrentAvailable(fsm.CurrentQuantity - 1);
        AssetLoader<Card>.Load<CardFSM, CardFSM>(fsm.cardId, fsm, CloneCard);
        fsm.IsDragging = true;
    }

    private static void CloneCard(CardFSM cardFSMPrefab, CardFSM fsm) {
        fsm.CreateClone(cardFSMPrefab);
    }

    public override void Update(CardFSM fsm) {
        if (!fsm.IsDragging || Camera.main == null) return;
        var whereTo = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        fsm.DragCard.transform.position = new Vector3(whereTo.x, whereTo.y + 1f, 10);
    }

    public override void StopDragging(CardFSM fsm) {
        var slot = fsm.DragCard.GetComponent<CardFSM>().CurrentSelectedSlot;
        if (slot != null) {

            if (slot.type == CardSlotType.Rock)
                PlayerDataV1.Instance.ChangeRockSlot(slot.index, fsm.cardId);
            else {
                // Changing abilities
                CardsDataV1.Instance.ChangeSavedAbility(slot.PlayerCard, slot.index, fsm.cardId);
            }
            
        } else
            fsm.UpdateCurrentAvailable(fsm.CurrentQuantity + 1);

        fsm.CurrentSelectedSlot = null;
        
        fsm.DestroyDragCard();
        fsm.IsDragging = false;

        fsm.SyncAllData(typeof(CardAbilitySlotFSM));
        fsm.SyncAllData(typeof(CardRockSlotFSM));
        fsm.SyncAllData(typeof(CardFSM));
        fsm.SyncAllData(typeof(BagController));
    }
}

}