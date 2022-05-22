using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public Sound[] sounds;

   void Awake()
    {
        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();

            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    void Start()
    {
        Play("MenuMusic");
    }

    public void Play(string soundName)
    {
        Sound foundSound = Array.Find(sounds, sound => sound.name == soundName);
        if (foundSound != null)
        {
            // sound found
            foundSound.source.Play();
            return;
        }

        Debug.LogWarning("Sound " + soundName + "Not Found");
    }

    public void Stop(string soundName)
    {
        Sound foundSound = Array.Find(sounds, sound => sound.name == soundName);
        if (foundSound != null)
        {
            // sound found
            foundSound.source.Stop();
            return;
        }

        Debug.LogWarning("Sound " + soundName + "Not Found");
    }
}
