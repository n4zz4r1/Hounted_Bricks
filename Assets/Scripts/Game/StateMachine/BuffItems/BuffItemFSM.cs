using System;
using Core.Utils;
using Core.Utils.Constants;
using DG.Tweening;
using Framework.Base;
using Game.Controller.Game;
using Game.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.StateMachine.BuffItems {
public class BuffItemFSM : StateMachine<BuffItemFSM, State<BuffItemFSM>> {
    public const float EffectTime = 0.3f;

    [SerializeField] public Buff buff;
    [SerializeField] public Color textColor;
    [SerializeField] public Components components;

    protected override BuffItemFSM FSM => this;
    protected override State<BuffItemFSM> GetInitialState => States.Preload;

    internal AtomicInt Counter { set; get; } = new(0);

    public static BuffItemFSM Build(GameController gameController, Buff buffId, Sprite icon, int index) {
        var gameObject = Instantiate(AssetLoader.AsGameObject(UI.BuffArea), gameController.components.gameResourceArea);
        var gameResourceFSM = gameObject.GetComponent<BuffItemFSM>();
        gameResourceFSM.Prepare(buffId, icon);

        gameObject.transform.localPosition = new Vector3(-72.5f, -531f + index * 51, 0f);
        return gameObject.GetComponent<BuffItemFSM>();
    }

    internal void PrintText() {
        components.counter.color = textColor;
    }

    public void Prepare(Buff buffId, Sprite icon) {
        // GameController = gameController;
        components.buffIcon.sprite = icon;
        buff = buffId;
        State.Prepare(FSM);
    }

    public void Increase(int value = 1) {
        FSM.State.Increase(FSM);
    }

    internal void ChangeText(int totalLeft) {
        const float fadeInSize = 1.5f;
        components.counter.text = Balancer.Instance.GetBuffText(buff);
        components.counter.rectTransform.DOKill();
        components.counter.rectTransform.localScale = new Vector3(fadeInSize, fadeInSize, fadeInSize);
        components.counter.rectTransform.DOScale(1f, EffectTime);
    }
}

[Serializable]
public class Components {
    [SerializeField] public Image buffIcon;
    [SerializeField] public TextMeshProUGUI counter;
}
}