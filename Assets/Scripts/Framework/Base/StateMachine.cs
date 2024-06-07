using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Framework.Base {
public abstract class StateMachine<T, TU> : MonoBehaviour where T : MonoBehaviour where TU : State<T> {
    // ReSharper disable once InconsistentNaming
    public TU State;
    private bool _isInitialized;
    protected abstract T FSM { get; }
    protected abstract TU GetInitialState { get; }

    public async void Awake() {
        // ReSharper disable once MethodHasAsyncOverload
        Before();
        await BeforeAsync();
        State = GetInitialState;
        State.Before(FSM);
        _isInitialized = true;
    }

    private IEnumerator Start() {
        while (!_isInitialized) yield return null;

        if (GetInitialState == State)
            State.Enter(FSM);
    }

    public void Update() {
        if (_isInitialized) {
            State?.Update(FSM);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (_isInitialized)
            State.OnCollisionEnter(FSM, collision);
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (_isInitialized)
            State.OnCollisionExit(FSM, collision);
    }

    public void Sync() {
        SyncDataBase();
    }

    protected virtual Task BeforeAsync() {
        return Task.CompletedTask;
    }

    public void SyncAllData(Type @class) {
        transform.root.BroadcastMessage("SyncData", @class);
    }

    public void SyncAllData<TY, TB>(TagType tagType) where TY : StateMachine<TY, TB> where TB : State<TY> {
        var objectsWithTag = GameObject.FindGameObjectsWithTag(tagType.ToString());
        if (objectsWithTag.Length == 0) return;
        foreach (var obj in objectsWithTag) {
            var stateMachine = obj.GetComponent<TY>();
            if (stateMachine == null) continue;

            stateMachine.State?.SyncData(stateMachine);
            stateMachine.SyncDataBase();
        }
    }

    public void ChangeState(TU newState) {
        if (State == newState) return;
        State?.Exit(FSM);
        State = newState;
        ChangeStateBase();
        State.Enter(FSM);
    }
    
    public void ChangeStateWithCoroutine(TU newState) {
        if (State == newState) return;
        StartCoroutine(State?.ExitAsync(FSM, () => {
            State = newState;
            ChangeStateBase();
            State.Enter(FSM);
        }));
    }

    public virtual void ChangeStateBase() { }

    public virtual void CreateInstance(GameObject obj) {
        _ = Instantiate(obj);
    }

    public virtual GameObject CreateInstance(GameObject obj, Transform area) {
        return Instantiate(obj, area);
    }

    protected virtual void Before() { }

    public void LoadScene(string scene) {
        SceneManager.LoadScene(scene);
    }

    public void DestroyObject(Object o) {
        Destroy(o);
    }
    

    // Propagate methods
    protected void OpenPopup() {
        State.OpenPopup(FSM);
    }

    protected virtual void SyncDataBase() { }

    public void SyncData(Type @class) {
        if (@class != FSM.GetType())
            return;
        // Debug.Log("sync " + @class + " with state " + State);
        State?.SyncData(FSM);
        SyncDataBase();
    }
}

public abstract class PersistentStateMachine<T, TU> : StateMachine<T, TU> where T : MonoBehaviour where TU : State<T> {
    protected static PersistentStateMachine<T, TU> Instance { get; private set; }

    public new void Awake() {
        if (ScenesToDestroy().Exists(s => s.ToString() == SceneManager.GetActiveScene().name)) {
            // Always destroy if scene should be
            Destroy(gameObject); // Destroy any duplicate instance
            if (Instance != null) Destroy(Instance.gameObject);
        }
        else if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Ensure it persists across scenes

            // Call the base Awake method to set initial state
            base.Awake();
        }
        else {
            Instance.BeforeChangeScene(); // call BeforeChangeScene to be executed every time a scene changed
            Destroy(gameObject); // Destroy any duplicate instance
        }
    }

    protected virtual List<GameScenes> ScenesToDestroy() {
        return new List<GameScenes>();
    }

    protected virtual void BeforeChangeScene() { }

    public override void ChangeStateBase() {
        // Additional behaviors on state change, if needed
    }
}

public enum TagType {
    Resource
}

public enum GameScenes {
    GameScene,
    MainScene,
    PreloadScene,
    AboutUs
}

}