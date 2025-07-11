using UnityEngine;

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
    public PlayerShape CurrentShape { get; private set; }

    private void Awake()
    {
        if (DataInstance == null)
        {
            DataInstance = new PlayerData(targetColor);
        }

        CurrentShape = DataInstance.Shape;

        // Initialise sprite renderer colours with targetColor and alpha = 0
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].color = new Color(
                targetColor.r,
                targetColor.g,
                targetColor.b,
                0f
            );
        }
    }

    private void Start()
    {
        // Set the initial appearance with full alpha on the current shape
        SetAppearanceFromTo(CurrentShape, CurrentShape, 1f);
    }

    public void SetAppearanceFromTo(PlayerShape from, PlayerShape to, float t)
    {
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            SpriteRenderer sr = spriteRenderers[i];

            float fromAlpha = i == (int)from ? 1f : 0f;
            float toAlpha = i == (int)to ? 1f : 0f;
            float lerpedAlpha = Mathf.Lerp(fromAlpha, toAlpha, t);

            Color baseColor = sr.color;
            sr.color = new Color(baseColor.r, baseColor.g, baseColor.b, lerpedAlpha);
        }

        if (t >= 1f)
        {
            CurrentShape = to;
            DataInstance.Shape = to;
        }
    }
}