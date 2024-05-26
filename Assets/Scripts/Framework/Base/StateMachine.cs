using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Framework.Base {

public abstract class StateMachine<T, U> : MonoBehaviour where T : MonoBehaviour where U : State<T> {
    // ReSharper disable once InconsistentNaming
    public U State;
    protected abstract T FSM { get; }
    protected abstract U GetInitialState { get; }
    private bool isInitialized = false; 

    public async void Awake() {
        // ReSharper disable once MethodHasAsyncOverload
        Before();
        await BeforeAsync();
        State = GetInitialState;
        State.Before(FSM);
        isInitialized = true;
    }

    private IEnumerator Start()
    {
        while (!isInitialized) yield return null;

        if (GetInitialState == State)
            State.Enter(FSM);
    }


    public void Update() {
        if (isInitialized)
           State?.Update(FSM);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (isInitialized)
            State.OnCollisionEnter(FSM, collision);
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (isInitialized)
            State.OnCollisionExit(FSM, collision);
    }

    protected virtual Task BeforeAsync() {
        return Task.CompletedTask;
    }

    // Generic method to load any type of asset from Addressables
    public async Task<Tu> LoadAssetAsync<Tu>(string address) where Tu : Object {
        AsyncOperationHandle handle = Addressables.LoadAssetAsync<Tu>(address);

        try {
            var asset = (Tu)await handle.Task;
            if (asset != null) {
                // Here you can use the asset, for example, instantiate it
                return asset;
            }

            Debug.LogError($"Failed to load asset: {asset}");
            return null;
        }
        catch (Exception ex) {
            Debug.LogError($"Failed to load asset: {ex.Message}");
            return null;
        }
    }

    public void SyncAllData(Type @class) {
        transform.root.BroadcastMessage("SyncData", @class);
    }

    public void SyncAllData<Tu, Tz>(TagType tagType) where Tu : StateMachine<Tu, Tz> where Tz : State<Tu> {
        var objectsWithTag = GameObject.FindGameObjectsWithTag(tagType.ToString());
        if (objectsWithTag.Length == 0) return;
        foreach (var obj in objectsWithTag) {
            var stateMachine = obj.GetComponent<Tu>();
            if (stateMachine == null) continue;

            stateMachine.State?.SyncData(stateMachine);
            stateMachine.SyncDataBase();
        }
    }

    public void ChangeState(U newState) {
        if (State == newState) return;
        State?.Exit(FSM);
        State = newState;
        ChangeStateBase();
        State.Enter(FSM);
    }

    public virtual void ChangeStateBase() { }

    //
    public void SelfDestroy() {
        Destroy(gameObject);
    }

    // public virtual void DestroyInstance(GameObject obj) {
    //     DestroyImmediate(obj, true);
    // }

    public virtual void CreateInstance(GameObject obj) {
        _ = Instantiate(obj);
    }

    public virtual GameObject CreateInstance(GameObject obj, Transform area) {
        return Instantiate(obj, area);
    }

    protected virtual void Before() { }

    protected virtual void Setup() { }
    //
    // // Propagate methods
    // protected void IncreaseLevel() {
    //     State.IncreaseLevel(FSM);
    // }

    public void LoadScene(string scene) {
        SceneManager.LoadScene(scene);
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

public abstract class PersistentStateMachine<T, U> : StateMachine<T, U> where T : MonoBehaviour where U : State<T> {
    public static PersistentStateMachine<T, U> Instance { get; set; }

    public new void Awake() {
        if (ScenesToDestroy().Exists(s => s.ToString() == SceneManager.GetActiveScene().name)) {
            // Aways destroy if scene should be
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

    public virtual List<GameScenes> ScenesToDestroy() {
        return new List<GameScenes>();
    }

    public virtual void BeforeChangeScene() { }

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