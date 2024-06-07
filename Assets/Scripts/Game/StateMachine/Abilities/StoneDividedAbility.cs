using Core.Sprites;
using Core.Utils.Constants;

namespace Game.StateMachine.Abilities {
public class StoneDividedAbility : BuffAbilityBase<StoneDividedAbilityBuff> { }

public class StoneDividedAbilityBuff : BuffAbility {
    public override Buff Buff() {
        return Core.Utils.Constants.Buff.StoneDivided;
    }

    public override BuffType BuffType() {
        return Core.Utils.Constants.BuffType.StoneCollision;
    }

    public override CardAttributeType CardAttributeType() {
        return Core.Sprites.CardAttributeType.Quantity;
    }
}
}