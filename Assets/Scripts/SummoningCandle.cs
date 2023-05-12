using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummoningCandle : Interactable
{
    [SerializeField] private AudioClip FireSFX, SummonSFX;
    [SerializeField] private GameObject GhostCat;
    private AudioSource SFXPlayer;

    // Start is called before the first frame update
    void Start()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        SFXPlayer= GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void InteractWith(FirstPersonController controller, Interactable withInteractable)
    {

        if (withInteractable.name == RequiredItemToViewInteraction && CanInteract == true)
        {
            CanInteract = false;
            transform.GetChild(0).gameObject.SetActive(true);
            GameManager.instance.SecretCandlesLit++;
            SFXPlayer.pitch = Random.Range(.9f, 1.1f);
            SFXPlayer.clip = FireSFX;
            SFXPlayer.Play();
        }
        if(GameManager.instance.SecretCandlesLit == 5)
        {
            //Spawn Ghost Cat
            SFXPlayer.pitch = 1;
            SFXPlayer.clip = SummonSFX;
            SFXPlayer.Play();
            GhostCat.SetActive(true);
            GhostCat.transform.LeanMoveLocalY(transform.position.y + .1f, 8f);
        }
    }
}