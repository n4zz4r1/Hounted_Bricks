using System;
using System.Collections.Generic;
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

    public Dictionary<Card, GameObject> CardPrefabDictionary { get; set; } = new();

    protected override void Before() {
        CardPrefabDictionary.Add(Card.Card_001_Crooked_Rock, Resources.Load("001_Crooked_Rock") as GameObject);
        CardPrefabDictionary.Add(Card.Card_002_Rounded_Rock, Resources.Load("002_Rounded_Rock") as GameObject);
        CardPrefabDictionary.Add(Card.Card_003_Arrowed_Rock, Resources.Load("003_Arrowed_Rock") as GameObject);
        CardPrefabDictionary.Add(Card.Card_004_Bomb_Rock, Resources.Load("004_Bomb_Rock") as GameObject);
        components.clearButton.onClick.AddListener(() => { State.Clear(FSM); });
        components.shuffleButton.onClick.AddListener(() => { State.Shuffle(FSM); });

        addResource.onClick.AddListener(AddResource);

        base.Before();
    }

    // TODO delete dev region

    #region DEV

    [SerializeField] public Button addResource;

    public void AddResource() {
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

    [FormerlySerializedAs("ClearButton")] [SerializeField]
    public Button clearButton;

    [FormerlySerializedAs("ClearButton")] [SerializeField]
    public Button shuffleButton;

    [FormerlySerializedAs("ClearButton")] [SerializeField]
    public Button recommendedButton;

    [FormerlySerializedAs("DragArea")] [SerializeField]
    public GameObject dragArea;
}

}