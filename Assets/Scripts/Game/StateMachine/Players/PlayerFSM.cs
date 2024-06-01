using System;
using System.Collections.Generic;
using Core.Data;
using Core.Utils.Constants;
using Framework.Base;
using Game.Controller.Game;
using Game.Handler;
using UnityEngine;

namespace Game.StateMachine.Players {

public class PlayerFSM : StateMachine<PlayerFSM, State<PlayerFSM>> {
    public static readonly Vector2 PlayerStartPosition = new(3, -0.6f);
    private static readonly Quaternion PlayerStartRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

    [SerializeField] public int lives;
    [SerializeField] public float movementSpeed = 6.0f;
    [SerializeField] public PlayerComponents components;

    internal float AimFactor { get; set; } = 1f;
    
    #region Sounds
    [SerializeField] public List<AudioClip> throwRockFX;
    #endregion

    internal GameController gameController;

    internal Vector3 nextMove;

    protected override PlayerFSM FSM => this;
    protected override State<PlayerFSM> GetInitialState => States.Created;
    //
    // public static PlayerFSM Build(string prefab, Transform parent) {
    //     var player = Instantiate(Resources.Load<PlayerFSM>(prefab), PlayerStartPosition, PlayerStartRotation,
    //         parent);
    //     return player;
    // }

    protected override void Before() {
        gameController = GetComponentInParent<GameController>();
        // components.aimLineHandler.gameController = gameController;
    }

    public static PlayerFSM GetSelected() {
        var prefab = PlayerDataV1.Instance.selectedCharacter switch {
            Card.Card_005_Char_Lucas => "P01_Lucas",
            Card.Card_006_Char_Lisa => "P02_Lisa",
            Card.Card_007_Char_Bill => "P03_Bill",
            _ => "P01_Lucas"
        };
        return Resources.Load<PlayerFSM>(prefab);
    }

    public void NextShoot() {
        gameController.NextShoot();
    }
}


[Serializable]
public class PlayerComponents {
    [SerializeField] public Animator animator;
    [SerializeField] public AimHandler aimLineHandler;
}

}