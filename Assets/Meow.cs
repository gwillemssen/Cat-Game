using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Meow : MonoBehaviour
{
    public static Meow instance; 
    //Hose and Play the Cat AudioSources HERE
    public AudioClip[] Catsounds;
    AudioSource source;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    public void PlayMeow()
    {
        source.PlayOneShot(Catsounds[Random.Range(0, Catsounds.Length)]);
    }
}
