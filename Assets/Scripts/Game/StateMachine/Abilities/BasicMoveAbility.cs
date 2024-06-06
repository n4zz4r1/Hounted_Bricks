using Core.Handler;
using Core.StateMachine.Abilities;
using Core.Utils;
using Game.Controller.Game;
using UnityEngine;

namespace Game.StateMachine.Abilities {
public class BasicMoveAbility : Ability<GameController> {
    protected override void InitAction() {
        AssetLoader<AbilityPanel>.LoadAsGameObject(AbilityPanel.Move, GameController, OnPanelOpen);
    }

    private void OnPanelOpen(GameObject prefab, GameController gameController) {
        GameController.HideGameUI();
        Panel = Instantiate(prefab, GameController.transform);

        // Set Clickable Area
        var clickableArea = Panel.GetComponentInChildren<ClickableButtonHandler>();
        clickableArea.onClickCallback = OnMove;
    }

    private void OnMove(Vector2 clickPosition) {
        GameController.ShowGameUI();
        GameController.NextPlayerPosition.x = clickPosition.x;
        GameController.PlayerInGame.State.Move(GameController.PlayerInGame, GameController.NextPlayerPosition.x);
        Destroy(Panel);
        AbilityDoneCallback?.Invoke();
    }
}
}