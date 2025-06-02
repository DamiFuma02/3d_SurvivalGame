using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



[Serializable] public class PlayerStats {
    public int health = 100;
    public int food = 50;
    public int water = 0;
}

//[Serializable] public class PlayerInventory {
//    public List<InventoryItem> items = new List<InventoryItem>();
//}
//[Serializable] InventoryItem  {"itemName", "quantity", "category", ... (other values ???)}

[Serializable] public class GameSettings {
    public bool survivalMode = true;
    public int gameVolume = 50; // 0-100
    public int creaturesVolume = 50; // 0-100
}

[Serializable] public class GameSaveData {
    public PlayerStats playerStats = new PlayerStats();
    //public PlayerInventory playerInventory = new PlayerInventory();
    public GameSettings gameSettings = new GameSettings();
    public string saveDateTime; // for saving the date and time of the save
}


public enum GameMode { Survival, Creative}

public enum GameDifficulty { Easy, Normal, Hard }


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
    GameObject gameModeSliderUI;
    public GameMode gameMode = GameMode.Survival;
    GameObject difficultySliderUI;
    public GameDifficulty gameDifficulty = GameDifficulty.Easy;
    // game menu components
    public Dictionary<string, GameObject> volumeGameObjects = new Dictionary<string, GameObject> {
        { "MainVolume", null },
        { "CreatureVolume", null }
    };
    public Dictionary<string, int> volumeDictionary = new Dictionary<string, int> {
        { "MainVolume", 50 }, // 0-100
        { "CreatureVolume", 50 } // 0-100
    };
    
    private GameObject saveGameMenuUI;
    
    bool gameLoaded;
    private string mainMenuSceneName = "MainMenuScene";
    private string gameSceneName = "SampleScene";
    public bool isMenuOpen = false;
    private string menuManager2DIconsPath = "2D_Prefabs/MenuManager/";

    // Start is called before the first frame update
    void Start()
    {
        gameLoaded = SceneManager.GetActiveScene().name == gameSceneName;
        GetAllMenuUIs();
        GetAllButtons();
    }

    private void GetAllMenuUIs() {
        mainMenuUI = transform.Find("MainMenuUI").gameObject;
        settingsMenuUI = transform.GetChild(2).gameObject;
        if (gameLoaded) {
            volumeGameObjects["MainVolume"] = settingsMenuUI.transform.GetChild(1).GetChild(1).gameObject;
            volumeGameObjects["MainVolume"].transform.GetChild(1).GetComponent<Slider>().onValueChanged.AddListener((value) => SetVolume(value, "MainVolume"));
            volumeGameObjects["CreatureVolume"] = settingsMenuUI.transform.GetChild(1).GetChild(2).gameObject;
            volumeGameObjects["CreatureVolume"].transform.GetChild(1).GetComponent<Slider>().onValueChanged.AddListener((value) => SetVolume(value, "CreatureVolume"));
            saveGameMenuUI = transform.GetChild(1).gameObject;
        } else {
            gameModeSliderUI = settingsMenuUI.transform.GetChild(1).GetChild(1).GetChild(1).gameObject;
            gameModeSliderUI.GetComponent<Slider>().onValueChanged.AddListener((value) => SetGameMode(value));
            difficultySliderUI = settingsMenuUI.transform.GetChild(1).GetChild(2).GetChild(1).gameObject;
            difficultySliderUI.GetComponent<Slider>().onValueChanged.AddListener((value) => SetGameDifficulty(value));
            loadGameMenuUI = transform.GetChild(1).gameObject;
        }
    }

    void SetVolume(float value,string volumeKey) {
        volumeDictionary[volumeKey] = Mathf.RoundToInt(value * 100);
        string currVolumeIconName = volumeDictionary[volumeKey] == 0 ? "mute_icon" : volumeDictionary[volumeKey] <= 50 ? "lowvolume_icon" : "highvolume_icon";
        volumeGameObjects[volumeKey].transform.GetChild(0).gameObject.GetComponent<Image>().overrideSprite = Resources.Load<Sprite>(menuManager2DIconsPath + currVolumeIconName);
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

    


    void SetGameDifficulty(float value) {
        gameDifficulty = (GameDifficulty)Mathf.RoundToInt(value);
    }


    void SetGameMode(float value) {
        gameMode = (GameMode)Mathf.RoundToInt(value);
        Debug.Log("Game Mode changed to: " + gameMode.ToString());
        gameModeSliderUI.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = gameMode.ToString();
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
        // Create a SaveGameData object and populate it with the current game state
        GameSaveData saveData = new GameSaveData();
        saveData.saveDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
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
