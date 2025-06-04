using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Palmmedia.ReportGenerator.Core.Common;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


[Serializable]
public class SerializableVector3 {
    public float x;
    public float y;
    public float z;
    public SerializableVector3(Vector3 vector) {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }
    public Vector3 ToVector3() {
        return new Vector3(x, y, z);
    }
}

[Serializable]
public class SerializableInventoryList {
    public List<SerializableInventoryItem> items = new List<SerializableInventoryItem>();
    public SerializableInventoryList(List<InventoryItem> inventoryItems) {
        foreach (InventoryItem item in inventoryItems) {
            if (item != null) {
                items.Add(new SerializableInventoryItem(item));
            }
        }
    }
    public List<InventoryItem> ToInventoryList() {
        List<InventoryItem> inventoryItems = new List<InventoryItem>();
        foreach (var item in items) {
            inventoryItems.Add(item.ToInventoryItem());
        }
        return inventoryItems;
    }
}

[Serializable] public class PlayerStats {
    public int health = 100;
    public int food = 50;
    public int water = 0;
    public SerializableVector3 position = new SerializableVector3(Vector3.zero); // Player's position in the game world
    public SerializableVector3 rotation = new SerializableVector3(Vector3.zero); // Player's rotation in the game world
    public SerializableInventoryList inventory = new SerializableInventoryList(new List<InventoryItem>()); // Player's inventory items

    public PlayerStats() { }
    public PlayerStats(int health, int food, int water, SerializableVector3 position, SerializableVector3 rotation, SerializableInventoryList inventory) {
        this.health = health;
        this.food = food;
        this.water = water;
        this.position = position;
        this.rotation = rotation;
        this.inventory = inventory;
    }
}

//[Serializable] public class PlayerInventory {
//    public List<InventoryItem> items = new List<InventoryItem>();
//}
//[Serializable] InventoryItem  {"itemName", "quantity", "category", ... (other values ???)}

[Serializable] public class GameSettings {
    public int gameMode = (int)GameMode.Survival;
    public int gameDifficulty = (int)GameDifficulty.Hard;
    public int gameVolume = 50; // 0-100
    public int creaturesVolume = 50; // 0-100
    public GameSettings() { }
    public GameSettings(int gameMode,int gameDifficulty, int gameVolume, int creaturesVolume) {
        this.gameMode = gameMode;
        this.gameDifficulty = gameDifficulty;
        this.gameVolume = gameVolume;
        this.creaturesVolume = creaturesVolume;
    }
}

[Serializable] public class GameSaveData {
    public PlayerStats playerStats = new PlayerStats();
    //public PlayerInventory playerInventory = new PlayerInventory();
    public GameSettings gameSettings = new GameSettings();
    public string saveDateTime; // for saving the date and time of the save

    public GameSaveData(PlayerStats playerStats, GameSettings gameSettings, string saveDateTime) {
        this.playerStats = playerStats;
        this.gameSettings = gameSettings;
        this.saveDateTime = saveDateTime;
    }
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
    //private int saveCount = 0;
    private string saveGameDirectory = "./SaveDataDirectory";

    // Start is called before the first frame update
    void Start()
    {
        gameLoaded = SceneManager.GetActiveScene().name == gameSceneName;
        GetAllMenuUIs();
        GetAllButtons();
    }

    private void GetAllMenuUIs() {
        mainMenuUI = transform.GetChild(0).gameObject;
        settingsMenuUI = transform.GetChild(1).gameObject;
        if (gameLoaded) {
            // save game menu 
            saveGameMenuUI = transform.GetChild(2).gameObject;
            // load volume components
            volumeGameObjects["MainVolume"] = settingsMenuUI.transform.GetChild(2).GetChild(1).gameObject;
            volumeGameObjects["MainVolume"].transform.GetChild(1).GetComponent<Slider>().onValueChanged.AddListener((value) => SetVolume(value, "MainVolume"));
            volumeGameObjects["CreatureVolume"] = settingsMenuUI.transform.GetChild(2).GetChild(2).gameObject;
            volumeGameObjects["CreatureVolume"].transform.GetChild(1).GetComponent<Slider>().onValueChanged.AddListener((value) => SetVolume(value, "CreatureVolume"));
        } else {
            // load game menu
            loadGameMenuUI = transform.GetChild(2).gameObject;
            // difficulty info
            gameModeSliderUI = settingsMenuUI.transform.GetChild(2).GetChild(1).GetChild(1).gameObject;
            gameModeSliderUI.GetComponent<Slider>().onValueChanged.AddListener((value) => SetGameMode(value));
            difficultySliderUI = settingsMenuUI.transform.GetChild(2).GetChild(2).GetChild(1).gameObject;
            difficultySliderUI.GetComponent<Slider>().onValueChanged.AddListener((value) => SetGameDifficulty(value));
        }
    }

    void SetVolume(float value,string volumeKey) {
        volumeDictionary[volumeKey] = Mathf.RoundToInt(value * 100);
        string currVolumeIconName = volumeDictionary[volumeKey] == 0 ? "mute_icon" : volumeDictionary[volumeKey] <= 50 ? "lowvolume_icon" : "highvolume_icon";
        volumeGameObjects[volumeKey].transform.GetChild(0).gameObject.GetComponent<Image>().overrideSprite = Resources.Load<Sprite>(menuManager2DIconsPath + currVolumeIconName);
    }



    private void GetAllButtons() {
        mainMenuUI.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(OnSettingsButtonClicked);
        mainMenuUI.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => OnExitButtonClicked());
        settingsMenuUI.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => SaveAndExitSettings());
        if (gameLoaded) {
            mainMenuUI.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(OnSaveGameButtonClicked);
            saveGameMenuUI.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => OnBackButtonClicked(saveGameMenuUI));
        }
        else {
            mainMenuUI.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(OnNewGameButtonClicked);
            mainMenuUI.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(OnLoadGameButtonClicked);
            loadGameMenuUI.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => OnBackButtonClicked(loadGameMenuUI));
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
        if (Directory.Exists(Path.Combine(saveGameDirectory, ""))) {
            string[] jsonFiles = Directory.GetFiles(Path.Combine(saveGameDirectory, ""), "*.json");
            CreateLoadGameComponents(jsonFiles);
        }
    }

    private void CreateLoadGameComponents(string[] jsonFiles) {
        List<string> fileNames = new List<string>();
        foreach (string file in jsonFiles) { 
            fileNames.Add(Path.GetFileNameWithoutExtension(file)); // Extracts only the file name "yyyy-mm-dd-hh-mm-ss"
        }
        if (fileNames.Count == 0) {
            return;
        }
        int maxWidth = 1800;
        int maxHeight = 700;
        int boxSize = 500;  // width = height
        int numberOfSaves = fileNames.Count;
        int maxColumns = 5;
        int maxRows = Mathf.Min(2, Mathf.CeilToInt((float)numberOfSaves / maxColumns));

        float spacingX = (float)maxWidth / maxColumns;
        float spacingY = (float)maxHeight / maxRows;

        float scaleFactorX = spacingX / boxSize;
        float scaleFactorY = spacingY / boxSize;
        float scaleFactor = Mathf.Min(scaleFactorX, scaleFactorY); // Maintain aspect ratio

        for (int i = 0; i < numberOfSaves; i++) {
            int row = i / maxColumns;
            int col = i % maxColumns;
            // Calculate total width occupied by slots
            float totalWidth = Mathf.Min(numberOfSaves, maxColumns) * spacingX;

            // Offset to center horizontally
            float offsetX = totalWidth / 2;

            float posX = col * spacingX + spacingX / 2 - offsetX;
            float posY = -row * spacingY - spacingY / 2 + maxHeight / 2;

            GameObject slot = Instantiate(Resources.Load<GameObject>(Path.Combine(menuManager2DIconsPath, "GameSlotUI")), loadGameMenuUI.transform.GetChild(2).transform);
            slot.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1); // Scale down while keeping aspect ratio
            slot.transform.localPosition = new Vector3(posX, posY, 0); // Position the slot in the grid
            UpdateSaveSlotUI(slot, i, jsonFiles[i]);
        }

    }

    private void UpdateSaveSlotUI(GameObject slot, int fileIdx, string filePath) {
        GameSaveData gameSaveData = LoadGameFromJson(filePath);
        GameObject dateTimeInfoUI = slot.transform.GetChild(0).gameObject;
        dateTimeInfoUI.GetComponentInChildren<TextMeshProUGUI>().text = gameSaveData.saveDateTime;
        GameObject gameModeInfoUI = slot.transform.GetChild(1).gameObject;
        GameMode currGameMode = (GameMode) gameSaveData.gameSettings.gameMode;
        gameModeInfoUI.transform.GetComponentInChildren<Image>().overrideSprite = Resources.Load<Sprite>(Path.Combine(menuManager2DIconsPath, currGameMode.ToString()));
        gameModeInfoUI.transform.GetComponentInChildren<TextMeshProUGUI>().text = currGameMode.ToString();
        GameObject difficultyInfoUI = slot.transform.GetChild(2).gameObject;
        GameDifficulty currDiff = (GameDifficulty)gameSaveData.gameSettings.gameDifficulty;
        difficultyInfoUI.GetComponentInChildren<Image>().overrideSprite = Resources.Load<Sprite>(Path.Combine(menuManager2DIconsPath, currDiff.ToString()));
        difficultyInfoUI.GetComponentInChildren<TextMeshProUGUI>().text = currDiff.ToString();
        GameObject loadCurrentSaveData = slot.transform.GetChild(3).gameObject;
        loadCurrentSaveData.GetComponent<Button>().onClick.AddListener(() => LoadGameViaSaveData(gameSaveData));
    }

    private void LoadGameViaSaveData(GameSaveData gameSaveData) {
        Debug.Log("Loading save..");
        Debug.Log(JsonUtility.ToJson(gameSaveData));
    }

    public void OnSaveGameButtonClicked() {
        Debug.Log("Save Game button clicked");
        // Add logic to save a game
        saveGameMenuUI.SetActive(true);
        mainMenuUI.SetActive(false);
        // Create a SaveGameData object and populate it with the current game state
        GameSaveData gameSaveData = GenerateGameSaveData();
        string fileName = $"{gameSaveData.saveDateTime}";
        string path = Path.Combine(saveGameDirectory, fileName);
        //saveCount++;
        SaveGameDataInJson(gameSaveData, path);
        SaveGameDataInBinary(gameSaveData, path);
    }

    GameSaveData GenerateGameSaveData() {
        PlayerDynamicBarsSystem playerData = PlayerDynamicBarsSystem.Instance;
        PlayerStats playerStats = new PlayerStats(
            playerData.playerCurrValues[BarType.Health],
            playerData.playerCurrValues[BarType.Food],
            playerData.playerCurrValues[BarType.Water],
            new SerializableVector3(playerData.transform.position),
            new SerializableVector3(playerData.transform.rotation.eulerAngles),
            new SerializableInventoryList(InventorySystem.Instance.inventoryItems.ToList())
        );
        GameSettings gameSettings = new GameSettings(
            (int)gameMode,
            (int)gameDifficulty,
            volumeDictionary["MainVolume"],
            volumeDictionary["CreatureVolume"]
        );
        GameSaveData saveData = new GameSaveData(
            playerStats,
            gameSettings,
            DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")
        );
        return saveData;
    }



    void SaveGameDataInBinary(GameSaveData gameSaveData, string path) {
        // Implement the logic to save the game data in binary format
        // This could involve serializing the GameSaveData object and writing it to a file
        Debug.Log("Game data saved in binary format.");
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        if (!Directory.Exists(saveGameDirectory)) {
            Directory.CreateDirectory(saveGameDirectory);
        }
        FileStream stream = new FileStream(path + ".bin", FileMode.Create);
        binaryFormatter.Serialize(stream, gameSaveData);
        stream.Close();
        Debug.Log($"Saved data in {path}.bin");
    }


    void SaveGameDataInJson(GameSaveData gameSaveData, string path) {
        // Implement the logic to save the game data in binary format
        // This could involve serializing the GameSaveData object and writing it to a file
        Debug.Log("Game data saved in binary format.");
        string jsonString = JsonUtility.ToJson(gameSaveData, true);
        Debug.Log("Serialized JSON:\n" + jsonString);
        File.WriteAllText(path+".json", jsonString);
        //GameSaveData readTest = JsonUtility.FromJson<GameSaveData>(jsonString);
        //Debug.Log("Read Successfull");
    }


    GameSaveData LoadGameDataFromBinary(string path) {
        // Implement the logic to load the game data from a binary file
        // This could involve deserializing the GameSaveData object from a file
        Debug.Log("Game data loaded from binary format.");
        if (File.Exists(path)) {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            GameSaveData gameSaveData = binaryFormatter.Deserialize(stream) as GameSaveData;
            stream.Close();
            return gameSaveData;
        }
        return null;
    }

    GameSaveData LoadGameFromJson(string directoryPath) {
        string jsonContent = File.ReadAllText(directoryPath);
        return JsonUtility.FromJson<GameSaveData>(jsonContent);
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
