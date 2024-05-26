using System;
using System.Linq;
using Core.StateMachine.Rewards;
using Core.StateMachine.Stages;
using Framework.Base;
using UnityEngine;

// using Core.StateMachine.Stages;

namespace Core.Data {

[Serializable]
public class GameDataV1 : Data<GameDataV1> {
    public StageStatus GetStageStatus(int currentLevel) {
        return stages[currentLevel];
    }

    public string GetNumberOfStarsAsString() {
        var total = 0;
        total += stages.Sum(stage => (int)stage);

        return total + " / " + stages.Length * 3;
    }

    public void SetStageStatus(int currentLevel, StageStatus status) {
        stages[currentLevel] = status;
        Save();
    }

    public void CompleteStage(StageFSM stageFSM, int stars) {
        if (!stageFSM.isMapStage) {
            level += 1;
            Save();
            return;
        }

        if (stars <= (int)stageFSM.CurrentStageStatus)
            return;

        for (var i = (int)stageFSM.CurrentStageStatus + 1; i <= stars; i++)
            RewardFSM.SaveReward(stageFSM.rewards[i - 1], stageFSM.rewardsQuantity[i - 1], stageFSM.cardReward);

        SetStageStatus(stageFSM.Level, (StageStatus)stars);
    }

    #region Properties

    [SerializeField] public double level = 1;
    [SerializeField] public StageStatus[] stages = Enumerable.Repeat(StageStatus.NOT_DONE, 114).ToArray();

    #endregion
}

public enum StageStatus {
    NOT_DONE = 0,
    DONE_ONE_STAR = 1,
    DONE_TWO_STAR = 2,
    DONE_THREE_STAR = 3
}

}