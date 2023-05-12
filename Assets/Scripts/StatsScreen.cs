using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsScreen : MonoBehaviour
{
    [SerializeField] private Text outcomeText, objectivesText, extrasText, secretText;
    
    private AudioSource audioS;
    private float lastTimePrintedText = -420f;
    private string Grade;

    private void Awake()
    {
        audioS = GetComponent<AudioSource>();
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
            if(!audioS.isPlaying)
            { audioS.Play(); }
        }
        else
        { audioS.Stop(); }

        Cursor.lockState = CursorLockMode.None;
    }

    void SetGrade()
    {
        float catsPercentage;
        catsPercentage = (float)GameManager.instance.CatsPet / (float)GameManager.instance.TotalCats;
        //Format- Grade, New Line, Funny Message
        switch (catsPercentage)
        {
            //no cats pet
            case 0:
                Grade = "F " + "\n\n" + "Not a single one?" + "\n\n" + "You've got work to do.";
                break;
            //One cat pet
            case 0.25f:
                Grade = "D " + "\n\n\n" + "Just one? You can" + "\n\n" + " pet better than that!";
                break;
            case .5f:
                Grade = "C" + "\n\n\n" + "There are still cats un-pet :/";
                break;
            case .75f:
                Grade = "B" + "\n\n\n" + "Only one more!" + "\n\n" + "You were so close! D:";
                break;
            case 1f:
                Grade = "A" + "\n\n\n" + "You're purr-fect cat burglar!" + "\n\n" + " =^_^= ";
                break;
            case 1.25f:
                Grade = "S" + "\n\n\n" + "No cat can escape your sight!" + "\n\n" + "Outstanding performance!";
                break;
            default:
                Grade = "Null" + "\n\n\n" + "Something went wrong";
                break;

        }

    }
    void SetExtras()
    {
        extrasText.text = "";

        if(GameManager.instance.PlayerWasInjured)
        {
            extrasText.text += "WOUNDED" + "\n" + "(Granny gave you a faceful of lead...)" + "\n\n";
        }

        if (!GameManager.instance.PlayerWasSpotted)
        {
            extrasText.text += "SNEAKY" + "\n" + "(You were never spotted!)" + "\n\n";
        }

        if (GameManager.instance.InteractablesClicked > 30)
        {
            extrasText.text += "CURIOUS" + "\n" + "(You spent a little extra time clicking things!)" + "\n\n";
        }

        if (GameManager.instance.TimesBonkedGranny > 5)
        {
            extrasText.text += "AGGRESSIVE" + "\n" + "(You bonked granny a lot. Oof!)" + "\n\n";
        }

        if(GameManager.instance.PettedGhostCat)
        {
            extrasText.text += "REVIVAL" + "\n" + "(You summoned the spirit of development's past!)\n\n";
        }

        string GhostCatPetText;
        if (!GameManager.instance.PettedGhostCat) 
        { 
            GhostCatPetText = "You get the feeling there's still" + "\n" + "some unfinished buisness before you leave...";
        }
        else
        {
            GhostCatPetText = "The ghost cat is at peace..."; 
        }
        secretText.text = GhostCatPetText;
        Invoke("StartFadeIn", 3f);
}

    private void StartFadeIn()
    {
        StartCoroutine(FadeInRoutine(Color.white, 20f));
    }
    void SetObjectives()
    {
        objectivesText.text = "";
        objectivesText.text += $"Cats: {GameManager.instance.CatsPet} / {GameManager.instance.TotalCats}";
        objectivesText.text += "\n\n\n";
        int minutes = (int)GameManager.instance.ElapsedTime / 60;
        int seconds = (int)GameManager.instance.ElapsedTime % 60;
        objectivesText.text += $"Time: {minutes:00}:{seconds:00}";
        objectivesText.text += "\n\n\n";
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

    private IEnumerator FadeInRoutine(Color targetColor, float duration)
    {
        // Calculate the amount to change the alpha each frame to achieve the desired duration
        float alphaChangePerFrame = Time.deltaTime / duration;

        // Gradually increase the alpha of the color until it reaches the target color
        while (secretText.color.a < targetColor.a)
        {
            Color newColor = secretText.color;
            newColor.a += alphaChangePerFrame;
            secretText.color = newColor;
            yield return null;
        }
    }

}
