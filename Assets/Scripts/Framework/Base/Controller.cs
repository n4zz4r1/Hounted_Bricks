using UnityEngine;

namespace Framework.Base {
public abstract class Controller<T, TU> : StateMachine<T, TU> where T : MonoBehaviour where TU : State<T> {
    [SerializeField] public GameObject customTransition;

    public void TransitionWithEffectTo(string scene) {
        customTransition.GetComponent<CustomTransition>().TransitionTo(scene);
    }
    
    public void FadeIn() => customTransition.GetComponent<CustomTransition>().FadeIn();

}
}