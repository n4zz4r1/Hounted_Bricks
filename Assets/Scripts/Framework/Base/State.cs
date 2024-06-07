using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.Base {
// Common Finite States
public abstract class State<T> {
    #region State Machine Defaults

    public virtual void Before(T fsm) { }
    public virtual void Enter(T fsm) { }

    public virtual Task AsyncEnter(T fsm) {
        return Task.CompletedTask;
    }

    public virtual void Update(T fsm) { }
    public virtual void OnCollisionEnter(T fsm, Collider2D collider) { }
    public virtual void OnCollisionExit(T fsm, Collider2D collider) { }
    public virtual void Exit(T fsm) { }

    public virtual IEnumerator ExitAsync(T fsm, System.Action callback) {
        yield return null;
        
    }
    public virtual void SyncData(T fsm) { }
    public virtual void OpenPopup(T fsm) { }
    public virtual void TransitionTo(T fsm, string scene, Button from) { }
    public virtual void Released(T fsm) { }
    public virtual void Pressed(T fsm) { }

    #endregion

    #region Common Behaves

    public virtual void StartDragging(T fsm) { }
    public virtual void StopDragging(T fsm) { }
    public virtual void Buy(T fsm) { }
    public virtual void Click(T fsm) { }
    public virtual void Clear(T fsm) { }
    public virtual void Break(T fsm) { }
    public virtual bool Decrease(T fsm, int value = 1) => false;
    public virtual void Increase(T fsm, int value = 1) { }
    public virtual void Inactive(T fsm) { }
    public virtual void Active(T fsm) { }
    public virtual void Prepare(T fsm) { }
    public virtual void Select(T fsm) { }
    public virtual void Destroy(T fsm) { }
    public virtual void Hide(T fsm) { }
    public virtual void Enable(T fsm) { }
    public virtual void Disable(T fsm) { }
    public virtual void Show(T fsm) { }
    public virtual void Pause(T fsm) { }
    public virtual void Unpause(T fsm) { }
    public virtual void Leave(T fsm) { }
    public virtual void Stop(T fsm) { }
    public virtual void Move(T fsm) { }
    public virtual void Rotate(T fsm) { }
    public virtual void Shuffle(T fsm) { }
    public virtual void SpeedUp(T fsm) { }
    public virtual void Unselect(T fsm) { }
    public virtual void Earn(T fsm) { }
    public virtual void Choose(T fsm) { }
    public virtual void Next(T fsm) { }
    public virtual void Previous(T fsm) { }

    #endregion

    #region Specifics

    public virtual void SetCard(T fsm) { }
    public virtual void RollTheDice(T fsm) { }
    public virtual void ChangeReward(T fsm, int amount) { }
    public virtual void Hit(T fsm) { }
    public virtual void Hit(T fsm, float damage) { }
    public virtual void Move(T fsm, float x) { }
    public virtual void NextWave(T fsm) { }
    public virtual void TakeHit(T fsm) { }
    public virtual void AddLife(T fsm) { }
    public virtual void Aim(T fsm) { }
    public virtual void Shoot(T fsm) { }
    public virtual void Collect(T fsm) { }
    public virtual void Kill(T fsm) { }
    public virtual void Complete(T fsm, int stars = 1) { }
    public virtual void Win(T fsm, int stars) { }
    public virtual void Loose(T fsm) { }
    public virtual void StopMoving(T fsm) { }

    #endregion

    // public virtual void IncreaseLevel(T fsm) { }
    // public virtual void Hit(T fsm) { }
    // public virtual void Hit(T fsm, float damage) { }
    // 

    // Specifics
    // public virtual void SetCard(T fsm) { }
    // public virtual void Wakeup(T fsm) { }
    // public virtual void Bark(T fsm) { }
    // public virtual void StartShooting(T fsm, Vector2 to) { }
}
}