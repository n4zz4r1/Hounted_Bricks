using Core.Data;
using Core.StateMachine.Cards;
using Core.Utils.Constants;
using UnityEngine;

namespace Core.StateMachine.CardSlots {
public class CardRockSlotFSM: CardSlotFSM {

    protected override void ClearSlot() {
        if (State == States.WithCard && PlayerDataV1.Instance.saveRockSlot[1] != Card.NONE) {
            PlayerDataV1.Instance.RemoveFromSlot(index);
            components.animator.SetTrigger(RemoveAnim);
            SyncAllData(typeof(CardRockSlotFSM));
            SyncAllData(typeof(CardFSM));
        }
    }
    
    protected override void SyncSlots() {
        var selectedCard = PlayerDataV1.Instance.saveRockSlot[index];
        var totalOfRocks = CardsDataV1.Instance.GetTotalRocks();

        if ((index != 0 && PlayerDataV1.Instance.saveRockSlot[index - 1] is Card.NONE) || index == totalOfRocks)
            ChangeState(States.Disabled);
        else if (PlayerDataV1.Instance.saveRockSlot[index] is Card.NONE) 
            ChangeState(States.Empty);
        else {
            ChangeState(States.WithCard);
            CurrentCard = CardPrefabDictionary[selectedCard];
            State.SetCard(FSM);
        }
    }
}


}