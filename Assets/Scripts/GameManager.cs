using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    public enum GameState { Normal, Loading, GameOver }
    public enum LoseState { Shot, Arrested }


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
        if (State == GameState.Normal)
            return;

        State = GameState.Normal;
    }

    public void WinGame()
    {
        if (State == GameState.Loading)
            return;
        State = GameState.Loading;
        StartCoroutine(TemporaryWinSequence());
    }

    public void GameOver(LoseState lose)
    {
        if(State != GameState.Normal)
        { return; }

        State = GameState.GameOver;
        loseState = lose;

        StartCoroutine(TemporaryGameOverSequence());
    }

    IEnumerator TemporaryGameOverSequence()
    {
        FirstPersonController.instance.Interaction.HideCrosshair = true;
        FirstPersonController.instance.enabled = false;
        switch(loseState)
        {
            case LoseState.Shot:
                Enemy.instance.audioPlayer.Play(Enemy.instance.Weaponry[1]);
                PlayerUI.instance.LoseScreen.SetActive(true);
                
                break;
            case LoseState.Arrested:
                PlayerUI.instance.LoseCopsScreen.SetActive(true);
                break;
        }
        
        FirstPersonController.instance.DisableMovement = true;
        yield return new WaitForSeconds(1.5f);
        RestartLevel();
    }

    IEnumerator TemporaryWinSequence()
    {
        FirstPersonController.instance.Interaction.HideCrosshair = true;
        FirstPersonController.instance.enabled = false;
        PlayerUI.instance.WinScreen.SetActive(true);
        FirstPersonController.instance.DisableMovement = true;
        yield return new WaitForSeconds(1.5f);
        RestartLevel();
    }

    public void RestartLevel()
    {
        State = GameState.Loading;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        FirstPersonController.instance.DisableMovement = false;
    }
}
