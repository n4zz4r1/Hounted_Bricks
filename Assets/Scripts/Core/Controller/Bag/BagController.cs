using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Data;
using Core.Popup.CardDetail;
using Core.StateMachine.Cards;
using Core.StateMachine.CardSlots;
using Core.StateMachine.Resource;
using Core.Utils;
using Core.Utils.Constants;
using Framework.Base;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Core.Controller.Bag {

public class BagController : StateMachine<BagController, State<BagController>> {
    [FormerlySerializedAs("Components")] [SerializeField]
    public Components components;

    protected override BagController FSM => this;
    protected override State<BagController> GetInitialState => States.Started;
    public Dictionary<Card, CardFSM> CardPrefabDictionary { get; set; } = new();

    protected override async Task BeforeAsync() {
        // update icon, label and text based on its type
        CardPrefabDictionary.Add(Card.Card_001_Crooked_Rock,
            await AssetLoader<CardFSM, ResourceType>.LoadCardFSM(Card.Card_001_Crooked_Rock));
        CardPrefabDictionary.Add(Card.Card_002_Rounded_Rock,
            await AssetLoader<CardFSM, ResourceType>.LoadCardFSM(Card.Card_002_Rounded_Rock));
        CardPrefabDictionary.Add(Card.Card_003_Arrowed_Rock,
            await AssetLoader<CardFSM, ResourceType>.LoadCardFSM(Card.Card_003_Arrowed_Rock));
        CardPrefabDictionary.Add(Card.Card_004_Bomb_Rock,
            await AssetLoader<CardFSM, ResourceType>.LoadCardFSM(Card.Card_004_Bomb_Rock));
    }

    protected override void Before() {
        components.clearButton.onClick.AddListener(() => { State.Clear(FSM); });
        components.shuffleButton.onClick.AddListener(() => { State.Shuffle(FSM); });
        addResource.onClick.AddListener(AddResource);
    }

    // TODO delete dev region

    #region DEV

    [SerializeField] public Button addResource;

    private void AddResource() {
        ResourcesV1.Instance.AddResources(ResourceType.ROCK_SCROLL, 50);
        SyncAllData(typeof(CardFSM));
        SyncAllData(typeof(ResourceFSM));
        SyncAllData(typeof(CardDetailPopup));
    }

    #endregion
}

[Serializable]
public class Components {
    [FormerlySerializedAs("Slots")] [SerializeField]
    public List<CardSlotFSM> slots;

    [SerializeField] public Button clearButton;
    [SerializeField] public Button shuffleButton;
    [SerializeField] public GameObject dragArea;
}

}