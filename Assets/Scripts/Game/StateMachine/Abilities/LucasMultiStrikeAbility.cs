using Core.Sprites;
using Core.Utils.Constants;

namespace Game.StateMachine.Abilities {
public class LucasMultiStrikeAbility : BuffAbilityBase<LucasMultiStrikeAbilityBuff> { }

public class LucasMultiStrikeAbilityBuff : BuffAbility {
    public override Buff Buff() {
        return Core.Utils.Constants.Buff.LucasMultiStrike;
    }

    public override BuffType BuffType() {
        return Core.Utils.Constants.BuffType.StoneLaunch;
    }

    public override CardAttributeType CardAttributeType() {
        return Core.Sprites.CardAttributeType.Probability;
    }
}
}