using Core.Controller.Audio;
using Core.Handler;
using Core.StateMachine.Abilities;
using Core.Utils;
using Core.Utils.Constants;
using DG.Tweening;
using Framework.Base;
using Game.Controller.Game;
using Game.StateMachine.AbilityPanels;
using Game.StateMachine.Monster;
using Game.StateMachine.Rocks;
using Game.Utils;
using UnityEngine;

namespace Game.StateMachine.Abilities {
public class BasicLucasAbility : AbilityPanelBase {
    private GameObject _axe;
    private MonsterFSM _monsterFSM;
    private readonly AtomicInt _actions = new AtomicInt(1);
    private const float Duration = 0.6f;

    protected override void InitAction() {
        AssetLoader<AbilityPanel>.LoadAsGameObject(AbilityPanel.LucasBasic, GameController, OnPanelOpen);
    }

    private void OnPanelOpen(GameObject prefab, GameController gameController) {
        _actions.Value = 1;
        GameController.HideGameUI();
        Panel = Instantiate(prefab, GameController.transform);

        // Set Clickable Area
        var clickableArea = Panel.GetComponentInChildren<ClickableButtonHandler>();
        clickableArea.onClickCallback = ThrownAxe;
    }

    private void ThrownAxe(Vector2 position) {
        if (--_actions.Value < 0) return;

        Destroy(Panel);
        _monsterFSM = GameController.MonsterAtPosition(new Vector2(Mathf.Floor(position.x), Mathf.Floor(position.y)));

        if (_monsterFSM == null) {
            Destroy(Panel);
            GameController.ShowGameUI();
            AbilityCanceledCallback?.Invoke();
            return;
        }
        AudioController.PlayFX(CommonFX.ThrownHigh);
        
        var center = _monsterFSM.monsterType == MonsterType.BOSS ? 1f : 0.5f;
        var target = new Vector2(Mathf.Floor(_monsterFSM.transform.position.x) + center, Mathf.Floor(_monsterFSM.transform.position.y) + center);

        _axe = Instantiate(AssetLoader.AsGameObject(Rock.Axe), GameController.PlayerInGame.transform);
        // Make the axe spin
        _axe.transform.DORotate(new Vector3(0, 0, 360 * 1.5f), Duration, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);
        _axe.transform.DOMove(target, Duration).OnComplete(ThrownComplete);
    }

    private void ThrownComplete() {
        // This effect kills normal monster, else make half monster's damage. Boss and Shielded takes regular damage.
        if (_monsterFSM.monsterType is MonsterType.NORMAL or MonsterType.FAST or MonsterType.CORNER)
            _monsterFSM.State.Kill(_monsterFSM);
        else if (_monsterFSM.monsterType is MonsterType.MAGE or MonsterType.SHAMAN)
            _monsterFSM.State.Hit(_monsterFSM, _monsterFSM.GetLife());
        else {
            if (_monsterFSM.monsterType == MonsterType.SHIELDED) 
                AudioController.PlayFX(CommonFX.ImpactMetal);
            var rock = AssetLoader.AsComponent<RockFSM>(Rock.Crooked);
            _monsterFSM.State.Hit(_monsterFSM, rock.rockDamage * GameController.AbilityFactor.DamageFactor);
        }
        
        _axe.transform.DOKill();
        Destroy(_axe); 
        GameController.ShowGameUI();
        AbilityDoneCallback?.Invoke();
    }
}
}