using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsScreen : MonoBehaviour
{
    [SerializeField] private Text outcomeText, objectivesText, extrasText, secretText;
    
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

        Cursor.lockState = CursorLockMode.None;
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
                Grade = "F " + "\n" + ">:( You didn't even try!";
                break;
            //One cat pet
            case 0.25f:
                Grade = "D " + "\n" + "Just one? You can pet better than that!";
                break;
            case .5f:
                Grade = "C" + "\n" + "There are still cats un-pet :/";
                break;
            case .75f:
                Grade = "B" + "\n" + "Only one more! You were so close! D:";
                break;
            case 1:
                Grade = "A" + "\n" + "You're purr-fect cat burglar! =^_^= ";
                break;
            case 1.25f:
                Grade = "S" + "\n" + "No cat can escape from your sight!" + "\n" + "Outstanding performance!";
                break;
            default:
                Grade = "Null" + "\n" + "Something went wrong";
                break;

        }

    }
    void SetExtras()
    {
        extrasText.text = "";

        if(GameManager.instance.PlayerWasInjured)
        {
            extrasText.text += "WOUNDED" + "\n" + "(granny gave you a faceful of lead)" + "\n\n";
        }

        if (!GameManager.instance.PlayerWasSpotted)
        {
            extrasText.text += "SNEAKY" + "\n" + "(you were never spotted)" + "\n\n";
        }

        if (GameManager.instance.InteractablesClicked > 30)
        {
            extrasText.text += "CURIOUS" + "\n" + "(you spent a little extra time clicking things)" + "\n\n";
        }

        if (GameManager.instance.TimesBonkedGranny > 5)
        {
            extrasText.text += "AGGRESSIVE" + "\n" + "(you bonked granny a lot)" + "\n\n";
        }

        //add the other extras that are in! I think they are really cute!

        //Ghost Cat Work:
        string GhostCatPetText;
        //if(GhostCat == Pet) {PetSecretCat = true; GhostCatPetText = "You get the feeling there's still" + "\n" + "some unfinished buisness before you leave..." }
        //else { PetSecretCat = false; "GhostCatPetText = "You finally let him rest after Granny has been" + "\n" + "trying to bring him back. His spirit thanks you!"}
        GhostCatPetText = "You get the feeling there's still" + "\n" + "some unfinished buisness before you leave...";
        secretText.text = GhostCatPetText;

        Invoke("StartFadeIn", 6f);
    }

    private void StartFadeIn()
    {
        StartCoroutine(FadeInRoutine(Color.white, 20f));
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
