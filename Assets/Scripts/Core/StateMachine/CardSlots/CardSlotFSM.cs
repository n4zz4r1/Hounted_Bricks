using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Data;
using Core.StateMachine.Cards;
using Core.Utils;
using Core.Utils.Constants;
using Framework.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Core.StateMachine.CardSlots {

public class CardSlotFSM : StateMachine<CardSlotFSM, State<CardSlotFSM>> {
    [SerializeField] public int index;
    [SerializeField] public Components components;

    protected override CardSlotFSM FSM => this;
    protected override State<CardSlotFSM> GetInitialState => States.Preload;

    public Sprite TemporaryIconSprite { get; set; }
    public CardFSM TemporaryCard { get; set; }
    public Sprite OriginalIconSprite { get; set; }
    public CardFSM SelectedCardFSM { get; set; }
    public CardFSM CurrentCard { get; private set; }
    public Dictionary<Card, CardFSM> CardPrefabDictionary { get; set; } = new();

    protected override async Task BeforeAsync() {
        // update icon, label and text based on its type
        CardPrefabDictionary.Add(Card.Card_001_Crooked_Rock,
            await AssetLoader<Card>.Load<CardFSM>(Card.Card_001_Crooked_Rock));
        CardPrefabDictionary.Add(Card.Card_002_Rounded_Rock,
            await AssetLoader<Card>.Load<CardFSM>(Card.Card_002_Rounded_Rock));
        CardPrefabDictionary.Add(Card.Card_003_Arrowed_Rock,
            await AssetLoader<Card>.Load<CardFSM>(Card.Card_003_Arrowed_Rock));
        CardPrefabDictionary.Add(Card.Card_004_Bomb_Rock,
            await AssetLoader<Card>.Load<CardFSM>(Card.Card_004_Bomb_Rock));
    }

    internal void Sync() {
        SyncDataBase();
    }

    protected override void SyncDataBase() {
        var selectedCard = PlayerDataV1.Instance.saveRockSlot[index];
        var totalOfRocks = CardsDataV1.Instance.GetTotalRocks();

        if ((index != 0 && PlayerDataV1.Instance.saveRockSlot[index - 1] is Card.NONE) || index == totalOfRocks) {
            ChangeState(States.Disabled);
        }
        else if (PlayerDataV1.Instance.saveRockSlot[index] is Card.NONE) {
            ChangeState(States.Empty);
        }
        else {
            ChangeState(States.WithRock);
            CurrentCard = CardPrefabDictionary[selectedCard];
            State.SetCard(FSM);
        }
    }
}

[Serializable]
public class Components {
    [SerializeField] public BoxCollider2D slotCollider;
    [SerializeField] public Image slotBox;
    [SerializeField] public Image slotIcon;
    [SerializeField] public GameObject backgroundFilledInBox;
    [SerializeField] public GameObject glow;
    [SerializeField] public List<Image> paths;
}

}