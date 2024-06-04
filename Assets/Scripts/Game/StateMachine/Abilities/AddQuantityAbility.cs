using System.Linq;
using Core.Sprites;
using Core.StateMachine.Abilities;
using Core.Utils.Constants;
using Game.Controller.Game;
using Game.StateMachine.ActionButton;
using UnityEngine;

namespace Game.StateMachine.Abilities {

public class AddQuantityAbility : Ability<GameController> {

    [SerializeField] public Card card;

    // Increase based on its quantity
    protected override void InitAction() {
        GameController.ActionButtons.ToList().Find(a => a.card == card)
            .AddCounter(CardFSM.Attribute(CardAttributeType.Quantity));
        AbilityDoneCallback?.Invoke();
    }
    
}

}