using Core.Data;
using Core.StateMachine.Cards;
using Core.Utils;
using Core.Utils.Constants;

namespace Core.StateMachine.CardSlots {
public class CardAbilitySlotFSM: CardSlotFSM {
    protected override void PersistCard() {
        
    }

    protected override void SyncSlots() {
        if (PlayerCard == Card.NONE) return;

        var selectedCard = CardsDataV1.Instance.GetPlayerAbilityAtPosition(this);
        if (index != 0 && CardsDataV1.Instance.GetPlayerAbilityAtPosition(PlayerCard, index - 1) is Card.NONE)
            ChangeState(States.Disabled);
        else if (selectedCard is Card.NONE)
            ChangeState(States.Empty);
        else {
            ChangeState(States.WithCard);
            CurrentCard = AssetLoader.AsComponent<CardFSM>(selectedCard);
            State.SetCard(FSM);
        }
    }
    
    public void SetPlayer(Card player) {
        PlayerCard = player;
        SyncDataBase();
    } 
}


}