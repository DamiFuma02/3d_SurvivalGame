using System;
using System.Collections;
using System.Collections.Generic;
using Palmmedia.ReportGenerator.Core.Common;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Text.Json;


[Serializable]
public class CategoryPropertiesDictionary : SerializableDictionary<string, int> {
    /// <summary>
    /// Creates empty dictionary. :base() can be removed an it will work
    /// </summary>
    public CategoryPropertiesDictionary() :base() {}
    /// <summary>
    /// calls the constructor for the parent class SerializableDictionary<string,string> 
    /// </summary>
    /// <param name="dictionary">argument of the parent (base) constructor</param>
    public CategoryPropertiesDictionary(IDictionary<string, int> dictionary) : base(dictionary) {}
}
[Serializable] public class CraftingRecipeDictionary: SerializableDictionary<string, int> { }






public enum ItemCategory {
    CraftingItem, Consumable, Armor, Tool, Weapon
}

public enum ItemMaterial {
    NoMaterial,Wood,Stone
}

public enum ArmorType {
    Helmet, Chestplate, Leggings, Boots
}



public class InventoryItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler {
    public string itemName;   //unique name
    public int quantity;
    public int maxQuantityPerStack;
    public ItemCategory category;
    // key: property name, value: property value (can be any type)
    public CategoryPropertiesDictionary categoryProperties = new CategoryPropertiesDictionary();
    // key: item name, value: item quantity required for crafting
    public CraftingRecipeDictionary craftingRecipe = new CraftingRecipeDictionary();


    GameObject hoveredItemInfoUI;
    TextMeshProUGUI hoveredItemName;
    TextMeshProUGUI hoveredItemCategory;
    TextMeshProUGUI hoveredItemCategoryProperties;


    public InventoryItem(string name,ItemCategory itemCategory=ItemCategory.CraftingItem,int itemQuantity=1) {
        itemName = name;
        quantity = itemQuantity;
        if ((int)itemCategory < 2) {
            maxQuantityPerStack = 16;
        } else {
            maxQuantityPerStack = 1;
        }
        category = itemCategory;
        categoryProperties = new CategoryPropertiesDictionary();
        craftingRecipe = new CraftingRecipeDictionary();
    }



    public void OnPointerEnter(PointerEventData eventData) {
        hoveredItemInfoUI.gameObject.SetActive(true);
        hoveredItemName.text = $"{itemName}: {quantity}";
        hoveredItemCategory.text = category.ToString();
        hoveredItemCategoryProperties.text = categoryProperties.Count > 0 ? categoryProperties.ToString() : "No properties available";
    }

    public void OnPointerExit(PointerEventData eventData) {
        hoveredItemInfoUI.gameObject.SetActive(false);
    }

    public void OnPointerUp(PointerEventData eventData) {
        throw new NotImplementedException();
    }
    public void OnPointerDown(PointerEventData eventData) {
        throw new NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        AssertCorrectInstanceValues();
        GetHoverInfoUIComponents();
    }

    
    private void AssertCorrectInstanceValues() {
        if ((int)category < 2) {
            maxQuantityPerStack = 16;
        }
        else {
            maxQuantityPerStack = 1;
        }

        Dictionary<string, int> requiredCategoryPropertiesIDict = new Dictionary<string, int>();
        switch (category) {
            case ItemCategory.Consumable:
                requiredCategoryPropertiesIDict = new Dictionary<string, int>() {
                        { "health", 0 },
                        { "food", 0 },
                        { "water", 0 }
                    };
                break;
            case ItemCategory.Tool:
                requiredCategoryPropertiesIDict = new Dictionary<string, int>() {
                        { "maxBreakableMaterial", (int)ItemMaterial.Stone },
                        { "durability", 0 },
                        { "damage", 0 },
                    };
                break;
            case ItemCategory.CraftingItem:
                requiredCategoryPropertiesIDict = new Dictionary<string, int>() {
                        { "material", (int)ItemMaterial.Stone }
                    };
                break;
            case ItemCategory.Weapon:
                requiredCategoryPropertiesIDict = new Dictionary<string, int>() {
                        { "durability", 0 },
                        { "damage", 0 },
                    };
                break;
            default:
                break;
        }
        if (categoryProperties == null || categoryProperties.Count == 0) {
            categoryProperties = new CategoryPropertiesDictionary(requiredCategoryPropertiesIDict);
        }
        // if empty it will simply not be executed
        foreach (var keyValuePair in requiredCategoryPropertiesIDict) {
            if (!categoryProperties.ContainsKey(keyValuePair.Key)) {
                categoryProperties.Add(keyValuePair);
            }
        }
    }

    private void GetHoverInfoUIComponents() {
        hoveredItemInfoUI = InventorySystem.Instance.hoveredItemInfoUI;
        hoveredItemInfoUI.transform.Find("ItemName").TryGetComponent<TextMeshProUGUI>(out hoveredItemName);
        hoveredItemInfoUI.transform.Find("ItemCategory").TryGetComponent<TextMeshProUGUI>(out hoveredItemCategory);
        hoveredItemInfoUI.transform.Find("ItemCategoryProperties").TryGetComponent<TextMeshProUGUI>(out hoveredItemCategoryProperties);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
