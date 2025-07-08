using UnityEngine;

[System.Serializable]
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

    public void SetAppearanceForPlayerData(PlayerData playerData)
    {
        spriteRenderer.color = playerData.Color;
    }
}
