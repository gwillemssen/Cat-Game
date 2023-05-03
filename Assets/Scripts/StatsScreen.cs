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

    private void Awake()
    {
        audio = GetComponent<AudioSource>();
    }

    private void Start()
    {
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
    void SetExtras()
    {
        int minutes = (int)GameManager.instance.ElapsedTime / 60;
        int seconds = (int)GameManager.instance.ElapsedTime % 60;
        extrasText.text = $"Time: {minutes}:{seconds}";

    }

    void SetObjectives()
    {
        objectivesText.text = "";
        objectivesText.text += $"Cats: {GameManager.instance.TotalCats - GameManager.instance.CatsToPet.Count} / {GameManager.instance.TotalCats}";
        objectivesText.text += "\n";
        objectivesText.text += GameManager.instance.FoundSecretRoom ? "Found the Secret Room" : "Didn't find the Secret Room";
        objectivesText.text += "\n";
        objectivesText.text += "\n";
        //objectivesText.text += "FINAL SCORE: " + ;
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
