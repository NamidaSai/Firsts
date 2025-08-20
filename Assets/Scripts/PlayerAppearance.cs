using UnityEngine;
using DG.Tweening;

[System.Serializable]
public class PlayerData
{
    public Color Color;
    public PlayerShape Shape;

    public PlayerData(Color color)
    {
        Color = color;
        Shape = PlayerShape.Circle;
    }

    public PlayerData(Color color, PlayerShape shape)
    {
        Color = color;
        Shape = shape;
    }
}

public enum PlayerShape
{
    Circle,
    Square,
    Triangle
}

public class PlayerAppearance : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] spriteRenderers;
    [SerializeField] private Color targetColor;

    public static PlayerData DataInstance;

    private void Awake()
    {
        if (gameObject.CompareTag("Player") && DataInstance == null)
        {
            DataInstance = new PlayerData(targetColor);
        }
    }

    private void Start()
    {
        if (gameObject.CompareTag("Player"))
        {
            SetAppearanceTo(DataInstance.Color);
            SetAppearanceFromTo(DataInstance.Shape, DataInstance.Shape, 1f);
        }
        else
        {
            SetAppearanceTo(targetColor);
        }
    }

    public void SetAppearanceTo(Color newColor)
    {
        bool isPlayer = gameObject.CompareTag("Player");
        int visibleIndex = isPlayer ? (int)DataInstance.Shape : 0;

        if (isPlayer)
        {
            DataInstance.Color = newColor;
        }

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            float targetAlpha = i == visibleIndex ? 1f : 0f;
            Color newTargetColor = new Color(
                newColor.r,
                newColor.g,
                newColor.b,
                targetAlpha
            );

            if (isPlayer && i == visibleIndex)
            {
                spriteRenderers[i].DOColor(newTargetColor, 1f);
                continue;
            }
            
            spriteRenderers[i].color = newTargetColor;
        }
    }

    public void SetAppearanceFromTo(PlayerShape from, PlayerShape to, float t, bool easedOut = false)
    {
        float easedT = LogEaseOut(t, 9f);
        t = easedOut ? easedT : t;
        
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            SpriteRenderer sr = spriteRenderers[i];

            float fromAlpha = i == (int)from ? 1f : 0f;
            float toAlpha = i == (int)to ? 1f : 0f;
            float lerpedAlpha = Mathf.Lerp(fromAlpha, toAlpha, t);

            Color baseColor = sr.color;
            sr.color = new Color(baseColor.r, baseColor.g, baseColor.b, lerpedAlpha);
        }

        if (t >= 1f && gameObject.CompareTag("Player"))
        {
            DataInstance.Shape = to;
        }
    }
    
    private float LogEaseOut(float t, float a = 9f)
    {
        // a > 0 controls curvature; try 4, 9, 19
        // t' = log(1 + a*t) / log(1 + a), maps [0,1] -> [0,1], concave down
        return Mathf.Log(1f + a * t) / Mathf.Log(1f + a);
    }
}