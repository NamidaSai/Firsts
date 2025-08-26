using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    public void Trigger(string soundName)
    {
        if (string.IsNullOrEmpty(soundName)) { return; }
        AudioManager.Instance.Play(soundName);
    }
}