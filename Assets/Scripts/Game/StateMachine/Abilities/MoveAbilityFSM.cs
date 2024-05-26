using Core.Handler;
using UnityEngine;

namespace Game.StateMachine.Abilities {

public class MoveAbilityFSM : AbilityFSM {
    private GameObject panel;
    protected override AbilityFSM FSM => this;

    protected override void Execute() {
        var moveAbilityPanel = Resources.Load("MoveAbilityPanel") as GameObject;
        panel = Instantiate(moveAbilityPanel, GameController.gameObject.transform);
        var clickableArea = panel.GetComponentInChildren<ClickableButtonHandler>();
        clickableArea.onClickCallback = Move;
    }

    private void Move(Vector2 clickPosition) {
        GameController.State.Move(GameController, clickPosition.x);

        Debug.Log("Current Player Position: " + GameController.nextPlayerPosition);
        Debug.Log("going to X: " + clickPosition.x);
        GameController.nextPlayerPosition.x = clickPosition.x;
        GameController.playerInGame.State.Move(GameController.playerInGame, GameController.nextPlayerPosition.x);
        Destroy(panel);
        CallBack(ActionButtonFSM, true);
    }
}

}