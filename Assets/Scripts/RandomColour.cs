using UnityEngine;

public class RandomColour : MonoBehaviour
{
    [SerializeField] private Color[] allColors;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer.color = allColors[Random.Range(0, allColors.Length)];
    }
}
