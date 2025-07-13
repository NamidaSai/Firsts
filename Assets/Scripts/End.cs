using UnityEngine;

public class End : MonoBehaviour
{
    [SerializeField] private int completeRequired = 3;

    private int _currentCompleted = 0;
    
    public void AddOneCompleted()
    {
        _currentCompleted++;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) { return; }
        if (_currentCompleted < completeRequired) { return; }
        SceneLoader.Instance.LoadNextScene();
        
        MusicPlayer musicPlayer = FindAnyObjectByType<MusicPlayer>();
        if (musicPlayer)
        {
            musicPlayer.PlayWithoutLoop("end");
        } 
    }
}
