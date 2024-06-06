using Core.Utils;
using Game.StateMachine.Rocks;
using UnityEngine;

namespace Game.Handler {
public class RockColliderHandler : MonoBehaviour {
    [SerializeField] public RockFSM rockFSM;

    private void OnTriggerEnter2D(Collider2D other) {
        // Debug.Log("OnTriggerEnter2D with " + other);
        if (other.CompareTag(Tags.EndLine))
            rockFSM.State.Destroy(rockFSM);

        else if (other.CompareTag(Tags.Wall))
            rockFSM.WallSound();

        else if (other.CompareTag(Tags.Monster))
            // TODO hit monster
            rockFSM.HitMonster(other.gameObject);
    }
}
}