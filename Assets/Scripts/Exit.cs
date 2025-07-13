using UnityEngine;

public class Exit : MonoBehaviour
{
    [SerializeField] private Color targetColor;

    private bool _wasActivated = false;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_wasActivated) { return; }
        if (!other.CompareTag("Player")) { return; }

        _wasActivated = true;
        GameObject.FindWithTag("Player").GetComponent<PlayerAppearance>().SetAppearanceTo(targetColor);
        SceneLoader.Instance.LoadNextScene();

        MusicPlayer musicPlayer = FindAnyObjectByType<MusicPlayer>();
        if (musicPlayer)
        {
            musicPlayer.Play("resistance");
        }
    }
}
