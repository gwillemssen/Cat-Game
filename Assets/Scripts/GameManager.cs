using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    enum State { Normal, Loading }
    State state = State.Loading;

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

        //hook up win / lose events
        Enemy.CaughtPlayer += GameOver;
        ExitDoor.ExitedDoor += WinGame;
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
        if (state == State.Normal)
            return;

        state = State.Normal;
    }

    private void WinGame()
    {
        if (state == State.Loading)
            return;
        state = State.Loading;
        StartCoroutine(TemporaryWinSequence());
    }

    private void GameOver()
    {
        if (state == State.Loading)
            return;
        state = State.Loading;
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
        state = State.Loading;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        FirstPersonController.instance.DisableMovement = false;
    }
}
