using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; set; }

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        }
        else {
            Instance = this;
        }
    }
    // common components
    private GameObject settingsButton;
    private GameObject exitGameButton;
    // main menu components
    private GameObject newGameButton;
    // game menu components
    private GameObject loadGameButton;
    private GameObject saveGameButton;
    
    bool gameLoaded;
    private string mainMenuSceneName = "MainMenuScene";
    private string gameSceneName = "SampleScene";
    public bool isMenuOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        gameLoaded = SceneManager.GetActiveScene().name == gameSceneName;
        GetAllButtons();
    }

    private void GetAllButtons() {
        settingsButton = gameObject.transform.Find("SettingsButton").gameObject;
        settingsButton.GetComponent<Button>().onClick.AddListener(OnSettingsButtonClicked);
        exitGameButton = gameObject.transform.Find("ExitButton").gameObject;
        exitGameButton.GetComponent<Button>().onClick.AddListener(OnExitGameButtonClicked);
        if (gameLoaded) {
            saveGameButton = gameObject.transform.Find("SaveGameButton").gameObject;
            saveGameButton.GetComponent<Button>().onClick.AddListener(OnSaveGameButtonClicked);
        }
        else {
            newGameButton = gameObject.transform.Find("NewGameButton").gameObject;
            newGameButton.GetComponent<Button>().onClick.AddListener(OnNewGameButtonClicked);
            loadGameButton = gameObject.transform.Find("LoadGameButton").gameObject;
            loadGameButton.GetComponent<Button>().onClick.AddListener(OnLoadGameButtonClicked);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Only available if not gameLoaded
    /// </summary>
    public void OnNewGameButtonClicked() {
        Debug.Log("New Game button clicked");
        // Add logic to start a new game
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnLoadGameButtonClicked() {
        Debug.Log("Load Game button clicked");
        // Add logic to load a game
    }
    public void OnSaveGameButtonClicked() {
        Debug.Log("Save Game button clicked");
        // Add logic to save a game
    }
    public void OnSettingsButtonClicked() {
        Debug.Log("Settings button clicked");
        // Add logic to open settings menu
    }
    public void OnExitGameButtonClicked() {
        Debug.Log("Exit Game button clicked");
        // Add logic to exit the game
        if (gameLoaded) {
            SceneManager.LoadScene(mainMenuSceneName);
        } else {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}
