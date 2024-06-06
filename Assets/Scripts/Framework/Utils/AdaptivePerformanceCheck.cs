using UnityEngine;

// using UnityEngine.AdaptivePerformance;

namespace Framework.Utils {
public class AdaptivePerformanceCheck : MonoBehaviour {
    // private void Start()
    // {
    //     // Ensure Adaptive Performance is initialized
    //     var ap = Holder.Instance;
    //     if (ap == null || !ap.Active)
    //     {
    //         Debug.LogError("Adaptive Performance is not active or not supported on this device.");
    //         return;
    //     }
    //
    //     // Subscribe to performance status updates
    //     ap.PerformanceStatus.PerformanceBottleneckChangeEvent += OnBottleneckChange;
    // }
    //
    // private static void OnBottleneckChange(PerformanceBottleneckChangeEventArgs args)
    // {
    //     switch (args.PerformanceBottleneck)
    //     {
    //         case PerformanceBottleneck.CPU:
    //             Debug.Log("CPU is the bottleneck. Consider reducing CPU load.");
    //             // Implement CPU load reduction strategies here
    //             break;
    //         case PerformanceBottleneck.GPU:
    //             Debug.Log("GPU is the bottleneck. Consider reducing GPU load.");
    //             // Implement GPU load reduction strategies here
    //             break;
    //         case PerformanceBottleneck.TargetFrameRate:
    //             Debug.Log("Target frame rate is the bottleneck. Consider reducing frame rate.");
    //             // Implement frame rate reduction strategies here
    //             break;
    //         case PerformanceBottleneck.Unknown:
    //             Debug.Log("No bottlenecks detected.");
    //             break;
    //         default:
    //             throw new ArgumentOutOfRangeException();
    //     }
    // }
}
}