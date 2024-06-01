using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Core.Data;
using Core.Handler;
using Core.Utils;
using Core.Utils.Constants;
using Framework.Base;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace Core.StateMachine.Stages {

public class StageFSM : StateMachine<StageFSM, State<StageFSM>>, IPointerClickHandler {
    [SerializeField] public List<StageFSM> nextStages = new();

    [SerializeField] public StageLevelType levelType = StageLevelType.GARDEN;
    [SerializeField] public StageDifficultyType difficulty = StageDifficultyType.EASY;
    [SerializeField] public Card cardReward = Card.NONE;

    [SerializeField]
    public List<ResourceType> rewards = new() { ResourceType.COIN, ResourceType.COIN, ResourceType.COIN };

    [SerializeField] public List<int> rewardsQuantity = new() { 10, 5, 1 };
    [SerializeField] public bool isMapStage;

    [SerializeField] public StageProperties properties;

    [SerializeField] public Components components;

    protected override StageFSM FSM => this;
    protected override State<StageFSM> GetInitialState => States.Preload;

    public int Level { get; private set; }

    internal StageStatus CurrentStageStatus { get; set; }

    public void OnPointerClick(PointerEventData eventData) {
        if (!isMapStage) return;

        // TODO map stage
        // components.stagePopup.gameObject.SetActive(true);
        // components.stagePopup.OpenStage(FSM);
    }

    protected override void Before() {
        if (isMapStage)
            Level = int.Parse(Regex.Match(gameObject.name, @"\d+").Value);
        else
            Level = (int)GameDataV1.Instance.level;
    }

    public void SetNextLevel() {
        if (isMapStage)
            Level = int.Parse(Regex.Match(gameObject.name, @"\d+").Value);
        else
            Level = (int)GameDataV1.Instance.level;
    }

    public static void SetCurrentStage(StageFSM stageFSM) {
        var currentStage = GameObject.FindWithTag("currentStage");
        if (currentStage != null) Destroy(currentStage.gameObject);

        // create a clone and set it on root
        var stageFSMClone = Instantiate(stageFSM.gameObject, null, false);
        stageFSMClone.name = "Stage Details";
        stageFSMClone.tag = "currentStage";
        DontDestroyOnLoad(stageFSMClone);
    }

    public static StageFSM GetCurrentStage() {
        var currentStage = GameObject.FindWithTag("currentStage");
        if (currentStage == null)
            SetCurrentStage(GetMainStage());

        return GameObject.FindWithTag("currentStage").GetComponent<StageFSM>();
    }

    public static StageFSM GetMainStage() {
        // Debug.Log("lets create stage");
        var cardPrefab = Resources.Load("Stage") as GameObject;
        if (cardPrefab != null) {
            // Debug.Log("creating");
            var cardObjectInstance = Instantiate(cardPrefab);
            var stageFSM = cardObjectInstance.GetComponent<StageFSM>();
            stageFSM.isMapStage = false;
            // Debug.Log("stage found");
            return stageFSM;
        }

        throw new Exception("no preset called 'Stage'");
    }

    public void RefreshStageStatus(StageFSM parentFSM) {
        FSM.CurrentStageStatus = GameDataV1.Instance.GetStageStatus(FSM.Level);


        FSM.ChangeState(States.Available);
        // TODO DEV change to all available, remove it here
        // switch (FSM.CurrentStageStatus)
        // {
        //     case StageStatus.NOT_DONE:
        //         if (parentFSM != null && parentFSM.CurrentStageStatus != StageStatus.NOT_DONE)
        //             FSM.ChangeState(States.AVAILABLE);
        //         else
        //             FSM.ChangeState(States.UNAVAILABLE);
        //         break;
        //     case StageStatus.DONE_ONE_STAR or StageStatus.DONE_TWO_STAR:
        //         FSM.ChangeState(States.AVAILABLE);
        //         break;
        //     case StageStatus.DONE_THREE_STAR:
        //         FSM.ChangeState(States.DONE);
        //         break;
        // }

        if ((State == States.Available || State == States.Done) && parentFSM != null)
            FSM.components.dottedLineHandler.DrawDottedLine(FSM.transform.position,
                parentFSM.gameObject.transform.position);
    }

    public void UpdateStars() {
        for (var i = 0; i < (int)FSM.CurrentStageStatus; i++) {
            FSM.components.stars[i].GetComponent<Image>().color = Colors.GOLD_CARD;
            FSM.components.stars[i].GetComponent<Shadow>().enabled = true;
        }
    }

    public void TurnLightsOn() {
        if (components.light2DBase != null) components.light2DBase.gameObject.SetActive(true);
    }
}

[Serializable]
public class Components {
    [SerializeField] public PathLineHandler dottedLineHandler;
    [SerializeField] public CanvasGroup canvasGroup;
    [SerializeField] public TextMeshProUGUI levelLabel;
    [SerializeField] public List<GameObject> stars;
    [SerializeField] public GameObject box;
    [SerializeField] public Shadow boxShadow;
    [SerializeField] public Image boxImage;
    [SerializeField] public TextMeshProUGUI difficultyLabel;
    [SerializeField] public GameObject doneBox;
    // [SerializeField] public StagePopup stagePopup;
    [SerializeField] public Light2DBase light2DBase;

    // Remove and Add game objects for doors, windows and others
    [SerializeField] public List<GameObject> spriteToDestroy = new();
    [SerializeField] public List<GameObject> spriteToShow = new();
}

[Serializable]
public class StageWave {
    [SerializeField] public int positionY;
    [SerializeField] public Monsters.Monster monster;
    [Range(0, 5)] [SerializeField] public List<int> positionX;
}

[Serializable]
public class StageBoss {
    [Range(0, 4)] [SerializeField] public int positionX;
    [SerializeField] public Monsters.MonsterBoss boss;
}

[Serializable]
public class StageProperties {
    [SerializeField] public bool autoGenerate = true;
    [SerializeField] public List<StageWave> waves;
    [SerializeField] public StageBoss boss;
}

}