using System;
using System.Collections.Generic;
using Core.Data;
using Core.StateMachine.Rewards;
using Core.Utils;
using Core.Utils.Constants;
using Framework.Base;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Core.StateMachine.ChestAreas {

public class ChestAreaFSM : StateMachine<ChestAreaFSM, State<ChestAreaFSM>> {
    [FormerlySerializedAs("Internal")] [SerializeField]
    public Components components;

    [FormerlySerializedAs("Animator")] public Animator animator;
    protected override ChestAreaFSM FSM => this;

    protected override State<ChestAreaFSM> GetInitialState => States.Started;

    public List<GameObject> Rewards { get; set; } = new();

    protected override void Before() {
        components.buttonGetKeys.onClick.AddListener(() => State.Click(FSM));
        components.buttonOpenChest.onClick.AddListener(() => State.Click(FSM));

        components.buttonGetKeys.gameObject.SetActive(false);
        components.buttonOpenChest.gameObject.SetActive(false);
        animator = components.iconChest.GetComponent<Animator>();

        base.Before();
    }

    protected override void SyncDataBase() {
        if (!PlayerDataV1.Instance.HasMasterKey())
            for (var i = 0; i < 3; i++) {
                FSM.components.iconKeys[i].GetComponent<Image>().color =
                    i < ResourcesV1.Instance.GetResourcesAmount(ResourceType.CHEST_KEYS)
                        ? Colors.PRIMARY
                        : Colors.DISABLED;
                FSM.components.iconKeys[i].GetComponent<Shadow>().enabled =
                    i < ResourcesV1.Instance.GetResourcesAmount(ResourceType.CHEST_KEYS);
            }

        components.counterChest.text = ResourcesV1.Instance.GetResourcesAmount(ResourceType.CHEST).ToString();
    }
}

[Serializable]
public class Components {
    [FormerlySerializedAs("IconChest")] public GameObject iconChest;
    [FormerlySerializedAs("IconKeys")] public GameObject[] iconKeys;

    [FormerlySerializedAs("IconKeyFromButton")]
    public GameObject iconKeyFromButton;

    [FormerlySerializedAs("IconMasterKey")]
    public GameObject iconMasterKey;

    [FormerlySerializedAs("IconMasterKeyFromButton")]
    public GameObject iconMasterKeyFromButton;

    [FormerlySerializedAs("ButtonOpenChest")]
    public Button buttonOpenChest;

    [FormerlySerializedAs("ButtonGetKeys")]
    public Button buttonGetKeys;

    [FormerlySerializedAs("CounterChest")] public TextMeshProUGUI counterChest;

    [FormerlySerializedAs("AreaReward")] public GameObject areaReward;

    [FormerlySerializedAs("RewardFSMPrefab")]
    public RewardFSM rewardFSMPrefab;
}

}