using System;
using System.Collections;
using System.Collections.Generic;
using Palmmedia.ReportGenerator.Core.Common;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Text.Json;


[Serializable] public class CategoryPropertiesDictionary: SerializableDictionary<string, string> { }
[Serializable] public class CraftingRecipeDictionary: SerializableDictionary<string, int> { }

public enum ItemCategory {
    CraftingItem, Consumable, Armor, Tool, Weapon
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
        GetHoverInfoUIComponents();
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
