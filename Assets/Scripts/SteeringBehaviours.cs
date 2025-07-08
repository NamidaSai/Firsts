using System.Collections.Generic;
using UnityEngine;

public static class SteeringBehaviours
{
    // Use negative weight to Flee target
    public static Vector2 Seek(Rigidbody2D boid, float maxSpeed, float maxForce,
        Vector2 target)
    {
        Vector2 targetPosition = target;

        Vector2 desiredVelocity = targetPosition - boid.position;
        Vector2 desiredDirection = desiredVelocity.normalized;
        desiredVelocity = desiredDirection * maxSpeed;

        Vector2 steer = desiredVelocity - boid.linearVelocity;
        steer = Vector2.ClampMagnitude(steer, maxForce);

        return steer;
    }

    public static Vector2 Arrive(Rigidbody2D boid, float maxSpeed, float maxForce,
        Vector2 target, float slowingRadius)
    {
        Vector2 targetPosition = target;

        Vector2 desiredVelocity = targetPosition - boid.position;
        Vector2 desiredDirection = desiredVelocity.normalized;

        float distance = desiredVelocity.magnitude;
        if (distance < slowingRadius)
        {
            float arrivalSpeed = Utilities.Map(
                distance, 0, slowingRadius, 0, maxSpeed);

            desiredVelocity = desiredDirection * arrivalSpeed;
        }
        else
        {
            desiredVelocity = desiredDirection * maxSpeed;
        }

        Vector2 steer = desiredVelocity - boid.linearVelocity;
        steer = Vector2.ClampMagnitude(steer, maxForce);

        return steer;
    }

    // Use negative weight to Evade target
    public static Vector2 Pursue(Rigidbody2D boid, float maxSpeed, float maxForce,
        Rigidbody2D target, float lookAheadTime, float slowingRadius)
    {
        Vector2 targetPosition = GetFuturePosition(target, lookAheadTime);
        Vector2 steer = Arrive(boid, maxSpeed, maxForce, targetPosition, slowingRadius);

        return steer;
    }

    /// <param name="strength">Turning strength from 0f to 1f as a percentage</param>
    /// <param name="rate">Angle change per frame in degrees</param>
    public static Vector2 Wander(Rigidbody2D boid, float maxSpeed, float maxForce,
        float strength, float wanderAngle)
    {
        // hard-coded Mathf.Sqrt(2) for performance
        Vector2 circlePosition = boid.position + (Vector2)boid.transform.up * 1.41421356237f;

        Quaternion displacementRotation = Quaternion.AngleAxis(wanderAngle, Vector3.forward);
        Vector2 displacement = displacementRotation * new Vector2(0.5f, 0f);

        Vector2 targetPosition = circlePosition + displacement;

        Vector2 desiredVelocity = targetPosition - boid.position;
        Vector2 desiredDirection = desiredVelocity.normalized;
        desiredVelocity = desiredDirection * (maxSpeed * strength);

        Vector2 steer = desiredVelocity - boid.linearVelocity;
        steer = Vector2.ClampMagnitude(steer, maxForce);

        return steer;
    }

    public static Vector2 Bound(Rigidbody2D boid, float maxSpeed, float maxForce,
        Vector2 bottomLeftBoundary, Vector2 topRightBoundary)
    {
        Vector2 desiredVelocity = boid.linearVelocity;

        if (boid.position.x > topRightBoundary.x)
        {
            desiredVelocity = new Vector2(-maxSpeed, desiredVelocity.y);
        }
        else if (boid.position.x < bottomLeftBoundary.x)
        {
            desiredVelocity = new Vector2(maxSpeed, desiredVelocity.y);
        }
        else if (boid.position.y > topRightBoundary.y)
        {
            desiredVelocity = new Vector2(desiredVelocity.x, -maxSpeed);
        }
        else if (boid.position.y < bottomLeftBoundary.y)
        {
            desiredVelocity = new Vector2(desiredVelocity.x, maxSpeed);
        }

        Vector2 steer = desiredVelocity - boid.linearVelocity;
        steer = Vector2.ClampMagnitude(steer, maxForce);

        return steer;
    }

    public static Vector2 Separate(Rigidbody2D boid, float maxSpeed, float maxForce,
        HashSet<Rigidbody2D> targets, float separationDistance)
    {
        Vector2 desiredVelocity = boid.linearVelocity;

        Vector2 sum = new Vector2();
        int directionCount = 0;

        foreach (Rigidbody2D other in targets)
        {
            float distance = Vector2.Distance(boid.position, other.position);

            if (distance > Mathf.Epsilon && distance < separationDistance)
            {
                Vector2 direction = (boid.position - (Vector2)other.position).normalized;
                Vector2 weightedDirection = direction / distance;
                sum += weightedDirection;
                directionCount++;
            }
        }

        if (directionCount > 0)
        {
            sum /= directionCount;
            sum = sum.normalized;
            sum *= maxSpeed;
            desiredVelocity = sum;
        }

        Vector2 steer = desiredVelocity - boid.linearVelocity;
        steer = Vector2.ClampMagnitude(steer, maxForce);

        return steer;
    }

    public static Vector2 Align(Rigidbody2D boid, float maxSpeed, float maxForce,
        HashSet<Rigidbody2D> targets, float alignmentDistance, float fieldOfView)
    {
        Vector2 desiredVelocity = boid.linearVelocity;

        Vector2 sum = new Vector2();
        int neighborCount = 0;

        foreach (Rigidbody2D other in targets)
        {
            float distance = Vector2.Distance(boid.position, other.position);

            if (distance > Mathf.Epsilon
                && IsInFieldOfView(boid.transform, other.position, alignmentDistance, fieldOfView))
            {
                sum += other.linearVelocity;
                neighborCount++;
            }
        }

        if (neighborCount > 0)
        {
            sum /= neighborCount;
            sum = sum.normalized;
            sum *= maxSpeed;
            desiredVelocity = sum;
        }

        Vector2 steer = desiredVelocity - boid.linearVelocity;
        steer = Vector2.ClampMagnitude(steer, maxForce);

        return steer;
    }

    /// <param name="fieldOfView">in degrees</param>
    public static bool IsInFieldOfView(Transform origin, Vector2 target,
        float maxDistance, float fieldOfView)
    {
        float distance = Vector2.Distance(origin.position, target);
        if (distance > maxDistance) { return false; }

        Vector2 heading = target - (Vector2)origin.position;
        float dotProduct = Vector2.Dot(heading, origin.transform.up);

        return dotProduct > Mathf.Cos((fieldOfView * 0.5f) * Mathf.Deg2Rad);
    }

    // Use negative weight to Scatter the flock
    public static Vector2 Cohere(Rigidbody2D boid, float maxSpeed, float maxForce,
        HashSet<Rigidbody2D> targets, float cohesionDistance)
    {
        Vector2 desiredVelocity = boid.linearVelocity;

        Vector2 sum = new Vector2();
        int neighborCount = 0;

        foreach (Rigidbody2D other in targets)
        {
            float distance = Vector2.Distance(boid.position, other.position);

            if (distance > Mathf.Epsilon && distance < cohesionDistance)
            {
                sum += other.position;
                neighborCount++;
            }
        }

        if (neighborCount > 0)
        {
            sum /= neighborCount;
            desiredVelocity = Seek(boid, maxSpeed, maxForce, sum);
        }

        Vector2 steer = desiredVelocity - boid.linearVelocity;
        steer = Vector2.ClampMagnitude(steer, maxForce);

        return steer;
    }

    // Action of a strong wind or current
    public static Vector2 Push(Vector2 force)
    {
        Vector2 steer = force;
        return steer;
    }

    // public Vector2 FollowFlowField(Rigidbody2D boid, float maxSpeed, float maxForce)
    // {
    //     FlowField.FlowField flow = new FlowField.FlowField(1f,new Vector2Int(10,10));
    //     flow.InitWithPerlinNoise();
    //
    //     Vector2 desiredVelocity = flow.Lookup(boid.position);
    //     desiredVelocity *= maxSpeed;
    //
    //     Vector2 steer = desiredVelocity - boid.linearVelocity;
    //     steer = Vector2.ClampMagnitude(steer, maxForce);
    //
    //     return steer;
    // }

    public static Vector2 Combine(IEnumerable<Vector2> forces)
    {
        Vector2 result = Vector2.zero;

        foreach (Vector2 force in forces)
        {
            result += force;
        }

        return result;
    }

    private static Vector2 GetFuturePosition(Rigidbody2D boid, float timeLookAhead)
    {
        return boid.position + boid.linearVelocity * timeLookAhead;
    }
}