using System;
using System.Collections.Generic;
using System.Linq;
using Core.Data;
using Core.StateMachine.CharacterLifeBar;
using Core.StateMachine.Rewards;
using Core.StateMachine.Stages;
using Core.Utils.Constants;
using Framework.Base;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Popup.StagePopup {

public class StagePopup : StateMachine<StagePopup, State<StagePopup>> {
    [SerializeField] public StagePopupComponents components;

    protected override StagePopup FSM => this;
    protected override State<StagePopup> GetInitialState => States.Started;

    private StageFSM CurrentStageFSM { get; set; }

    protected override void SyncDataBase() {
        components.playButton.gameObject.SetActive(PlayerDataV1.Instance.CurrentPlayerHasLife());
    }

    protected override void Before() {
        components.playButton.onClick.AddListener(() => { Play(1); });
        // components.playButton2.onClick.AddListener(() => { Play(2);});
        // components.playButton3.onClick.AddListener(() => { Play(3);});
        components.playButtonHit.onClick.AddListener(Hit);
        // components.playButtonCure.onClick.AddListener(Cure);
        components.closeButton.onClick.AddListener(() => { gameObject.SetActive(false); });
    }

    public void OpenStage(StageFSM stageFSM) {
        CurrentStageFSM = stageFSM;
        components.level.text = stageFSM.Level.ToString();
        // TODO add label on stages for maps
        // components.levelDifficulty.text = LocalizationUtils.GetLocalizedText("Label.StageDifficultyType." + stageFSM.difficulty);
        // components.levelType.text = LocalizationUtils.GetLocalizedText("Label.StageType." + stageFSM.levelType);
        components.playButton.gameObject.SetActive(PlayerDataV1.Instance.CurrentPlayerHasLife());
        components.topBarImage.sprite = components.topBarByLevelSprites[(int)stageFSM.levelType];
        components.topBarImage.color = DifficultyUtil.From(stageFSM.difficulty).color;

        // Change reward status
        for (var i = 0; i < 3; i++) {
            components.rewards[i].ChangeState(StateMachine.Rewards.States.NotEarned);
            components.rewards[i].ChangeReward(stageFSM.rewards[i], stageFSM.cardReward, stageFSM.rewardsQuantity[i]);

            if (i + 1 <= (int)stageFSM.CurrentStageStatus) {
                components.rewards[i].ChangeState(StateMachine.Rewards.States.Earned);
                components.stars[i].color = Colors.MEDIUM;
                components.stars[i + 3].color = Colors.MEDIUM;
                components.stars[i + 6].color = Colors.MEDIUM;
                components.rewardBox[i].color = Colors.SECONDARY;
                components.rewardShadow[i].enabled = true;
                // components.stars[0][i].color = Color.WHITE;
            }
            else {
                components.stars[i].color = Colors.DISABLED_ALPHA;
                components.stars[i + 3].color = Colors.DISABLED_ALPHA;
                components.stars[i + 6].color = Colors.DISABLED_ALPHA;
                components.rewardBox[i].color = Colors.DISABLED_ALPHA;
                components.rewardShadow[i].enabled = false;
                // components.stars[0][i].color = Color.BLACK_ALPHA_1;
            }
            // else
            //     components.rewards[i].ChangeState(States.NOT_EARNED);
        }

        gameObject.SetActive(true);
    }

    // TODO change play functionality
    private void Play(int stars) {
        // CurrentStageFSM.State.Complete(CurrentStageFSM, stars);
        PlayerDataV1.Instance.SetLastPosition(components.mainCamera.gameObject.transform.position);
        StageFSM.SetCurrentStage(CurrentStageFSM);
        gameObject.SetActive(false);
        components.customTransition.GetComponent<CustomTransition>().TransitionTo("GameScene");
    }

    // TODO remove for tests
    private void Hit() {
        var selectedCharacter = PlayerDataV1.Instance.selectedCharacter;
        foreach (var componentsCharacterLifeBar in components.characterLifeBars.Where(componentsCharacterLifeBar =>
                     componentsCharacterLifeBar.character == selectedCharacter))
            componentsCharacterLifeBar.TakeHit();

        gameObject.SetActive(false);
    }

    private void Cure() {
        foreach (var componentsCharacterLifeBar in components.characterLifeBars)
            componentsCharacterLifeBar.Cure();
        gameObject.SetActive(false);
    }
}

[Serializable]
public class StagePopupComponents {
    [SerializeField] public TextMeshProUGUI level;
    [SerializeField] public TextMeshProUGUI levelType;
    [SerializeField] public TextMeshProUGUI levelDifficulty;

    [SerializeField] public Button playButton;

    // [SerializeField] public Button playButton2;
    // [SerializeField] public Button playButton3;
    [SerializeField] public Button playButtonHit;

    // [SerializeField] public Button playButtonCure;
    [SerializeField] public List<CharacterLifeBarFSM> characterLifeBars;
    [SerializeField] public Camera mainCamera;
    [SerializeField] public List<RewardFSM> rewards;
    [SerializeField] public List<Image> stars;
    [SerializeField] public List<Image> rewardBox;
    [SerializeField] public List<Shadow> rewardShadow;
    [SerializeField] public Button closeButton;
    [SerializeField] public GameObject customTransition;
    [SerializeField] public List<Sprite> topBarByLevelSprites;
    [SerializeField] public Image topBarImage;
}

}