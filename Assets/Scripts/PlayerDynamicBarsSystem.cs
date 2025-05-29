using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    private enum BarType {
        Health,Food,Water
    }
    public int[] maxValues = { 1000,1000,1000 };
    public int[] currValues = { 1000, 1000, 1000 };
    public bool isDead;

    public GameObject dynamicBarsUI;
    List<Slider> dynamicBarsSliders = new List<Slider>();
    List<Image> dynamicBarsImg = new List<Image>();
    List<TextMeshProUGUI> dynamicBarsStatus = new List<TextMeshProUGUI>();

    List<Color32[]> dynamicBarsColors = new List<Color32[]> {
        //              maxColor,                           mediumColor,                     lowColor
        new Color32[] { new Color32(23, 132, 18, 255), new Color32(166, 137, 31, 255), new Color32(255, 0, 0, 255) }, // health bar colors
        new Color32[] { new Color32(168, 120, 9, 255) }, // food bar colors
        new Color32[] { new Color32(0,139,255,255)} // water bar colors    
    };



    List<Coroutine> coroutines = new List<Coroutine>();

    // Start is called before the first frame update
    void Start() {
        GetDynamicBarsComponents();
        UpdateBarsUI();
    }

    private void GetDynamicBarsComponents() {
        foreach (Transform child in dynamicBarsUI.transform) {
            if (child.gameObject.CompareTag("DynamicBar")) {
                dynamicBarsSliders.Add(child.GetComponent<Slider>());
                dynamicBarsImg.Add(child.Find("ColorBar").gameObject.GetComponent<Image>());
                dynamicBarsStatus.Add(child.Find("BarValues").gameObject.GetComponent<TextMeshProUGUI>());
            }
        }
        Assert.AreEqual(dynamicBarsSliders.Count, Enum.GetValues(typeof(BarType)).Length, $"The number of bars {dynamicBarsSliders.Count} must be equal to the number of bar types {string.Join(", ", Enum.GetNames(typeof(BarType)))}");
    }

    
    

    public void ConsumeItem(InventoryItem inventoryItem) {
        if (inventoryItem.categoryProperties == null && inventoryItem.categoryProperties.Count == 0) {
            return;
        }
    }



    public void UpdateBarsUI() {
        foreach (BarType barType in Enum.GetValues(typeof(BarType))) {
            UpdateBarUI(barType);
        }
    }

    

    void UpdateBarUI(BarType barType) {
        dynamicBarsSliders[(int)barType].value = (float)currValues[(int)barType]/ maxValues[(int)barType];
        dynamicBarsStatus[(int)barType].text = $"{currValues[(int)barType]}/{maxValues[(int)barType]}";
        int numberOfColors = dynamicBarsColors[(int)barType].Length;
        if (numberOfColors == 1) {
            dynamicBarsImg[(int)barType].color = dynamicBarsColors[(int)barType][0];
        }
        else {
            // divide the range values from 0 to maxValue into numberOfColors equal parts
            float range = maxValues[(int)barType] / numberOfColors;
            for (int i = 0; i < numberOfColors; i++) {
                if (currValues[(int)barType] >= range * (numberOfColors - i-1)) {
                    dynamicBarsImg[(int)barType].color = dynamicBarsColors[(int)barType][i];
                    break;
                }
            }
        }
    }
}
