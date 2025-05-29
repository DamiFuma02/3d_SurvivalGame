using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public enum BarType {
    Health, Food, Water
}

public class PlayerDynamicBarsSystem : MonoBehaviour
{

    public static PlayerDynamicBarsSystem Instance { get; set; }

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        }
        else {
            Instance = this;
        }
    }

    
    public Dictionary<BarType, int> playerMaxValues = new Dictionary<BarType, int> {
        { BarType.Health, 1000 },
        { BarType.Food, 1000 },
        { BarType.Water, 1000 }
    };
    public Dictionary<BarType, int> playerCurrValues = new Dictionary<BarType, int> {
        { BarType.Health, 1000 },
        { BarType.Food, 1000 },
        { BarType.Water, 1000 }
    };
    public bool isPlayerDead;

    public GameObject playerDynamicBarsUI;
    private Dictionary<string,List<object>> playerDynamicBarsUIComponents = new Dictionary<string, List<object>> {
        { "Slider", new List<object>() },
        { "Image", new List<object>() },
        { "Text", new List<object>() }
    };

    public GameObject lookingObjectDynamicBarUICanvas;
    private Dictionary<string, object> lookingObjectDynamicBarUIComponents = new Dictionary<string, object> {
        { "Slider", null },
        { "Image", null },
        { "Text", null }
    };

    List<Color32[]> dynamicBarsColors = new List<Color32[]> {
        //              maxColor,                           mediumColor,                     lowColor
        new Color32[] { new Color32(23, 132, 18, 255), new Color32(166, 137, 31, 255), new Color32(255, 0, 0, 255) }, // health bar colors
        new Color32[] { new Color32(168, 120, 9, 255) }, // food bar colors
        new Color32[] { new Color32(0,139,255,255)} // water bar colors    
    };


    // Start is called before the first frame update
    void Start() {
        GetDynamicBarsComponents();
        UpdatePlayerBarsUI();
    }

    private void GetDynamicBarsComponents() {
        foreach (Transform child in playerDynamicBarsUI.transform) {
            if (child.gameObject.CompareTag("DynamicBar")) {
                playerDynamicBarsUIComponents["Slider"].Add(child.GetComponent<Slider>());
                playerDynamicBarsUIComponents["Image"].Add(child.Find("ColorBar").gameObject.GetComponent<Image>());
                playerDynamicBarsUIComponents["Text"].Add(child.Find("BarValues").gameObject.GetComponent<TextMeshProUGUI>());
            }
        }
        
        lookingObjectDynamicBarUIComponents["Slider"] = lookingObjectDynamicBarUICanvas.GetComponent<Slider>();
        lookingObjectDynamicBarUIComponents["Image"] = lookingObjectDynamicBarUICanvas.transform.Find("ColorBar").gameObject.GetComponent<Image>();
        lookingObjectDynamicBarUIComponents["Text"] = lookingObjectDynamicBarUICanvas.transform.Find("BarValues").gameObject.GetComponent<TextMeshProUGUI>();

        Assert.AreEqual(playerDynamicBarsUIComponents["Slider"].Count,
            Enum.GetValues(typeof(BarType)).Length,
            $"The number of bars {playerDynamicBarsUIComponents["Slider"].Count} must be equal to the number of bar types {string.Join(", ", Enum.GetNames(typeof(BarType)))}"
        );

    }

    
    

    public void ConsumeItem(InventoryItem inventoryItem) {
        if (inventoryItem.categoryProperties == null && inventoryItem.categoryProperties.Count == 0) {
            return;
        }
    }



    public void UpdatePlayerBarsUI() {
        foreach (BarType barType in Enum.GetValues(typeof(BarType))) {
            UpdateBarUI(barType, playerCurrValues[barType], playerMaxValues[barType], player:true );
        }
    }

    

    public void UpdateBarUI(BarType barType, int currValue, int maxValue, bool player=true) {
        Slider currSlider;
        Image currImage;
        TextMeshProUGUI currText;

        if (player) {
            currSlider = (Slider)playerDynamicBarsUIComponents["Slider"][(int)barType];
            currImage = (Image)playerDynamicBarsUIComponents["Image"][(int)barType];
            currText = (TextMeshProUGUI)playerDynamicBarsUIComponents["Text"][(int)barType];
        } else {
            currSlider = (Slider)lookingObjectDynamicBarUIComponents["Slider"];
            currImage = (Image)lookingObjectDynamicBarUIComponents["Image"];
            currText = (TextMeshProUGUI)lookingObjectDynamicBarUIComponents["Text"];
        }
        currSlider.value = (float)currValue / maxValue;
        currText.text = $"{currValue}/{maxValue}";

        int numberOfColors = dynamicBarsColors[(int)barType].Length;
        if (numberOfColors == 1) {
            currImage.color = dynamicBarsColors[(int)barType][0];
        }
        else {
            // divide the range values from 0 to maxValue into numberOfColors equal parts
            float range = maxValue / numberOfColors;
            for (int i = 0; i < numberOfColors; i++) {
                if (currValue >= range * (numberOfColors - i-1)) {
                    currImage.color = dynamicBarsColors[(int)barType][i];
                    break;
                }
            }
        }
    }
}
