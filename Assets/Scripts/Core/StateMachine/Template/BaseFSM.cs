using System;
using Framework.Base;
using UnityEngine;

namespace Core.StateMachine.Template {

/**
 * Refactor: ok
 */
public class TemplateFSM : StateMachine<TemplateFSM, State<TemplateFSM>> {
    [SerializeField] public Components components;

    protected override TemplateFSM FSM => this;
    protected override State<TemplateFSM> GetInitialState => States.Created;
}

[Serializable]
public class Components { }

}