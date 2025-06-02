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
    private GameObject mainMenuUI;
    private GameObject settingsMenuUI;
    // main menu components
    private GameObject loadGameMenuUI;
    // game menu components
    private GameObject saveGameMenuUI;
    
    bool gameLoaded;
    private string mainMenuSceneName = "MainMenuScene";
    private string gameSceneName = "SampleScene";
    public bool isMenuOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        gameLoaded = SceneManager.GetActiveScene().name == gameSceneName;
        GetAllMenuUIs();
        GetAllButtons();
    }

    private void GetAllMenuUIs() {
        mainMenuUI = transform.Find("MainMenuUI").gameObject;
        settingsMenuUI = transform.Find("SettingsUI").gameObject;
        if (gameLoaded) {
            saveGameMenuUI = transform.Find("SaveGameUI").gameObject;
        } else {
            loadGameMenuUI = transform.Find("LoadGameUI").gameObject;
        }
    }

    private void GetAllButtons() {
        mainMenuUI.transform.Find("SettingsButton").GetComponent<Button>().onClick.AddListener(OnSettingsButtonClicked);
        settingsMenuUI.transform.Find("ExitButton").GetComponent<Button>().onClick.AddListener(() => SaveAndExitSettings());
        mainMenuUI.transform.Find("ExitButton").GetComponent<Button>().onClick.AddListener(() => OnExitButtonClicked());
        if (gameLoaded) {
            mainMenuUI.transform.Find("SaveGameButton").GetComponent<Button>().onClick.AddListener(OnSaveGameButtonClicked);
            saveGameMenuUI.transform.Find("ExitButton").GetComponent<Button>().onClick.AddListener(() => OnBackButtonClicked(saveGameMenuUI));
        }
        else {
            mainMenuUI.transform.Find("LoadGameButton").GetComponent<Button>().onClick.AddListener(OnLoadGameButtonClicked);
            loadGameMenuUI.transform.Find("ExitButton").GetComponent<Button>().onClick.AddListener(() => OnBackButtonClicked(loadGameMenuUI));
            mainMenuUI.transform.Find("NewGameButton").GetComponent<Button>().onClick.AddListener(OnNewGameButtonClicked);
        }

    }

    

    private void SaveAndExitSettings() {
        if (gameLoaded) {
            
        } else {

        }
        settingsMenuUI.SetActive(false);
        mainMenuUI.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Only available if not gameLoaded
    /// </summary>
    public void OnNewGameButtonClicked() {
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnLoadGameButtonClicked() {
        Debug.Log("Load Game button clicked");
        // Add logic to load a game
        loadGameMenuUI.SetActive(true);
        mainMenuUI.SetActive(false);
    }
    public void OnSaveGameButtonClicked() {
        Debug.Log("Save Game button clicked");
        // Add logic to save a game
        saveGameMenuUI.SetActive(true);
        mainMenuUI.SetActive(false);
    }
    public void OnSettingsButtonClicked() {
        Debug.Log("Settings button clicked");
        // Add logic to open settings menu
        settingsMenuUI.SetActive(true);
        mainMenuUI.SetActive(false);
    }
    private void OnExitButtonClicked() {
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

    public void OnBackButtonClicked(GameObject currentMenuUI) {
        currentMenuUI.SetActive(false);
        mainMenuUI.SetActive(true);
    }
}
