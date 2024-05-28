using System.Collections.Generic;
using Core.Utils;
using Game.Controller.Game;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Handler {

public class DottedLineHandler : MonoBehaviour {
    // Inspector fields
    [FormerlySerializedAs("Dot")] public Sprite dot;

    [FormerlySerializedAs("Size")] [Range(0.01f, 1f)]
    public float size;

    [FormerlySerializedAs("Delta")] [Range(0.1f, 2f)]
    public float delta;

    [SerializeField] public int limit = 10;

    [SerializeField] public int reflection = 2;

    [SerializeField] public GameController gameController;
    private readonly List<GameObject> _dots = new();

    //Utility fields
    private readonly List<Vector2> _positions = new();
    private int _dotsInt;

    // Update is called once per frame
    private void FixedUpdate() {
        if (_positions.Count <= 0) return;

        DestroyAllDots();
        _positions.Clear();
    }

    private int GetLimitSize() {
        return (int)(limit * gameController.AbilityFactor.AimSizeFactor);
    }

    internal void DestroyAllDots() {
        foreach (var dotObj in _dots) Destroy(dotObj);
        _dots.Clear();
    }

    private GameObject GetOneDot() {
        var obj = new GameObject {
            transform = {
                localScale = Vector3.one * size,
                parent = transform
            }
        };

        var sr = obj.AddComponent<SpriteRenderer>();
        sr.sortingLayerName = SortLayers.Default;
        sr.sprite = dot;
        sr.sortingOrder = 2;
        return obj;
    }

    private GameObject GetOneBigDot() {
        var obj = new GameObject {
            transform = {
                localScale = Vector3.one * (size * 2),
                parent = transform
            }
        };

        var sr = obj.AddComponent<SpriteRenderer>();
        sr.sortingLayerName = SortLayers.Default;
        sr.sprite = dot;
        sr.sortingOrder = 3;
        return obj;
    }

    public void CreateDottedAim(Vector3 from, Vector3 target) {
        var difference = target - transform.position;
        var distance = difference.magnitude;
        var direction = difference / distance;
        direction.Normalize();

        var recastHit2D = MountLines(from, target, 10000).ToArray();

        DestroyAllDots();
        _dotsInt = GetLimitSize();

        for (var i = 0; i < recastHit2D.Length; i++)
            if (_dotsInt > 0)
                DrawDottedLine(recastHit2D[i].from, recastHit2D[i].target, i + 1 == recastHit2D.Length);
    }

    private void DrawDottedLine(Vector2 start, Vector2 target, bool isLast) {
        var point = start;
        var direction = (target - start).normalized;

        while ((target - start).magnitude > (point - start).magnitude) {
            _positions.Add(point);
            point += direction * delta;

            if (--_dotsInt <= 0)
                break;
        }

        Render(isLast);
    }

    private void Render(bool isLast) {
        for (var i = 0; i < _positions.Count; i++)
            if ((i + 1 == _positions.Count && isLast) || i == 0) {
                // Always set one dot at the end
                var endPoint = GetOneBigDot();
                endPoint.transform.position = _positions[i];
                _dots.Add(endPoint);
            }
            else {
                var g = GetOneDot();
                g.transform.position = _positions[i];
                _dots.Add(g);
            }
        // dots.ForEach(x => { Debug.Log(x.transform.position); });
    }


    private List<DottedLineModel> MountLines(Vector2 from, Vector2 target, int maxLength) {
        var recast = new List<DottedLineModel>();

        var difference = target - from;
        var distance = difference.magnitude;
        var direction = difference / distance;
        direction.Normalize();

        float remainingLength = maxLength;

        var ray = new Ray2D(from, direction);

        // For Each Reflection
        for (var i = 0; i < reflection; i++) {
            if (remainingLength <= 0)
                break;

            var layerMask = (1 << Layers.Monster) | (1 << Layers.Wall) | (1 << Layers.EndLine);

            var hit = Physics2D.CircleCast(ray.origin, 0.25f, ray.direction, remainingLength, layerMask);

            recast.Add(new DottedLineModel(ray.origin, hit.centroid, hit.point));
            remainingLength -= Vector2.Distance(ray.origin, hit.centroid);

            var nextDirection = Vector2.Reflect(ray.direction, hit.normal);
            var nextStartPoint = hit.centroid + nextDirection * .01f;
            ray = new Ray2D(nextStartPoint, nextDirection);
        }

        return recast;
    }
}

public class DottedLineModel {
    internal Vector3 end;
    internal Vector3 from;
    internal Vector3 target;

    public DottedLineModel(Vector3 from, Vector3 target, Vector3 end) {
        this.from = from;
        this.target = target;
        this.end = end;
    }


    public override string ToString() {
        return "From: " + from + ", Target: " + target;
    }
}

}