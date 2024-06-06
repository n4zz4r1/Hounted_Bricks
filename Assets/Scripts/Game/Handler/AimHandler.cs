using System.Collections.Generic;
using UnityEngine;

namespace Game.Handler {
public class AimHandler : MonoBehaviour {
    // Inspector fields
    public Sprite dotSprite;
    [Range(0.01f, 1f)] public float dotSize = 0.1f;
    [Range(0.1f, 2f)] public float dotSpacing = 0.2f;
    public int reflections = 5;
    private readonly float _distance = 5f;

    private readonly List<GameObject> dots = new();
    private float _aimFactor = 1f;
    private bool isAiming;
    private Camera mainCamera;
    private LayerMask rebounceLayers;
    private Vector3 startedPosition;

    private void Awake() {
        // Define the layers that will cause rebounces
        rebounceLayers = LayerMask.GetMask("Wall", "Monster", "EndLine");
        mainCamera = Camera.main;
    }

    private void Update() {
        if (!isAiming) return;
        var mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        CreateDottedPath(startedPosition, mousePosition);
    }

    public void StartAiming(Vector3 start, float aimFactor = 1f) {
        _aimFactor = aimFactor;
        startedPosition = start;
        isAiming = true;
    }

    public void StopAiming() {
        isAiming = false;
        ClearDots();
    }

    private void CreateDottedPath(Vector3 startPosition, Vector3 targetPosition) {
        // Clear previous dots
        ClearDots();

        var direction = (targetPosition - startPosition).normalized;
        var remainingLength = _distance * _aimFactor; // Total allowed length
        var remainingReflections = 10;
        var ray = new Ray2D(startPosition, direction);

        var totalDistance = 0f; // To keep track of the total distance traveled by the dots

        while (remainingReflections >= 0 && remainingLength > 0) {
            var hit = Physics2D.CircleCast(ray.origin, 0.25f, ray.direction, rebounceLayers);

            // if (hit.collider != null)
            // {
            // Draw dots up to the hit point
            // Debug.Log($"Reflection {remainingReflections} HIT from: {currentPosition} to: {hit.point} length {remainingLength}");
            var segmentDistance = Vector2.Distance(ray.origin, hit.centroid);
            DrawDots(ray.origin, hit.centroid, totalDistance, remainingLength);
            totalDistance += segmentDistance;
            remainingLength -= segmentDistance;

            // Reflect the direction
            var nextDirection = Vector2.Reflect(ray.direction, hit.normal);
            var nextStartPoint = hit.centroid + nextDirection * .01f;
            ray = new Ray2D(nextStartPoint, nextDirection);

            remainingReflections--;

            if (hit.rigidbody != null && hit.rigidbody.CompareTag("EndLine") && remainingReflections > 0) break;
        }
    }

    private void DrawDots(Vector3 startPosition, Vector3 endPosition, float totalDistance, float remainingLength) {
        var direction = (endPosition - startPosition).normalized;
        var distance = Vector3.Distance(startPosition, endPosition);
        var currentDistance = totalDistance;

        for (float i = 0; i < distance; i += dotSpacing) {
            var position = startPosition + direction * i;
            var alpha = Mathf.Clamp01(1 - currentDistance /
                (_distance * _aimFactor)); // Calculate alpha based on total traveled distance
            CreateDot(position, alpha < 0 ? 0 : alpha);
            currentDistance += dotSpacing;
        }
    }

    private void CreateDot(Vector3 position, float alpha) {
        var dot = new GameObject("Dot") {
            transform = {
                position = position,
                localScale = Vector3.one * dotSize
            }
        };

        var sr = dot.AddComponent<SpriteRenderer>();
        sr.sprite = dotSprite;
        sr.sortingOrder = 2; // Adjust as needed for rendering order

        // Apply alpha to the dot color
        var color = _aimFactor > 1 ? Color.red : Color.white;
        color.a = alpha;
        sr.color = color;

        dots.Add(dot);
    }

    private void ClearDots() {
        foreach (var dot in dots) Destroy(dot);
        dots.Clear();
    }
}
}