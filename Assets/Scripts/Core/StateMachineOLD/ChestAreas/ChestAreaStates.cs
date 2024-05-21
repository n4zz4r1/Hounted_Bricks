using Core.Data;
using Core.StateMachine.Menu;
using Core.StateMachine.Rewards;
using Core.Utils;
using Core.Utils.Constants;
using Framework.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Core.StateMachine.ChestAreas {

public abstract class States {
    public static readonly Started Started = new();
    public static readonly ChestWithKey ChestWithKey = new();
    public static readonly ChestWithoutKey ChestWithoutKey = new();
    public static readonly NoChests NoChests = new();
}

public class Started : State<ChestAreaFSM> {
    public override void Enter(ChestAreaFSM fsm) {
        fsm.components.iconChest.GetComponent<Image>().color = Colors.ENABLED;
        fsm.components.iconChest.GetComponent<Shadow>().enabled = true;
        fsm.components.counterChest.color = Colors.ENABLED;

        // Check master key
        if (PlayerDataV1.Instance.HasMasterKey()) {
            fsm.components.iconKeys[0].SetActive(false);
            fsm.components.iconKeys[1].SetActive(false);
            fsm.components.iconKeys[2].SetActive(false);
            fsm.components.iconKeyFromButton.SetActive(false);

            fsm.components.iconMasterKey.SetActive(true);
            fsm.components.iconMasterKeyFromButton.SetActive(true);

            fsm.ChangeState(States.ChestWithKey);
        }
        else if (ResourcesV1.Instance.HasResource(ResourceType.CHEST_KEYS)) {
            fsm.SyncAllData(typeof(ChestAreaFSM));
            fsm.ChangeState(States.ChestWithKey);
        }
        else {
            fsm.SyncAllData(typeof(ChestAreaFSM));
            fsm.ChangeState(States.ChestWithoutKey);
        }
    }
}

public class ChestWithKey : State<ChestAreaFSM> {
    private static readonly int Open = Animator.StringToHash("open");

    public override void Enter(ChestAreaFSM fsm) {
        if (!ResourcesV1.Instance.HasResource(ResourceType.CHEST_KEYS)) {
            fsm.ChangeState(States.NoChests);
            return;
        }

        fsm.components.buttonOpenChest.gameObject.SetActive(true);
        fsm.components.buttonGetKeys.gameObject.SetActive(false);
    }

    public override void Click(ChestAreaFSM fsm) {
        fsm.components.buttonOpenChest.enabled = false;

        if (ResourcesV1.Instance.OpenChest()) {
            GenerateReward(fsm);

            fsm.animator.SetTrigger(Open);

            fsm.SyncAllData(typeof(MenuFSM));

            if (!ResourcesV1.Instance.HasResource(ResourceType.CHEST))
                fsm.ChangeState(States.NoChests);
            else if (!ResourcesV1.Instance.HasResource(ResourceType.CHEST_KEYS))
                fsm.ChangeState(States.ChestWithoutKey);
        }

        fsm.components.buttonOpenChest.enabled = true;
    }

    private void GenerateReward(ChestAreaFSM fsm) {
        Debug.Log("<color=green> ----- Rolling Reward Dices!! ------ </color>");

        // 1. remove current rewards
        fsm.Rewards.ForEach(Object.Destroy);
        _ = fsm.Rewards.RemoveAll(_ => true);

        // 2. generate new rewardFSM instances

        var lowRewardResult = LowRewardDice.Roll();
        var xPositionFirstReward = -70;
        var xPositionSecondReward = -0;
        if (lowRewardResult is ResourceType.NONE) {
            xPositionFirstReward = -35;
            xPositionSecondReward = 35;
        }
        else {
            GenerateInstance(fsm, 70, lowRewardResult);
        }

        GenerateInstance(fsm, xPositionFirstReward, NormalRewardDice.Roll());
        GenerateInstance(fsm, xPositionSecondReward, NormalRewardDice.Roll());

        // var rewardTwo = FSM.CreateInstance(FSM.components.rewardFSMPrefab.gameObject, FSM.components.areaReward.transform);
        // rewardTwo.transform.localPosition = new Vector3(xPositionSecondReward, 0, 0);
        // var rewardFSMInstance2 = rewardTwo.GetComponent<RewardFSM>();
        // rewardFSMInstance2.State.RollTheDice(rewardFSMInstance2, NormalRewardDice.Roll());
        // rewardFSMInstance2.State.Earn(rewardFSMInstance2);
        //
        // rewardTwo.GetComponent<RewardFSM>().SetReward(NormalRewardDice.Roll());
        // FSM.Rewards.Add(rewardTwo);
        //
        // // var rewardThree = FSM.CreateInstance(FSM.components.rewardFSMPrefab.gameObject, FSM.components.areaReward.transform);
        // rewardThree.transform.localPosition = new Vector3(70, 0, 0);
        // rewardThree.GetComponent<RewardFSM>().SetReward(lowRewardResult);
        // FSM.Rewards.Add(rewardThree);
    }

    private static void GenerateInstance(ChestAreaFSM fsm, float xPositionFirstReward, ResourceType type) {
        var rewardOne = fsm.CreateInstance(fsm.components.rewardFSMPrefab.gameObject,
            fsm.components.areaReward.transform);
        rewardOne.transform.localPosition = new Vector3(xPositionFirstReward, 0, 0);
        var rewardFSMInstance = rewardOne.GetComponent<RewardFSM>();
        rewardFSMInstance.RollTheDice(NormalRewardDice.Roll());
        rewardFSMInstance.State.Earn(rewardFSMInstance);
        fsm.Rewards.Add(rewardOne);
    }
}

public class ChestWithoutKey : State<ChestAreaFSM> {
    public override void Enter(ChestAreaFSM fsm) {
        if (!ResourcesV1.Instance.HasResource(ResourceType.CHEST)) {
            fsm.ChangeState(States.NoChests);
            return;
        }

        fsm.components.buttonOpenChest.gameObject.SetActive(false);
        fsm.components.buttonGetKeys.gameObject.SetActive(true);
    }

    // TODO Ads Reward here!
    public override void Click(ChestAreaFSM fsm) {
        ResourcesV1.Instance.AddResources(ResourceType.CHEST_KEYS, 3);

        fsm.SyncAllData(typeof(ChestAreaFSM));
        if (!ResourcesV1.Instance.HasResource(ResourceType.CHEST))
            fsm.ChangeState(States.NoChests);
        else
            fsm.ChangeState(States.ChestWithKey);
    }
}

// FINAL STATE
public class NoChests : State<ChestAreaFSM> {
    public override void Enter(ChestAreaFSM fsm) {
        fsm.components.buttonOpenChest.gameObject.SetActive(false);
        fsm.components.buttonGetKeys.gameObject.SetActive(false);

        fsm.components.iconChest.GetComponent<Image>().color = Colors.DISABLED;
        fsm.components.iconChest.GetComponent<Shadow>().enabled = false;
        fsm.components.counterChest.color = Colors.DISABLED;
    }
}

}