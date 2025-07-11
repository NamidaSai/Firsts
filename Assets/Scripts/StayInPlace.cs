using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class StayInPlace : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;

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
}

