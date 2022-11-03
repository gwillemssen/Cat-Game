using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    public Sound[] Sounds;

    private Dictionary<string, Sound> soundDictionary;
    AudioSource au;
    Sound soundToPlay;
    private void Awake()
    {
        Debug.LogWarning("disabling this for now until all the sounds are set up");
        return;

        au = GetComponent<AudioSource>();
        soundDictionary = new Dictionary<string, Sound>();
        for(int i = 0; i < Sounds.Length; i++)
        {
            soundDictionary.Add(Sounds[i].Name, Sounds[i]);
        }
    }
    public void Play(string sound)
    {
        Debug.LogWarning("disabling this for now until all the sounds are set up");
        return;

        soundToPlay = soundDictionary[sound];
        if (soundToPlay != null)
        {
            au.pitch = soundToPlay.Pitch; //Doesnt work with multiple sounds playing at the same time.  Pitch affects all currently playing sounds
            au.PlayOneShot(soundToPlay.Clip, soundToPlay.Volume);
            LevelManager.instance.MakeNoise(transform.position, soundToPlay.NoiseAmt);
        }
    }
    public void Stop()
    {
        au.Stop();
    }
}
