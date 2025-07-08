using System.Collections;
using UnityEngine;

public static class Utilities
{
    public static IEnumerator DisableGameObjectAfterDelay(GameObject objectToDisable, float delayInSeconds)
    {
        yield return new WaitForSeconds(delayInSeconds);
        objectToDisable.SetActive(false);
    }        
        
    public static Vector3 ScreenToWorld(Camera camera, Vector3 position)
    {
        position.z = camera.nearClipPlane;
        return camera.ScreenToWorldPoint(position);
    }
    
    public static Vector2 GetDestinationFromPosition(Vector2 pos, float angle, float distance) 
    {
        float vx = Mathf.Sin(angle) * distance;
        float vy = Mathf.Cos(angle) * distance;
        
        return pos + new Vector2(vx, vy);
    }

    public static float Map(float input, float oldLow, float oldHigh, float newLow, float newHigh)
    {
        float inputPercentage = Mathf.InverseLerp(oldLow, oldHigh, input);
        float result = Mathf.Lerp(newLow, newHigh, inputPercentage);
        return result;
    }

    public static Vector2 RandomPointOnCircle(Vector2 center, float radius)
    {
        Vector2 point = center;
        float angle = Random.Range(0, 360) * Mathf.Deg2Rad;
        point.x += radius * Mathf.Cos(angle);
        point.y += radius * Mathf.Sin(angle);

        return point;
    }

    public static Vector2 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees -= eulerY;
            
        float directionX = Mathf.Sin(angleInDegrees * Mathf.Deg2Rad);
        float directionY = Mathf.Cos(angleInDegrees * Mathf.Deg2Rad);
        Vector2 direction = new Vector2(directionX, directionY);
            
        return direction;
    }
}