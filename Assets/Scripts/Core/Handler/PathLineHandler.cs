using System.Collections.Generic;
using Core.Utils;
using Core.Utils.Constants;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Handler {

public class PathLineHandler : MonoBehaviour {
    private const int Limit = 1000;

    // Inspector fields
    [FormerlySerializedAs("Dot")] public Sprite dot;

    [FormerlySerializedAs("Size")] [Range(0.01f, 80f)]
    public float size;

    [FormerlySerializedAs("Delta")] [Range(0.1f, 80f)]
    public float delta;

    private readonly List<GameObject> _dots = new();

    //Utility fields
    private readonly List<Vector2> _positions = new();
    public float AimSizeFactor { get; set; } = 1;

    internal void DestroyAllDots() {
        foreach (var dotGameObject in _dots) Destroy(dotGameObject);
        _dots.Clear();
    }

    private GameObject CreateDot() {
        GameObject createDot = new() {
            transform = {
                localScale = Vector3.one * size,
                parent = transform
            }
        };

        var sr = createDot.AddComponent<SpriteRenderer>();
        sr.sortingLayerName = LayerUtils.Default;
        sr.sprite = dot;
        sr.color = Colors.DISABLED_ALPHA;
        sr.sortingOrder = 5;
        return createDot;
    }

    public void DrawDottedLine(Vector2 start, Vector2 target) {
        var point = start;
        var dotLimit = Limit;
        var direction = (target - start).normalized;

        while ((target - start).magnitude > (point - start).magnitude) {
            _positions.Add(new Vector3(point.x, point.y, 0));
            point += direction * delta;
            if (--dotLimit <= 0) break;
        }

        foreach (var t in _positions) {
            var g = CreateDot();
            g.transform.position = t;
            _dots.Add(g);
        }
    }
}

}