using Core.Sprites;

namespace Core.Utils {
public abstract class GameMathUtils {
    // This method uses Fibonacci to calculate cost based on its level
    // TODO change and research when leveling maths
    public static long GenerateUpdateCostByLevel(long currentLevel) {
        var level = currentLevel + 2;
        var fib = new long[level + 1];
        fib[0] = 0;
        fib[1] = 1;
        for (long i = 2; i <= level; i++) fib[i] = fib[i - 2] + fib[i - 1];

        return fib[level];
    }

    public static long GenerateUpdateCostByLevel(long currentLevel, ResourceType resourceType) {
        var level = currentLevel + 2;
        var fib = new long[level + 1];
        fib[0] = 0;
        fib[1] = 1;
        for (long i = 2; i <= level; i++) fib[i] = fib[i - 2] + fib[i - 1];

        return fib[level];
    }
    
    // This method apply percentage in a way never reach 100 %
    public static float GetPercentageByQuantity(int percentage, int quantity)
    {
        double totalBuff = 0;
        double remainingPercentage = 100;

        for (var i = 0; i < quantity; i++)
        {
            var currentBuff = (percentage / 100.0) * remainingPercentage;
            totalBuff += currentBuff;
            remainingPercentage -= currentBuff;
        }

        return (float) totalBuff;
    }
}
}