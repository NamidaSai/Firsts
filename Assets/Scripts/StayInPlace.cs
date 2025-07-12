using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class StayInPlace : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float rotationSpeed = 12f;

    private Rigidbody2D _thisRigidbody;
    private Vector2 _startPosition;

    private void Awake()
    {
        _startPosition = transform.position;
        _thisRigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        MoveTowardsStartPosition();
        Vector2 velocityTarget = _thisRigidbody.position + _thisRigidbody.linearVelocity.normalized;
        LookAt(velocityTarget);  
    }

    private void MoveTowardsStartPosition()
    {
        Vector2 currentPosition = _thisRigidbody.position;
        Vector2 direction = _startPosition - currentPosition;
        float distance = direction.magnitude;

        if (distance < 0.01f)
        {
            _thisRigidbody.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 move = direction.normalized * moveSpeed;
        _thisRigidbody.linearVelocity = move;
    }
    
    private void LookAt(Vector2 targetPosition)
    {
        Vector2 direction = (targetPosition - (Vector2)_thisRigidbody.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
        Quaternion desiredRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.fixedDeltaTime * rotationSpeed);
    } 
}

