using System;
using System.Collections.Generic;
using Core.Controller.Audio;
using Core.Handler;
using Core.Sprites;
using Core.StateMachine.Cards;
using Core.Utils;
using Core.Utils.Constants;
using DG.Tweening;
using Framework.Base;
using Game.Controller.Game;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace Game.StateMachine.GameResources {

public class GameResourceFSM : StateMachine<GameResourceFSM, State<GameResourceFSM>> {

    [SerializeField] public Core.Utils.Constants.GameResource type;
    [SerializeField] public Color textColor;
    [SerializeField] public bool oneAssetOnly = true;
    [SerializeField] public Components components;
    private Random random = new Random();

    protected override GameResourceFSM FSM => this;
    protected override State<GameResourceFSM> GetInitialState => States.Preload;
    internal GameController GameController { get; private set; }
    internal List<OriginalSprite> SpriteIcons { get; set; } = new();
    public const float EffectTime = 0.3f;

    internal readonly Vector3 InitialPosition = new(-200f,0f, 0f);
    internal readonly Vector2 IconSize = new(40f,40f);

    public static GameResourceFSM Build(GameController gameController, GameResource gameResource, int index) {
        var gameObject = Instantiate(AssetLoader.AsGameObject(gameResource), gameController.components.gameResourceArea);
        var gameResourceFSM = gameObject.GetComponent<GameResourceFSM>();
        gameResourceFSM.Prepare(gameController);
        
        gameObject.transform.localPosition = new Vector3(-72.5f, 716f - index * 51, 0f);
        return gameObject.GetComponent<GameResourceFSM>();
    }

    internal AtomicInt Counter { set; get; } = new AtomicInt(0);
    internal bool AllEmpty = false;

    internal void PrintText() {
        components.counter.color = textColor;
    }

    public void Prepare(GameController gameController) {
        GameController = gameController;
        State.Prepare(FSM);
    }

    public int GetQuantity() => Counter.Value;

    public bool Decrease(int value = 1) => State.Decrease(FSM, value);

    public void Increase(int value = 1) => State.Increase(FSM, value);

    public void IncreaseWithEffect(Vector3 from, int value = 1) {
        
        for (var i = 0; i < value; i++) {
            var iconInstance = Instantiate(AssetLoader.AsGameObject((GameResourceType) type),
                GameController.components.gameResourceArea);
            iconInstance.transform.position = new Vector3(from.x + (float)random.NextDouble(), from.y + (float)random.NextDouble(), from.z);
            iconInstance.transform.DOMove(SpriteIcons[0].transform.position, 0.5f).OnComplete(() => {
                FSM.State.Increase(FSM, 1);
                if (FSM.type is GameResource.Coin)
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

    [SerializeField] public Sprite originalSprite;
    [SerializeField] public Sprite emptySprite;
    [SerializeField] public Sprite recoverSprite;

    [SerializeField] public TextMeshProUGUI counter;
    [SerializeField] public List<AudioClip> coinSounds;

}


}