using System;
using Framework.Base;
using UnityEngine;

namespace Core.Controller.Main {
public class MainController : StateMachine<MainController, State<MainController>> {
    [SerializeField] public Components components;
    protected override MainController FSM => this;
    protected override State<MainController> GetInitialState => States.Created;
}

[Serializable]
public class Components {
    
}
}