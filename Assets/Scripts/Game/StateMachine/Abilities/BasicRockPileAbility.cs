using Core.Handler;
using Core.StateMachine.Abilities;
using Core.Utils;
using Game.Controller.Game;
using Game.StateMachine.Monster;
using UnityEngine;

namespace Game.StateMachine.Abilities {

public class BasicRockPileAbility : Ability<GameController> {
    protected override void InitAction() =>
        AssetLoader<AbilityPanel>.LoadAsGameObject(AbilityPanel.RockPile, OnPanelOpen);

    private void OnPanelOpen(GameObject prefab) {
        GameController.HideGameUI();
        Panel = Instantiate(prefab, GameController.transform);
        
        // Set Clickable Area
        var clickableArea = Panel.GetComponentInChildren<ClickableButtonHandler>();
        clickableArea.onClickCallback = OnMove;
    }

    private void OnMove(Vector2 position) {

        GameController.AddRockPile(RockPile.Basic, new Vector2(Mathf.Floor(position.x), Mathf.Floor(position.y)));
        GameController.ShowGameUI();
        // Print Grid
        GameController.MonsterGrid.PrintGrid();
        Destroy(Panel);
        AbilityDoneCallback?.Invoke();
    }
}

}