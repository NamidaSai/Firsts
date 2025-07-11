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
        if (DataInstance != null)
        {
            return;
        }

        DataInstance = new PlayerData(targetColor);
    }

    private void Start()
    {
        SetAppearanceForPlayerData(DataInstance);
    }

    public void SetAppearanceForPlayerData(PlayerData playerData, float progress = 1f)
    {
        SpriteRenderer targetRenderer = spriteRenderers[(int)playerData.Shape];

        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            Color rendererColor = spriteRenderer.color;
            float rendererAlpha = spriteRenderer.color.a;

            float lerpedAlpha = Mathf.Lerp(
                rendererAlpha, 
                spriteRenderer != targetRenderer ? 0f : 1f, 
                progress
            );

            spriteRenderer.color = new Color(
                rendererColor.r, rendererColor.g, rendererColor.b, lerpedAlpha);
        }
    }
}