using Core.Data;
using Core.Utils.Constants;
using Framework.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Core.StateMachine.Stages {

public abstract class States {
    public static readonly Created Created = new();
    public static readonly MainGameStage MainGameStage = new();
    public static readonly Unavailable Unavailable = new();
    public static readonly Available Available = new();
    public static readonly Done Done = new();
}

public class Created : State<StageFSM> {
    public override void Before(StageFSM fsm) {
        if (!fsm.isMapStage) {
            fsm.ChangeState(States.MainGameStage);
            return;
        }

        fsm.CurrentStageStatus = GameDataV1.Instance.stages[fsm.Level];
        // Debug.Log("Stage " + FSM.Level + " initiated with " + FSM.CurrentStageStatus);

        fsm.components.levelLabel.text = "";
        fsm.components.boxShadow.enabled = false;
        fsm.components.boxImage.color = DifficultyUtil.From(fsm.difficulty).color;
        fsm.components.difficultyLabel.text = DifficultyUtil.From(fsm.difficulty).label;
        fsm.components.canvasGroup.alpha = .0f;
        foreach (var star in fsm.components.stars) star.GetComponent<Shadow>().enabled = false;
    }

    public override void Enter(StageFSM fsm) {
        // If is the first stage, set to Available
        if (fsm.Level == 0) {
            if (fsm.CurrentStageStatus == StageStatus.DONE_THREE_STAR)
                fsm.ChangeState(States.Done);
            else
                fsm.ChangeState(States.Available);
        }
    }
}

public class MainGameStage : State<StageFSM> { }

public class Unavailable : State<StageFSM> {
    public override void Enter(StageFSM fsm) {
        fsm.components.canvasGroup.alpha = .0f;

        // refresh state from each child
        fsm.nextStages?.ForEach(stage => stage.RefreshStageStatus(fsm));
    }
}

public class Available : State<StageFSM> {
    public override void Enter(StageFSM fsm) {
        // Debug.Log("Stage " + FSM.Level + " AVAILABLE");
        fsm.components.levelLabel.text = fsm.Level.ToString();
        fsm.components.canvasGroup.interactable = true;
        fsm.components.canvasGroup.blocksRaycasts = true;
        fsm.components.boxShadow.enabled = true;
        fsm.components.boxImage.color = DifficultyUtil.From(fsm.difficulty).color;
        fsm.components.canvasGroup.alpha = 1f;

        fsm.UpdateStars();
        fsm.TurnLightsOn();

        // refresh state from each child
        fsm.nextStages?.ForEach(stage => stage.RefreshStageStatus(fsm));

        // Remove objects
        foreach (var gameObject in fsm.components.spriteToDestroy)
            gameObject.SetActive(false);
        foreach (var gameObject in fsm.components.spriteToShow)
            gameObject.SetActive(true);
    }

    // public override void Complete(StageFSM FSM, int stars = 1)
    // {
    //     if (stars <= (int) FSM.CurrentStageStatus)
    //         return;
    //
    //     for (var i = (int) FSM.CurrentStageStatus + 1; i <= stars; i++)
    //     {
    //         RewardFSM.SaveReward(FSM.rewards[i - 1], FSM.rewardsQuantity[i - 1], FSM.cardReward);
    //     }
    //
    //     GameDataV1.Instance.SetStageStatus(FSM.Level, (StageStatus) stars);
    //     FSM.CurrentStageStatus = GameDataV1.Instance.Stages[FSM.Level];
    // }
}

public class Done : State<StageFSM> {
    public override void Enter(StageFSM fsm) {
        fsm.components.doneBox.GetComponent<CanvasGroup>().alpha = 1f;
        fsm.components.canvasGroup.alpha = 0f;
        fsm.components.canvasGroup.interactable = false;
        fsm.components.canvasGroup.blocksRaycasts = false;

        // refresh state from each child
        fsm.nextStages?.ForEach(stage => stage.RefreshStageStatus(fsm));
        fsm.TurnLightsOn();
        // Remove objects
        foreach (var gameObject in fsm.components.spriteToDestroy)
            gameObject.SetActive(false);
        foreach (var gameObject in fsm.components.spriteToShow)
            gameObject.SetActive(true);
    }
}

}