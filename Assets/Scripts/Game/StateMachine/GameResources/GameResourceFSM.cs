using System;
using System.Collections.Generic;
using Core.Controller.Audio;
using Core.Handler;
using Core.Sprites;
using Core.Utils;
using Core.Utils.Constants;
using DG.Tweening;
using Framework.Base;
using Game.Controller.Game;
using TMPro;
using UnityEngine;
using Random = System.Random;

namespace Game.StateMachine.GameResources {
public class GameResourceFSM : StateMachine<GameResourceFSM, State<GameResourceFSM>> {
    public const float EffectTime = 0.3f;

    [SerializeField] public Components components;

    private readonly Random _random = new();

    internal readonly Vector3 InitialPosition = new(-200f, 0f, 0f);
    private Color _textColor;
    internal bool AllEmpty = false;
    internal ResourceType Type = ResourceType.None;
    protected override GameResourceFSM FSM => this;
    protected override State<GameResourceFSM> GetInitialState => States.Preload;
    private GameController GameController { get; set; }
    internal List<OriginalSprite> SpriteIcons { get; } = new();
    internal bool OneAssetOnly { get; private set; } = true;

    internal AtomicInt Counter { set; get; } = new(0);

    public static GameResourceFSM Build(GameController gameController, ResourceType resourceType, int index,
        int quantity) {
        var gameObject = Instantiate(AssetLoader.AsGameObject(UI.ResourceArea),
            gameController.components.gameResourceArea);
        var gameResourceFSM = gameObject.GetComponent<GameResourceFSM>();
        gameResourceFSM.Prepare(gameController, resourceType, quantity, index);
        return gameObject.GetComponent<GameResourceFSM>();
    }

    private Vector2 GetPosition(int index) {
        return Type switch {
            ResourceType.Heart => new Vector3(-72.5f, -658, 0f),
            ResourceType.Elixir => new Vector3(-72.5f, -658 + 51, 0f),
            _ => new Vector3(-72.5f, 716f - index * 51, 0f)
        };
    }

    internal void PrintText() {
        components.counter.color = _textColor;
    }

    private void Prepare(GameController gameController, ResourceType resourceType, int quantity, int index) {
        Type = resourceType;
        gameObject.transform.localPosition = GetPosition(index);

        if (resourceType is ResourceType.Heart or ResourceType.Elixir)
            OneAssetOnly = false;
        _textColor = Colors.From(resourceType);
        GameController = gameController;
        Counter = new AtomicInt(quantity);
        State.Prepare(FSM);
    }

    public int GetQuantity() {
        return Counter.Value;
    }

    public bool Decrease(int value = 1) {
        return State.Decrease(FSM, value);
    }

    public void Increase(int value = 1) {
        State.Increase(FSM, value);
    }

    public void IncreaseWithEffect(Vector3 from, int value = 1) {
        for (var i = 0; i < value; i++) {
            var iconInstance = Instantiate(AssetLoader.AsGameObject(UI.ResourceIcon),
                GameController.components.gameResourceArea);
            iconInstance.GetComponent<OriginalSprite>().image.sprite = AssetLoader.AsSprite(Type);
            iconInstance.transform.position = new Vector3(from.x + (float)_random.NextDouble(),
                from.y + (float)_random.NextDouble(), from.z);
            iconInstance.transform.DOMove(SpriteIcons[0].transform.position, 0.5f).OnComplete(() => {
                FSM.State.Increase(FSM);
                if (FSM.Type is ResourceType.Coin)
                    AudioController.PlayFXRandom(FSM.components.coinSounds);
                iconInstance.transform.DOKill();
                Destroy(iconInstance);
            });
        }
    }

    internal void ChangeText(int totalLeft) {
        const float fadeInSize = 1.5f;
        components.counter.text = $"{totalLeft.ToString()} x";
        components.counter.rectTransform.DOKill();
        components.counter.rectTransform.localScale = new Vector3(fadeInSize, fadeInSize, fadeInSize);
        components.counter.rectTransform.DOScale(1f, EffectTime);
    }
}

[Serializable]
public class Components {
    // [SerializeField] public 

    [SerializeField] public TextMeshProUGUI counter;
    [SerializeField] public List<AudioClip> coinSounds;
}
}