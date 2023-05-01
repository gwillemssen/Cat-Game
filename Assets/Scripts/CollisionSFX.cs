using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSFX : MonoBehaviour
{
    public AudioSource SFXPlayer;
    private Collider ThisCollider;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") { SFXPlayer.PlayOneShot(SFXPlayer.clip); }
    }
}
