using Core.Utils.Constants;
using DG.Tweening;
using Framework.Base;

namespace Core.StateMachine.CardSlots {

public abstract class States {
    public static readonly Preload Preload = new();
    public static readonly Empty Empty = new();
    public static readonly WithRock WithRock = new();
    public static readonly Disabled Disabled = new();
    public static readonly OnHover OnHover = new();
}

public class Preload : State<CardSlotFSM> {
    public override void Before(CardSlotFSM fsm) {
        fsm.Sync();
    }
}

public class Empty : State<CardSlotFSM> {

    public override void Enter(CardSlotFSM fsm) {
        fsm.components.slotBox.color = Colors.PRIMARY;
        fsm.components.slotIcon.enabled = false;
        fsm.components.backgroundFilledInBox.SetActive(false);
    }
}

public class WithRock : State<CardSlotFSM> {
    public override void Enter(CardSlotFSM fsm) {
        fsm.components.slotIcon.sprite = fsm.OriginalIconSprite;
        fsm.components.slotIcon.enabled = true;
        fsm.components.backgroundFilledInBox.SetActive(true);
    }

    public override void SetCard(CardSlotFSM fsm) {
        fsm.OriginalIconSprite =
            fsm.CardPrefabDictionary[fsm.CurrentCard.cardId].components.cardIcon.sprite;
        fsm.components.slotIcon.sprite =
            fsm.CardPrefabDictionary[fsm.CurrentCard.cardId].components.cardIcon.sprite;
        fsm.SelectedCardFSM = fsm.CurrentCard;
        fsm.components.slotBox.color = RarityUtils.From(fsm.CurrentCard.Rarity).NormalColor;
    }
}

public class OnHover : State<CardSlotFSM> {
    public override void Enter(CardSlotFSM fsm) {
        fsm.components.slotIcon.sprite = fsm.TemporaryIconSprite;
        fsm.components.slotIcon.enabled = true;
        fsm.components.slotBox.color = RarityUtils.From(fsm.TemporaryCard.Rarity).NormalColor;
        fsm.TemporaryCard.State.Hide(fsm.TemporaryCard);
        fsm.components.slotBox.transform.DOScale(1.5f, 0.25f).SetEase(Ease.OutQuad);
    }

    public override void Exit(CardSlotFSM fsm) {
        fsm.components.slotIcon.sprite = fsm.OriginalIconSprite;
        fsm.components.slotIcon.enabled = false;
        fsm.TemporaryIconSprite = null;
        if (fsm.SelectedCardFSM != null && fsm.SelectedCardFSM.cardId != Card.NONE)
            fsm.components.slotBox.color = RarityUtils.From(fsm.SelectedCardFSM.Rarity).NormalColor;

        fsm.components.slotBox.transform.DOScale(1f, 0.25f).SetEase(Ease.OutQuad);
        fsm.TemporaryCard.State.Show(fsm.TemporaryCard);
    }
}

public class Disabled : State<CardSlotFSM> {
    public override void Enter(CardSlotFSM fsm) {
        fsm.components.slotBox.color = Colors.DISABLED_ALPHA;
        fsm.components.slotIcon.enabled = false;
        fsm.components.backgroundFilledInBox.SetActive(false);
    }
}

}