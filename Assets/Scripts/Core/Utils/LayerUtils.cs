using UnityEngine;

namespace Core.Utils {
public static class LayerUtils {
    public static string Background { get; } = "Background";
    public static string Default { get; } = "Default";
    public static string Foreground { get; } = "Foreground";
}

public static class SortLayers {
    public static string Background { get; } = "Background";
    public static string Default { get; } = "Default";
    public static string Foreground { get; } = "Foreground";
}

public static class MovementUtils {
    public static Vector3 RotateVector(this Vector3 v, float degrees) {
        var sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        var cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        var tx = v.x;
        var ty = v.y;
        v.x = cos * tx - sin * ty;
        v.y = sin * tx + cos * ty;
        return v;
    }

    public static void test() {
        Vector3 vectorTest = new();
        vectorTest.RotateVector(90f);
    }
}

public static class Tags {
    public static string Monster { get; } = "Monster";
    public static string Rock { get; } = "Rock";
    public static string SandBag { get; } = "SandBag";
    public static string Wall { get; } = "Wall";
    public static string EndLine { get; } = "EndLine";
}

public static class Layers {
    public static int Default { get; } = LayerMask.NameToLayer("Default");
    public static int IgnoreCollision { get; } = LayerMask.NameToLayer("IgnoreCollision");
    public static int ArrowRock { get; } = LayerMask.NameToLayer("ArrowRock");
    public static int Monster { get; } = LayerMask.NameToLayer("Monster");
    public static int EndLine { get; } = LayerMask.NameToLayer("EndLine");
    public static int RockParty { get; } = LayerMask.NameToLayer("RockParty");
    public static int Wall { get; } = LayerMask.NameToLayer("Wall");
    public static int SandBag { get; } = LayerMask.NameToLayer("SandBag");
    public static int Fireball { get; } = LayerMask.NameToLayer("Fireball");
    public static int Stage { get; } = LayerMask.NameToLayer("Stage");
}
}