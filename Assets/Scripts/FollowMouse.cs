using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class FollowMouse : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float velocityDamping = 5f;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float maxDelta = 500f;
    [SerializeField] private float rotationSpeed = 12f;

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
    }

    private void LookAt(Vector2 targetPosition)
    {
        Vector2 direction = (targetPosition - (Vector2)_rigidbody2D.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
        Quaternion desiredRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.fixedDeltaTime * rotationSpeed);
    }
    
    private void FixedUpdate()
    {
        Vector2 currentPosition = _rigidbody2D.position;
        Vector2 targetPosition = currentPosition + _velocity;

        RaycastHit2D hit = Physics2D.Linecast(currentPosition, targetPosition, obstacleLayer);
        Vector2 destination = hit.collider ? hit.point : targetPosition;

        Vector2 newPosition = Vector2.Lerp(currentPosition, destination, Time.fixedDeltaTime * moveSpeed);
        _rigidbody2D.MovePosition(newPosition);

        LookAt(newPosition); 
        
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
}
