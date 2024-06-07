using System;
using Framework.Base;
using UnityEngine;

namespace Game.StateMachine.AbilityPanels {
public class AbilityPanelFSM : StateMachine<AbilityPanelFSM, State<AbilityPanelFSM>> {
    [SerializeField] public Components components;

    protected override AbilityPanelFSM FSM => this;
    protected override State<AbilityPanelFSM> GetInitialState => States.Preload;

    protected override void Before() {
        components.canvas.worldCamera = Camera.main;
        components.canvas.sortingLayerID = 4;
        components.canvas.sortingLayerName = "Above All";
    }
}

[Serializable]
public class Components {
    [SerializeField] public Canvas canvas;
}
}