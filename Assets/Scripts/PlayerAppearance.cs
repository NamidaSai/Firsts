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

    private void Awake()
    {
        if (DataInstance == null)
        {
            DataInstance = new PlayerData(targetColor);
        }
    }

    private void Start()
    {
        SetAppearanceTo(DataInstance.Color);
        SetAppearanceFromTo(DataInstance.Shape, DataInstance.Shape, 1f);
    }

    public void SetAppearanceTo(Color newColor)
    {
        DataInstance.Color = newColor;
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].color = new Color(
                DataInstance.Color.r,
                DataInstance.Color.g,
                DataInstance.Color.b,
                i == (int)DataInstance.Shape ? 1f : 0f
            );
        }
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
            DataInstance.Shape = to;
        }
    }
}