using System.Globalization;
using TMPro;
using UnityEngine;

namespace Core.Utils {
public class FPSCounter : MonoBehaviour {
    public TextMeshProUGUI fpsDisplay; // Assign a UI Text element in the Inspector
    public TextMeshProUGUI fpsMinDisplay; // Assign a UI Text element in the Inspector
    public float startDelay = 15.0f; // Time in seconds before tracking min FPS
    private float _delayTimer;
    private float _deltaTime;
    private float _minFPS = 99f;

    private void Update() {
        _deltaTime += (Time.deltaTime - _deltaTime) * 0.1f;
        var fps = 1.0f / _deltaTime;

        _delayTimer += Time.deltaTime;
        if (_delayTimer > startDelay) // Check if the delay has elapsed
            if (fps < _minFPS)
                _minFPS = fps;

        // Display current FPS and minimum FPS after delay
        fpsDisplay.text = fps >= 69 ? "70+" : Mathf.Ceil(fps).ToString(CultureInfo.InvariantCulture);

        if (_delayTimer > startDelay)
            fpsMinDisplay.text = Mathf.Ceil(_minFPS).ToString(CultureInfo.InvariantCulture);
    }
}
}