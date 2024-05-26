using System;
using System.Globalization;
using Core.Data;
using Framework.Base;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;

namespace Core.Controller.Home {

public class HomeController : Controller<HomeController, State<HomeController>> {
    [SerializeField] public Components components;
    protected override HomeController FSM => this;
    protected override State<HomeController> GetInitialState => States.Created;

    protected override void Before() {
        components.playButton.onClick.AddListener(() => TransitionWithEffectTo("GameScene"));
        components.levelText.text = GameDataV1.Instance.level.ToString(CultureInfo.InvariantCulture);
    }
}

[Serializable]
public class Components {
    [SerializeField] public Button playButton;
    [SerializeField] public TextMeshProUGUI levelText;
}

}