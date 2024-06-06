using UnityEngine;

namespace Game.Behaves {
public class PlayerShootingBehave : StateMachineBehaviour {
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state

    // public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    // {
    //     Debug.Log("OnStateMachineExit");
    //     animator.gameObject.SendMessage("NextShoot");
    // }

    // public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    // {
    //     Debug.Log("OnStateEnter");
    //     animator.gameObject.SendMessage("NextShoot");
    //     // base.OnStateEnter(animator, stateInfo, layerIndex);
    // }

    // public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    // {
    //     animator.gameObject.SendMessage("NextShoot");
    //     base.OnStateExit(animator, stateInfo, layerIndex);
    //
    // }
}
}