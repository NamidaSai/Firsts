using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class FollowMouse : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float velocityDamping = 5f;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float maxDelta = 500f;

    private Vector2 _velocity;
    private Camera _mainCamera;
    private Rigidbody2D _rigidbody2D;

    private void Start()
    {
        _mainCamera = Camera.main;
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        UpdateVelocityFromMouseDelta();
        RotateTowardsMouse();
    }

    private void FixedUpdate()
    {
        Vector2 currentPosition = _rigidbody2D.position;
        Vector2 targetPosition = currentPosition + _velocity;

        RaycastHit2D hit = Physics2D.Linecast(currentPosition, targetPosition, obstacleLayer);
        Vector2 destination = hit.collider ? hit.point : targetPosition;

        Vector2 newPosition = Vector2.Lerp(currentPosition, destination, Time.fixedDeltaTime * moveSpeed);
        _rigidbody2D.MovePosition(newPosition);

        _velocity = Vector2.Lerp(_velocity, Vector2.zero, Time.fixedDeltaTime * velocityDamping);
    }

    private void UpdateVelocityFromMouseDelta()
    {
        Vector2 rawDelta = Mouse.current.delta.ReadValue();
        if (rawDelta == Vector2.zero) return;

        float pixelsPerUnit = Screen.height / (_mainCamera.orthographicSize * 2);
        Vector2 worldDelta = rawDelta / pixelsPerUnit;

        if (worldDelta.magnitude > maxDelta) { return; }

        _velocity += worldDelta;
    }
    
    private void RotateTowardsMouse()
    {
        Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = _mainCamera.ScreenToWorldPoint(mouseScreenPos);
        Vector2 directionToMouse = (mouseWorldPos - transform.position);

        if (directionToMouse.sqrMagnitude > 0.001f)
        {
            float angle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    } 
}
