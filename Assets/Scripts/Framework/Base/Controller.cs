using UnityEngine;

namespace Framework.Base {

public abstract class Controller<T, U> : StateMachine<T, U> where T : MonoBehaviour where U : State<T> {

    [SerializeField] public GameObject customTransition;

    public void TransitionWithEffectTo(string scene) {
        customTransition.GetComponent<CustomTransition>().TransitionTo(scene);
    }
}

}