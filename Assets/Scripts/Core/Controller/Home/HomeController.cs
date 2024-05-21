using System;
using Framework.Base;
using UnityEngine;

namespace Core.Controller.Home {

public class HomeController : StateMachine<HomeController, State<HomeController>> {
    [SerializeField] public Components components;
    protected override HomeController FSM => this;
    protected override State<HomeController> GetInitialState => States.Created;
}

[Serializable]
public class Components { }

}