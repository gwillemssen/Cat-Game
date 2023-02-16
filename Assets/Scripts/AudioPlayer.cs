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
        au.pitch = sound.Pitch; //Doesnt work with multiple sounds playing at the same time.  Pitch affects all currently playing sounds
        au.PlayOneShot(sound.Clip, sound.Volume);
        if (sound.NoisePercentage > 0)
        { LevelManager.instance.MakeNoise(transform.position, sound.NoisePercentage); }
    }

    public void Stop()
    { au.Stop(); }
}
