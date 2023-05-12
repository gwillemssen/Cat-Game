using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phone : InteractablePickup
{
    [SerializeField] private AudioClip DialTone;
    [SerializeField] private AudioSource VoicemailSource;
    [SerializeField] private List<AudioClip> Voicemails = new List<AudioClip>();
    private List<AudioClip> AnsweringMachine;
    // Start is called before the first frame update
    void Start()
    {
        AnsweringMachine = Voicemails;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Interact(FirstPersonController controller)
    {
        PlaySound(DialTone);

    }

    public override void Click()
    {

        if (!VoicemailSource.isPlaying && AnsweringMachine.Count > 0) 
        {
            player.Stop();
            int index = Random.Range(0, AnsweringMachine.Count - 1);
            VoicemailSource.clip = AnsweringMachine[index];
            VoicemailSource.Play();
            AnsweringMachine.RemoveAt(index);
        }
        //will re-add all voicemails to the answering machine if we want them to be repeatable
        //if(AnsweringMachine.Count == 0)
        //{
        //    AnsweringMachine = Voicemails;
        //}
        
    }

}
