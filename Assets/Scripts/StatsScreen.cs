using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsScreen : MonoBehaviour
{
    [SerializeField] private Text outcomeText;
    [SerializeField] private Text objectivesText;
    [SerializeField] private Text extrasText;
    private AudioSource audio;
    private float lastTimePrintedText = -420f;
    private bool PetSecretCat;
    private string Grade;

    private void Awake()
    {
        audio = GetComponent<AudioSource>();
    }

    private void Start()
    {
        SetGrade();
        SetOutcome();
        SetObjectives();
        SetExtras();
        StartCoroutine(PrintText(objectivesText));
        StartCoroutine(PrintText(extrasText));
    }

    private void Update()
    {
        if(Time.time - lastTimePrintedText < 0.25f)
        {
            if(!audio.isPlaying)
            { audio.Play(); }
        }
        else
        { audio.Stop(); }
    }

    void SetGrade()
    {
        float z;
        float total;
        z = GameManager.instance.TotalCats - GameManager.instance.CatsToPet.Count;
        total = z / GameManager.instance.TotalCats;
        //Format- Grade, New Line, Funny Message
        switch (total)
        {
            //no cats pet
            case 0:
                Grade = "F " + "\n" + "\n" + ">:( You didn't even try!";
                break;
            //One cat pet
            case 0.25f:
                Grade = "D " + "\n" + "\n" + "Just one? You can pet better than that!";
                break;
            case .5f:
                Grade = "C" + "\n" + "\n" + "There are still cats un-pet D:";
                break;
            case .75f:
                Grade = "B" + "\n" + "\n" + "Only one more! You were so close!";
                break;
            case 1:
                Grade = "A" + "\n" + "\n" + ">You're purrfect cat burglar!";
                break;
            case 1.25f:
                Grade = "S" + "\n" + "\n" + "No cat can escape from your sight!" + "\n" + "Outstanding performance!";
                break;
            default:
                Grade = "Null" + "\n" +"\n" + "Something went wrong";
                break;

        }

    }
    void SetExtras()
    {


        //add the other extras that are in! I think they are really cute!

    }

    void SetObjectives()
    {
        Debug.Log(Grade);
        objectivesText.text = "";
        objectivesText.text += $"Cats: {GameManager.instance.TotalCats - GameManager.instance.CatsToPet.Count} / {GameManager.instance.TotalCats}";
        objectivesText.text += "\n";
        objectivesText.text += "\n";
        int minutes = (int)GameManager.instance.ElapsedTime / 60;
        int seconds = (int)GameManager.instance.ElapsedTime % 60;
        objectivesText.text += $"Time: {minutes}:{seconds}";
        objectivesText.text += "\n";
        objectivesText.text += "\n";
        objectivesText.text += "FINAL SCORE: " + Grade ;
    }
    void SetOutcome()
    {
        if(GameManager.instance.PlayerWasShot)
        {
            outcomeText.text = "Lights out.  You took a cat-nap.";
            return;
        }
        if (GameManager.instance.CatsToPet.Count == 0)
        {
            outcomeText.text = "A Purrfect Crime!";
        }
        else if (GameManager.instance.CatsToPet.Count == 1 || GameManager.instance.CatsToPet.Count == 2)
        {
            outcomeText.text = "You did Meow-kay.";
        }
        else
        {
            outcomeText.text = "That was a CATastrophe...";
        }
    }
    public void QuitButton()
    {
        GameManager.instance.LoadMainMenu();
    }
    public void ReplayButton()
    {
        GameManager.instance.RestartLevel();
    }

    IEnumerator PrintText(Text text)
    {
        string finalText = text.text;
        text.text = "";
        for(int i = 0; i < finalText.Length; i++)
        {
            text.text += finalText[i];
            yield return new WaitForSeconds(0.02f);
            lastTimePrintedText = Time.time;
        }
    }
}
