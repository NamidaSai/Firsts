using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private Sound[] sounds = null;

    public static AudioManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
            sound.source.volume = sound.volume;
        }
    }

    public void Play(string soundName)
    {
        Sound sound = Array.Find(sounds, soundClip => soundClip.name == soundName);

        if (sound == null)
        {
            Debug.LogWarning("Sound: " + soundName + " not found.");
            return;
        }

        if (sound.source == null) { return; }

        if (sound.source.isPlaying) { return; }

        if (sound.randomPitch)
        {
            sound.source.pitch = UnityEngine.Random.Range(sound.pitch - 0.2f, sound.pitch + 0.2f);
        }
        else
        {
            sound.source.pitch = sound.pitch;
        }

        sound.source.Play();
    }
}