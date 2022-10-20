using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private AudioSource audio;
    private AudioClip audioClip;
    public Sound[] sounds;

    // Start is called before the first frame update
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;

        foreach (Sound s in sounds)
        {
           s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            
        }
    }
    /// <summary>
    /// Fnds sounds by name and plays them
    /// </summary>
    /// <param name="name"></param>
    public void Play(string name)
    {
        
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("Sound file " + name + " not found!");
            return;
        }

        s.source.PlayOneShot(s.clip);

        
    }
    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("Sound file " + name + " not found!");
            return;
        }
        s.source.Stop();
    }
}
