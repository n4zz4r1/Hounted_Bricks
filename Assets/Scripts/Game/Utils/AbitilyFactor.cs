namespace Game.Utils {

public class AbilityFactor {
    public int RockOnFireFactor { get; set; } = 0;
    public int MonsterOnFireFactor { get; set; } = 100; // by default, monsters will aways burn, wuawuawua

    public int RockLuckyFactor { get; set; } =
        0; // throw more than one rock at the same time. Check Rock Colider ability to see mor

    public float FireDamage { get; set; } = 5;
    public float FireDamageFactor { get; set; } = 1.1f; // Percentage of how much fire increases damage
    public bool FireWallEffect { get; set; } = false;
    public bool FireBallEffect { get; set; }

    public float EffectDamageChance1 { get; set; } = 1.0f;
    public float EffectDamageChance2 { get; set; } = 0.3f;
    public float EffectDamageChance3 { get; set; } = 0.1f;
    public float EffectDamageChance4 { get; set; } = 0f;


    public float DamageFactor { get; set; } = 1;
    public float AimSizeFactor { get; set; } = 1;

    // Effect trigger
    public bool RockPartyEffect { get; set; }
    public bool RockRainEffect { get; set; }
    public bool RockOnFire { get; set; }

    // Lisa
    public int HealFactor { get; set; } = 0;
    public bool HealEffect { get; set; } = false;
    public bool ComboFireEffect { get; set; } = false;
    public bool FireBombEffect { get; set; } = false;

    // Clear all effects

    // Billy
    public bool AcidBombEffect { get; set; } = false;
    public bool RockOnPoisonTurn { get; set; }
    public int RockOnPoisonFactor { get; set; } = 0;
    public int MonsterOnPoisonFactor { get; set; } = 100; // by default, monsters will aways get poison, wuawuawua
    public float PoisonDamageFactor { get; set; } = 1.1f; // Percentage of how much fire increases damage
    public float PoisonDamage { get; set; } = 10; // 

    internal void ClearEffects() {
        RockPartyEffect = false;
        RockRainEffect = false;
        RockOnFire = false;
        RockOnPoisonTurn = false;
        FireBallEffect = false;
    }
}

}