using System;
using UnityEngine;
public class NonRepeatingSound
{
    public Sound[] Sounds { get; set; }

    private int lastSoundIndex;

    public NonRepeatingSound(Sound[] sounds)
    {
        Sounds = sounds;
    }
    public Sound Random()
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
