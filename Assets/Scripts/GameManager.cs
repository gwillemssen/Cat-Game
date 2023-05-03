using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    public event Action OnAllCatsPetted;
    public event Action OnCatPetted;
    [HideInInspector] public List<Cat> CatsToPet = new List<Cat>();
    public int TotalCats { get; private set; }
    public enum GameState { TitleScreen, InGame, Loading, GameOver }
    public GameState State { get; private set; } = GameState.Loading;
    [SerializeField] private Texture2D loadingTexture;

    public bool PlayerWasShot { get; private set; } = false;
    public bool PlayerWasInjured { get; private set; } = false;
    [HideInInspector] public bool FoundSecretRoom = false;
    [HideInInspector] public double ElapsedTime;

    [HideInInspector] public int SecretCandlesLit;

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            RestartLevel();
        }

        if(State == GameState.InGame)
        {
            ElapsedTime += Time.deltaTime;
            //print("test");
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        StartGame();
    }

    private void StartGame()
    {
        if(SceneManager.GetActiveScene().name == "Main Menu")
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            State = GameState.TitleScreen;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            State = GameState.InGame;
        }
    }

    public void CatPetted(Cat cat)
    {
        CatsToPet.Remove(cat);
        OnCatPetted?.Invoke();

        if (CatsToPet.Count == 0)
        { OnAllCatsPetted?.Invoke(); }
    }
    

    private void ResetGameManager()
    {
        CatsToPet.Clear();
        FoundSecretRoom = false;
        ElapsedTime = 0f;
        PlayerWasInjured = false;
        PlayerWasShot = false;
    }

    public void RegisterCat(Cat cat)
    {
        CatsToPet.Add(cat);
        TotalCats = CatsToPet.Count;
    }

    /*IEnumerator TemporaryGameOverSequence()
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

    IEnumerator TemporaryWinSequence(bool pettedAllCats)
    {
        FirstPersonController.instance.Interaction.HideCrosshair = true;
        FirstPersonController.instance.enabled = false;

        if (pettedAllCats)
        { PlayerUI.instance.WinScreen.SetActive(true); }
        else
        { PlayerUI.instance.NotAllOfTheCatsScreen.SetActive(true); }

        FirstPersonController.instance.DisableMovement = true;
        yield return new WaitForSeconds(2.5f);
        LoadMainMenu();
    }*/

    public void GameOver()
    {
        Enemy.instance.enabled = false;
        PlayerWasShot = FirstPersonController.instance.WasShot;
        PlayerUI.instance.StatsScreen.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void ExitDoor()
    {
        GameOver();
    }

    public void PlayerShot()
    {
        PlayerWasShot = true;
        GameOver();
    }

    public void LoadMainMenu()
    { LoadScene("Main Menu"); }

    public void RestartLevel()
    {
        LoadScene(SceneManager.GetActiveScene().name);
        //FirstPersonController.instance.DisableMovement = false;
    }

    public void LoadScene(string sceneName)
    {
        State = GameState.Loading;
        SceneManager.LoadScene(sceneName);
    }

    private void OnGUI()
    {
        if (State == GameState.Loading)
        { GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), loadingTexture); }
    }
}
