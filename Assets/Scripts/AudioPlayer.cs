using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    //public Sound[] Sounds;

    //private Dictionary<string, Sound> soundDictionary;
    AudioSource au;
    Sound soundToPlay;
    private void Awake()
    {
        au = GetComponent<AudioSource>();
        au.spatialBlend = 1.0f;
        /*
        Debug.LogWarning("disabling this for now until all the sounds are set up");
        return;
        
        soundDictionary = new Dictionary<string, Sound>();
        for(int i = 0; i < Sounds.Length; i++)
        {
            soundDictionary.Add(Sounds[i].Name, Sounds[i]);
        }
        */
    }

    public void Play(Sound sound)
    {
        if(sound == null)
        {
            Debug.LogError("SOUND IS NULL");
            return;
        }
        au.spatialBlend = sound.SpatialBlend;
        au.pitch = sound.Pitch; //Doesnt work with multiple sounds playing at the same time.  Pitch affects all currently playing sounds
        au.PlayOneShot(sound.Clip, sound.Volume);
        if (sound.IncreasesNoiseMeter && sound.NoiseAmt > 0)
        { LevelManager.instance.MakeNoise(transform.position, sound.NoiseAmt); }
    }

    /*public void Play(string sound)
    {
        soundToPlay = soundDictionary[sound];
        if (soundToPlay != null)
        {
            Play(soundToPlay);
        }
    }*/

    public void Stop()
    {
        au.Stop();
    }
}
