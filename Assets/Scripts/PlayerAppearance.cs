using UnityEngine;

public class PlayerData
{
    public Color Color;

    public PlayerData(Color color)
    {
        Color = color;
    }
}

public class PlayerAppearance : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color targetColor;

    private static PlayerData s_dataInstance;

    private void Awake()
    {
        if (s_dataInstance != null)
        {
            return;
        }

        s_dataInstance = new PlayerData(targetColor);
    }

    private void Start()
    {
        spriteRenderer.color = s_dataInstance.Color;
    }
}
