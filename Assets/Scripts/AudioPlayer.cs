using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    AudioSource au;

    private void Awake()
    { au = GetComponent<AudioSource>(); }

    public void Play(Sound sound)
    {
        if(sound == null)
        {
            Debug.LogError("SOUND IS NULL");
            return;
        }
        else
        {
            au.pitch = sound.Pitch; //Doesnt work with multiple sounds playing at the same time.  Pitch affects all currently playing sounds
            au.PlayOneShot(sound.Clip, sound.Volume);
        }
    }

    public void Play(AudioClip sound)
    {
        if (sound == null)
        {
            Debug.LogError("SOUND IS NULL");
            return;
        }
        else
        {
            au.PlayOneShot(sound, 1f);
        }
    }

    public void Stop()
    { au.Stop(); }
}
