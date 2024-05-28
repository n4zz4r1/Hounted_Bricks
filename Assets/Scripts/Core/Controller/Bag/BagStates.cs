using Core.Data;
using Core.StateMachine.Cards;
using Core.StateMachine.CardSlots;
using Core.Utils;
using Framework.Base;

namespace Core.Controller.Bag {

public abstract class States {
    public static readonly Preloading Preloading = new();
    public static readonly Started Started = new();
}

public class Preloading : State<BagController> {
    public override void Before(BagController fsm) { }
}


public class Started : State<BagController> {
    public override void Clear(BagController fsm) {
        PlayerDataV1.Instance.ClearSaveRockSlots();
        fsm.SyncAllData(typeof(CardSlotFSM));
        fsm.SyncAllData(typeof(CardFSM));
        fsm.SyncAllData(typeof(BagController));
    }

    public override void Shuffle(BagController fsm) {
        var rockCards = CardsDataV1.Instance.GetAllRocksAvailable();
        var shuffledIndex = Dices.GenerateRandomList(rockCards.Count - 1);
        for (var i = 0; i < rockCards.Count; i++) {
            if (i >= fsm.components.slots.Count)
                break;

            PlayerDataV1.Instance.ChangeRockSlot(i, rockCards[shuffledIndex[i]]);
        }

        fsm.SyncAllData(typeof(CardSlotFSM));
        fsm.SyncAllData(typeof(CardFSM));
        fsm.SyncAllData(typeof(BagController));
    }

    public override void Before(BagController fsm) {
        SyncData(fsm);
    }

    public override void SyncData(BagController fsm) {
        var totalOfRocks = CardsDataV1.Instance.GetTotalRocks();
        for (var i = 0; i < 30; i++) {
            // TODO
        }
    }
}

}