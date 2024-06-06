using Game.StateMachine.Rocks;
using UnityEngine;

namespace Game.Behaves {
public class RockBreakingBehave : StateMachineBehaviour {
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        var rockFSM = animator.gameObject.GetComponentInParent<RockFSM>();
        rockFSM.DestroyRock();
    }
}
}