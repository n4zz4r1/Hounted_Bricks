using System;
using System.Threading.Tasks;
using Core.StateMachine.Stages;
using Core.Utils;
using Framework.Base;
using Game.Controller.Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Popup.GameMenu {
public class GameMenuFSM : StateMachine<GameMenuFSM, State<GameMenuFSM>> {
    [SerializeField] public GameMenuComponents components;
    [SerializeField] public StageFSM stageFSM;

    internal string GamePausedLabel = string.Empty;
    internal string YouLoseLabel = string.Empty;
    internal string YouWonLabel = string.Empty;

    protected override GameMenuFSM FSM => this;
    protected override State<GameMenuFSM> GetInitialState => States.Playing;

    protected override async Task BeforeAsync() {
        GamePausedLabel = await LocalizationUtils.LoadTextAsync("Game.Pause");
        YouWonLabel = await LocalizationUtils.LoadTextAsync("Game.Won");
        YouLoseLabel = await LocalizationUtils.LoadTextAsync("Game.Lost");
    }

    protected override void Before() {
        components.buttonLeave.onClick.AddListener(() => State.Leave(FSM));
        components.win.onClick.AddListener(() => State.Win(FSM, 2));
        components.buttonUnpause.onClick.AddListener(() => State.Unpause(FSM));
        components.buttonNextLevel.onClick.AddListener(() => LoadScene("GameScene"));
        components.buttonRestart.onClick.AddListener(() => {
            LoadScene("GameScene");
            Time.timeScale = 1f;
        });

        components.buttonPause.onClick.AddListener(() => State.Pause(FSM));
        stageFSM = StageFSM.GetCurrentStage();

        if (stageFSM.isMapStage)
            components.buttonRestart.gameObject.SetActive(false);

        // TODO Localize here
        var level = stageFSM.isMapStage ? "Campaign Level: " : "Level: ";
        components.stageDetail.text = level + stageFSM.Level;
    }
}

[Serializable]
public class GameMenuComponents {
    // TODO remove dev area
    [SerializeField] public Button win;
    [SerializeField] public Button buttonPause;
    [SerializeField] public Button buttonUnpause;
    [SerializeField] public Button buttonLeave;
    [SerializeField] public Button buttonRestart;
    [SerializeField] public GameController gameController;
    [SerializeField] public TextMeshProUGUI gameMenuTitle;
    [SerializeField] public TextMeshProUGUI stageDetail;
    [SerializeField] public GameObject menuBox;
    [SerializeField] public Button buttonNextLevel;
}
}