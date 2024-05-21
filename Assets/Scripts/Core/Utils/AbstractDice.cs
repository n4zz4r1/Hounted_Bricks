// This is a generic implementation of dices probabilities

using System;
using System.Linq;

namespace Core.Utils {

[Serializable]
public abstract class AbstractDice<T1, T2> where T1 : AbstractDice<T1, T2>, new() {
    private static readonly Lazy<T1> LazyInstance = new(() => new T1());

    public static T1 Instance => LazyInstance.Value;

    public abstract Factor<T2>[] GetProbabilities { get; }

    // TODO improve performance
    public static T2 Roll() {
        // Get all probability
        Random random = new();
        var total = Instance.GetProbabilities.Sum(factor => factor.Percent);
        var diceResult = Math.Round(random.NextDouble() * total, 2);

        var totalSum = 0d;
        T2 result = default;

        for (var j = 0; j < Instance.GetProbabilities.Length; j++) {
            var isLast = Instance.GetProbabilities.Length == j + 1;
            totalSum += Instance.GetProbabilities[j].Percent;

            if (!isLast && !(diceResult <= totalSum)) continue;

            result = Instance.GetProbabilities[j].Value;
            break;
        }

        return result;
    }
}

public class Factor<T> {
    public Factor(T value, float percent) {
        Percent = percent;
        Value = value;
    }

    public float Percent { get; }
    public T Value { get; }
}

}