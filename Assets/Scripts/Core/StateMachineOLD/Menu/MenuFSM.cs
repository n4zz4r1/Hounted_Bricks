using System.Collections.Generic;
using Framework.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Core.StateMachine.Menu {

public class MenuFSM : StateMachine<MenuFSM, State<MenuFSM>> {
    [SerializeField] public List<Button> menuButtons;
    [SerializeField] public RectTransform[] panels; // Panels that correspond to each button

    internal readonly Dictionary<int, MenuPanel> PanelByIndex = new() {
        { 0, MenuPanel.CHARACTER },
        { 1, MenuPanel.BAG },
        { 2, MenuPanel.HOME },
        { 3, MenuPanel.MAP },
        { 4, MenuPanel.SHOP }
    };

    internal int CurrentPanelIndex = 2;

    protected override MenuFSM FSM => this;
    protected override State<MenuFSM> GetInitialState => States.Started;
}

public enum MenuPanel {
    CHARACTER = -5774,
    BAG = -2845,
    HOME = 0,
    MAP = 3244,
    SHOP = 6169
}

}