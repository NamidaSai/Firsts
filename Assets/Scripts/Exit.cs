using UnityEngine;

public class Exit : MonoBehaviour
{
    [SerializeField] private Color targetColor;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) { return; }
        
        GameObject.FindWithTag("Player").GetComponent<PlayerAppearance>().SetAppearanceTo(targetColor);
        SceneLoader.Instance.LoadNextScene();
    }
}
