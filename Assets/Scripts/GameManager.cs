using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    public static event Action AllCatsPetted;
    public enum GameState { Normal, Loading, GameOver }
    public enum LoseState { Shot }

    [HideInInspector] public List<Cat> CatsToPet = new List<Cat>();

    public GameState State { get; private set; } = GameState.Loading;
    private LoseState loseState;


    void Awake()
    {
        if(instance != null)
        {
            Destroy(this);
            return;
        }
        else
        {
            instance = this;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        StartGame();
    }

    private void OnLevelWasLoaded(int level)
    {
        StartGame();
    }

    private void StartGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        if (State == GameState.Normal)
            return;

        State = GameState.Normal;
    }

    public void CatPetted(Cat cat)
    {
        CatsToPet.Remove(cat);
        
        if (CatsToPet.Count == 0)
        { AllCatsPetted?.Invoke(); }
    }
    public void WinGame()
    {
        if (State == GameState.Loading)
            return;
        State = GameState.Loading;
        ResetGameManager();
        StartCoroutine(TemporaryWinSequence());
    }

    public void GameOver(LoseState lose)
    {
        if(State != GameState.Normal)
        { return; }

        State = GameState.GameOver;
        loseState = lose;
        ResetGameManager();
        StartCoroutine(TemporaryGameOverSequence());
    }

    private void ResetGameManager()
    {
        CatsToPet.Clear();
    }

    public void RegisterCat(Cat cat)
    { CatsToPet.Add(cat); }

    IEnumerator TemporaryGameOverSequence()
    {
        FirstPersonController.instance.Interaction.HideCrosshair = true;
        FirstPersonController.instance.enabled = false;
        switch(loseState)
        {
            case LoseState.Shot:
                PlayerUI.instance.LoseScreen.SetActive(true);
                
                break;
        }
        
        FirstPersonController.instance.DisableMovement = true;
        yield return new WaitForSeconds(2.5f);
        RestartLevel();
    }

    IEnumerator TemporaryWinSequence()
    {
        FirstPersonController.instance.Interaction.HideCrosshair = true;
        FirstPersonController.instance.enabled = false;
        PlayerUI.instance.WinScreen.SetActive(true);
        FirstPersonController.instance.DisableMovement = true;
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("Main Menu");
    }

    public void RestartLevel()
    {
        State = GameState.Loading;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        FirstPersonController.instance.DisableMovement = false;
    }
}
