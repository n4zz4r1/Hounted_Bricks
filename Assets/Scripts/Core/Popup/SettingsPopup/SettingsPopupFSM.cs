using Core.Data;
using Core.Utils.Constants;
using Framework.Base;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Core.Popup.SettingsPopup {
public class SettingsPopupFSM : StateMachine<SettingsPopupFSM, State<SettingsPopupFSM>> {
    [SerializeField] public Button aboutButton;
    [SerializeField] public Button langEnglishButton;
    [SerializeField] public Button langPortugueseButton;

    protected override SettingsPopupFSM FSM => this;
    protected override State<SettingsPopupFSM> GetInitialState => States.Started;

    protected override void Before() {
        aboutButton.onClick.AddListener(GoToHome);

        langEnglishButton.onClick.AddListener(ChangeLangToEnglish);
        langPortugueseButton.onClick.AddListener(ChangeLangToPortuguese);
        ChangeLangButton(SettingsDataV1.Instance.GetLanguage());

        cleanButton.onClick.AddListener(DataManager.Clean);
        // rewardButton.onClick.AddListener(() => {
        //     if (rewardFSM == null) throw new ArgumentNullException(nameof(rewardFSM));
        //
        //     for (var i = 0; i < 50; i++) {
        //         var rewardTemp = CreateInstance(rewardFSM, transform);
        //         var rewardFSMInstance = rewardTemp.GetComponent<RewardFSM>();
        //         rewardFSMInstance.shouldDestroy = true;
        //         rewardFSMInstance.RollTheDice(NormalRewardDice.Roll());
        //         rewardFSMInstance.State.Earn(rewardFSMInstance);
        //     }
        //
        //     SyncAllData(typeof(MenuFSM));
        // });
        // reward1000Button.onClick.AddListener(() => {
        //     if (rewardFSM == null) throw new ArgumentNullException(nameof(rewardFSM));
        //
        //     for (var i = 0; i < 1000; i++) {
        //         var rewardTemp = CreateInstance(rewardFSM, transform);
        //         var rewardFSMInstance = rewardTemp.GetComponent<RewardFSM>();
        //         rewardFSMInstance.shouldDestroy = true;
        //         rewardFSMInstance.RollTheDice(NormalRewardDice.Roll());
        //         rewardFSMInstance.State.Earn(rewardFSMInstance);
        //     }
        //
        //     SyncAllData(typeof(MenuFSM));
        // });
    }

    private void ChangeLangToEnglish() {
        SettingsDataV1.Instance.ChangeLanguage(BMLanguage.English);
        ChangeLangButton(BMLanguage.English);
    }

    private void ChangeLangButton(BMLanguage language) {
        if (language == BMLanguage.English) {
            langEnglishButton.enabled = false;
            langEnglishButton.GetComponent<Image>().color = Colors.WHITE;
            langEnglishButton.GetComponent<Shadow>().enabled = false;
            langPortugueseButton.enabled = true;
            langPortugueseButton.GetComponent<Image>().color = Colors.DISABLED;
            langPortugueseButton.GetComponent<Shadow>().enabled = false;
        }
        else {
            langEnglishButton.enabled = true;
            langEnglishButton.GetComponent<Image>().color = Colors.DISABLED;
            langEnglishButton.GetComponent<Shadow>().enabled = false;
            langPortugueseButton.enabled = false;
            langPortugueseButton.GetComponent<Image>().color = Colors.WHITE;
            langPortugueseButton.GetComponent<Shadow>().enabled = false;
        }
    }

    private void ChangeLangToPortuguese() {
        SettingsDataV1.Instance.ChangeLanguage(BMLanguage.Portuguese);
        ChangeLangButton(BMLanguage.Portuguese);
    }

    private static void GoToHome() {
        SceneManager.LoadScene("AboutScene");
    }

    // TODO remove dev region

    #region DEV

    [SerializeField] public Button cleanButton;
    [SerializeField] public Button rewardButton;
    [SerializeField] public Button reward1000Button;

    #endregion
}
}