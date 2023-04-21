using System;
using UnityEngine;
public class NonRepeatingSound
{
    public AudioClip[] Sounds { get; set; }

    private int lastSoundIndex;

    public NonRepeatingSound(AudioClip[] sounds)
    {
        Sounds = sounds;
    }
    public AudioClip Random()
    {
        int random = UnityEngine.Random.Range(0, Sounds.Length);

        if (lastSoundIndex == random)
        {
            random++;
            if (random >= Sounds.Length)
            { random = 0; }
        }

        lastSoundIndex = random;
        return Sounds[random];
    }
}
