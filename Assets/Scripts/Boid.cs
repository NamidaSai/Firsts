using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Boid : MonoBehaviour
{
    [SerializeField] private float seekMultiplier;
    [SerializeField] private Vector2 pushForce;
    
    [SerializeField] private float rotationSpeed = 12f;
    [SerializeField] private float startMaxSpeed = 10f;
    [SerializeField] private float startMaxForce = 10f;

    [SerializeField] private float separationDistance = 1f;
    [SerializeField] private float separateWeight = 0.5f;

    [SerializeField] private float alignmentDistance = 1f;
    [SerializeField] private float fieldOfView = 180f;
    [SerializeField] private float alignWeight = 0.2f;
    
    [SerializeField] private float cohesionDistance = 1f;
    [SerializeField] private float cohereWeight = 0.2f;

    [SerializeField] private float wanderWeight = 1.1f;
    
    private Rigidbody2D thisRigidbody;
    private float maxSpeed;
    private float maxForce;
    private ClickMorpher _morpher;
    
    private Vector2 acceleration = Vector2.zero;
    private float wanderAngle = 0f;

    private void Awake()
    {
        thisRigidbody = GetComponent<Rigidbody2D>();
        _morpher = FindAnyObjectByType<ClickMorpher>();
    }

    private void Start()
    {
        maxSpeed = startMaxSpeed;
        maxForce = startMaxForce;
    }

    private void OnEnable()
    {
        BoidManager.Instance.AddBoid(this);
    }

    private void OnDisable()
    {
        BoidManager.Instance.RemoveBoid(this);
    }

    public void Despawn()
    {
        BoidManager.Instance.ReleaseBoid(gameObject);
    }

    private void FixedUpdate()
    {
        Move();
        
        Vector2 velocityTarget = thisRigidbody.position + thisRigidbody.linearVelocity.normalized;
        LookAt(velocityTarget); 
    }

    private void Move()
    {
        CalculateAcceleration();
        ApplyAcceleration();
    }

    private void CalculateAcceleration()
    {
        acceleration += SeparationForces();
        acceleration += FlockingForces();
        acceleration += ExploringForces();
        acceleration += PushingForces();
        acceleration += SeekingForces();
    }

    private void ApplyAcceleration()
    {
        if (acceleration == Vector2.zero) { return; }

        Vector2 newVelocity = thisRigidbody.linearVelocity + acceleration;
        newVelocity = Vector2.ClampMagnitude(newVelocity, maxSpeed);
        thisRigidbody.linearVelocity = newVelocity;
        acceleration = Vector2.zero;
    }

    private void LookAt(Vector2 targetPosition)
    {
        Vector2 direction = (targetPosition - (Vector2)thisRigidbody.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
        Quaternion desiredRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.fixedDeltaTime * rotationSpeed);
    }

    private Vector2 SeparationForces()
    {
        Vector2 separateForce = SteeringBehaviours.Separate(
            thisRigidbody,
            maxSpeed,
            maxForce,
            BoidManager.Instance.AllBoidRigidbodies,
            separationDistance);
            
        separateForce *= separateWeight;

        return separateForce;
    }

    private Vector2 FlockingForces()
    {
        Vector2 alignForce = SteeringBehaviours.Align(
            thisRigidbody,
            maxSpeed,
            maxForce,
            BoidManager.Instance.AllBoidRigidbodies,
            alignmentDistance,
            fieldOfView);

        Vector2 cohereForce = SteeringBehaviours.Cohere(
            thisRigidbody,
            maxSpeed,
            maxForce,
            BoidManager.Instance.AllBoidRigidbodies,
            cohesionDistance);

        alignForce *= alignWeight;
        cohereForce *= cohereWeight;

        return alignForce + cohereForce;
    }

    private Vector2 ExploringForces()
    {
        float rate = 30f;
        wanderAngle += Random.Range(0f, 1f) * rate - rate * 0.5f;

        Vector2 wanderForce = SteeringBehaviours.Wander(
            thisRigidbody,
            maxSpeed,
            maxForce,
            1f,
            wanderAngle);

        wanderForce *= wanderWeight;

        return wanderForce;
    }

    private Vector2 PushingForces()
    {
        float maxMagnitude = pushForce.magnitude * (1f - _morpher.SeekWeight);
        Vector2 currentPush = Vector2.ClampMagnitude(pushForce, maxMagnitude);

        return SteeringBehaviours.Push(currentPush);
    }

    private Vector2 SeekingForces()
    {
        Vector2 seekForce = SteeringBehaviours.Seek(
            thisRigidbody,
            maxSpeed,
            maxForce,
            _morpher.transform.position
        );

        seekForce *= _morpher.SeekWeight * seekMultiplier;
        
        return seekForce;
    }
}