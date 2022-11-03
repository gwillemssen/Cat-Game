using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    public enum GameState { Normal, Loading, CopsCalled }

    public GameState State { get; private set; }

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

    public void CallCops()
    {
        //TODO: start timer
        State = GameState.CopsCalled;
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

    public void GameOver()
    {
        if (State == GameState.Loading)
            return;
        State = GameState.Loading;
        StartCoroutine(TemporaryGameOverSequence());
    }

    IEnumerator TemporaryGameOverSequence()
    {
        FirstPersonController.instance.Interaction.HideCrosshair = true;
        FirstPersonController.instance.enabled = false;
        FirstPersonController.instance.UI.LoseScreen.SetActive(true);
        FirstPersonController.instance.DisableMovement = true;
        yield return new WaitForSeconds(1.5f);
        RestartLevel();
    }

    IEnumerator TemporaryWinSequence()
    {
        FirstPersonController.instance.Interaction.HideCrosshair = true;
        FirstPersonController.instance.enabled = false;
        FirstPersonController.instance.UI.WinScreen.SetActive(true);
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
