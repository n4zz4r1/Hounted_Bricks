using Core.Data;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Handler {

public class CameraTouchMovementHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    private const float ImageWidth = 2048f;
    private const float ImageHeight = 1360f;
    private const float TouchSensitivity = 1f;
    private const float Friction = 0.95f; // Used to simulate friction/deceleration
    private const float Acceleration = 0.01f; // How quickly we accelerate
    [SerializeField] public new Camera camera;

    private float _aspect;
    private bool _isTouchingTarget;
    private Vector2 _lastTouchPosition;
    private Vector2 _velocity = Vector2.zero;

    private void Awake() {
        if (Camera.main != null) _aspect = Camera.main.aspect;
    }

    private void Start() {
        var lastPosition = PlayerDataV1.Instance.GetLastPosition();
        if (lastPosition.x == 0f || lastPosition.y == 0f) return;

        camera.gameObject.transform.position = lastPosition;
    }

    private void Update() {
        if (_isTouchingTarget) {
            var touch = Input.GetTouch(0);
            var delta = (touch.position - _lastTouchPosition) * TouchSensitivity;
            // Apply acceleration
            _velocity += new Vector2(delta.x, delta.y) * Acceleration;
            _lastTouchPosition = touch.position;
        }
        else {
            // Apply friction to simulate deceleration when not touching
            _velocity *= Friction;
        }

        MoveMap(_velocity);
    }

    public void OnPointerDown(PointerEventData eventData) {
        var touch = Input.GetTouch(0);
        _lastTouchPosition = touch.position;
        _isTouchingTarget = true;
    }

    public void OnPointerUp(PointerEventData eventData) {
        _isTouchingTarget = false;
    }

    private void MoveMap(Vector2 velocity) {
        var cameraTransform = camera.gameObject.transform;
        var newPosition = cameraTransform.position - new Vector3(velocity.x, velocity.y, 0);
        var cameraSizeY = camera.orthographicSize;
        var minX = -(ImageWidth / 2f) + cameraSizeY * _aspect;
        var maxX = ImageWidth / 2f - cameraSizeY * _aspect;
        var minY = -(ImageHeight / 2f) + cameraSizeY + 30;
        var maxY = ImageHeight / 2f - cameraSizeY + 100;

        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

        cameraTransform.position = newPosition;
    }
}

}