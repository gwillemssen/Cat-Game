using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummoningCandle : Interactable
{
    [SerializeField] private Sound FireSFX, SummonSFX;
    [SerializeField] private GameObject GhostCat;

    // Start is called before the first frame update
    void Start()
    {
        transform.GetChild(0).gameObject.SetActive(false);
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
            FireSFX.Pitch = Random.Range(.95f, 1.05f);
            player.Play(FireSFX);
        }
        if(GameManager.instance.SecretCandlesLit == 5)
        {
            //Spawn Ghost Cat
            Debug.Log("Ooga Booga!! Scary cat!!!!");
            player.Play(SummonSFX);
            GhostCat.SetActive(true);
            GhostCat.transform.LeanMoveLocalY(transform.position.y + 1, 8f);
        }
    }
}
