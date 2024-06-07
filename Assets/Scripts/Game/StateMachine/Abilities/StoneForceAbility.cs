using Core.Sprites;
using Core.Utils.Constants;

namespace Game.StateMachine.Abilities {
public class StoneForceAbility : BuffAbilityBase<StoneForceAbilityBuff> { }

public class StoneForceAbilityBuff : BuffAbility {
    public override Buff Buff() {
        return Core.Utils.Constants.Buff.PlayerDamage;
    }

    public override BuffType BuffType() {
        return Core.Utils.Constants.BuffType.StoneCollision;
    }

    public override CardAttributeType CardAttributeType() {
        return Core.Sprites.CardAttributeType.Probability;
    }
}
}