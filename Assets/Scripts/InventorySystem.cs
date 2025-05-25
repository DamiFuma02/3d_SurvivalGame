using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Linq;
using TMPro;
using static UnityEditor.Progress;


public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; set; }

    

    public GameObject inventoryScreenUI;
    public GameObject playerBarUI;
    // make it accessible in InventoryItem.cs to handle the hover info UI
    public GameObject hoveredItemInfoUI;

    // array of the gameobject inventory slots in the inventory screen UI
    public GameObject[] inventorySlotsArray;
    // array of the InventoryItem component of each slot in the inventory screen UI
    public InventoryItem[] inventoryItems;
    // array of the gameobject inventory slots in the player bar UI refering 
    // to the first slots of the inventory screen UI
    public GameObject[] playerBarSlotsArray;

    
    
    public bool isOpen = false;
    public bool isFull = false;


    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        }
        else {
            Instance = this;
        }
    }


    void Start() {
        isOpen = false;
        GetInventorySlots();
    }

    

    void GetInventorySlots() {
        List<GameObject> inventoryScreenSlots = new List<GameObject>();
        List<GameObject> playerBarSlots = new List<GameObject>();
        GameObject currObj;
        for (int i = 0; i < inventoryScreenUI.transform.childCount-1; i++) {
            currObj = inventoryScreenUI.transform.GetChild(i).gameObject;
            if (currObj.CompareTag("invSlot")) {
                inventoryScreenSlots.Add(currObj);
            }
        }

        for (int i = 0; i < playerBarUI.transform.childCount; i++) {
            currObj = playerBarUI.transform.GetChild(i).gameObject;
            if (currObj.CompareTag("playerBarInvSlot")) {
                playerBarSlots.Add(currObj);
            }
        }
        inventorySlotsArray = inventoryScreenSlots.ToArray();
        playerBarSlotsArray = playerBarSlots.ToArray();
        Assert.IsTrue(playerBarSlotsArray.Length <= inventorySlotsArray.Length);
        inventoryItems = new InventoryItem[inventorySlotsArray.Length];
    }


    void Update() {
        ToggleInventory();
    }

    void ToggleInventory() {
        if (Input.GetKeyDown(KeyCode.E) && !isOpen) {
            Debug.Log("E is pressed");
            UpdateInventoryUI();
            inventoryScreenUI.SetActive(true);
            playerBarUI.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
            isOpen = true;
            CraftingSystem.instance.craftingMenuUI.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.E) && isOpen) {
            inventoryScreenUI.SetActive(false);
            playerBarUI.SetActive(true);
            CraftingSystem.instance.craftingMenuUI.SetActive(false);
            CraftingSystem.instance.craftCategoriesScreenUIParent.SetActive(false);
            // close all the crafting UIs left open
            foreach (var screen in CraftingSystem.instance.craftCategoryScreensArray) {
                screen.SetActive(false);
            }
            Cursor.lockState = CursorLockMode.Locked;
            isOpen = false;
        }
    }

    private void UpdateInventoryUI() {
        foreach (GameObject slot in inventorySlotsArray) { 
            if (slot.transform.childCount > 0) {
                GameObject currItem = slot.transform.GetChild(0).gameObject;
                string itemName = currItem.name.Substring(0, currItem.name.IndexOf("(")).Trim();
                InventoryItem currInventoryItem = currItem.GetComponent<InventoryItem>();
                currItem.transform.Find("ItemProperties").GetComponent<TextMeshProUGUI>().text = currInventoryItem.quantity.ToString();
            }
        }
        for (int i = 0; i < playerBarSlotsArray.Length; i++) {
            if (inventorySlotsArray[i].transform.childCount > 0) {
                if (playerBarSlotsArray[i].transform.childCount > 0) {
                    playerBarSlotsArray[i].transform.GetChild(0).GetComponent<InventoryItem>().quantity = inventorySlotsArray[i].transform.GetChild(0).GetComponent<InventoryItem>().quantity;
                    playerBarSlotsArray[i].transform.GetChild(0).Find("ItemProperties").GetComponent<TextMeshProUGUI>().text = inventorySlotsArray[i].transform.GetChild(0).Find("ItemProperties").GetComponent<TextMeshProUGUI>().text;
                } else { 
                    Instantiate(inventorySlotsArray[i].transform.GetChild(0).gameObject,
                        playerBarSlotsArray[i].transform.position,
                        playerBarSlotsArray[i].transform.rotation
                    ).transform.SetParent(playerBarSlotsArray[i].transform);
                }
                // if the inventory slot is empty but the player bar no
                // it means that item has been removed from the inventory
            } else if (playerBarSlotsArray[i].transform.childCount > 0) {
                Destroy(playerBarSlotsArray[i].transform.GetChild(0).gameObject);
                playerBarSlotsArray[i].transform.DetachChildren();
            }
        }

    }


    /// <summary>
    /// Can be called by InteractableObject.cs to add an item to the inventory
    /// or by the CraftingSystem to add a crafted item
    /// </summary>
    /// <param name="itemName"></param>
    /// <param name="itemCategory"></param>
    /// <param name="itemQuantity"></param>
    public void AddToInventory(string itemName,ItemCategory itemCategory=ItemCategory.CraftingItem, int itemQuantity=1) {
        bool itemFoundInInventory = CheckIfItemInInventory(itemName);
        int validIdx = GetFirstInsertionValidIndex(itemName, itemFoundInInventory);
        Assert.AreNotEqual(validIdx,-1,"The inventory is Full");
        GameObject chosenSlot = inventorySlotsArray[validIdx];
        if (chosenSlot.transform.childCount > 0) {
            InventoryItem currItem = chosenSlot.transform.Find(itemName + "(Clone)").GetComponent<InventoryItem>();
            currItem.quantity += itemQuantity;
            int diff = currItem.quantity + itemQuantity - currItem.maxQuantityPerStack;
            if (diff > 0) {
                // recursively add the item to the inventory
                AddToInventory(itemName, itemCategory, diff);
            }
            inventoryItems[validIdx] = currItem;
        }
        else {  // first time adding the item to this inventory slot
            GameObject itemToAdd = GameObject.Instantiate(
                        Resources.Load<GameObject>("InventorySystemPrefabs/" + itemName),
                        chosenSlot.transform.position,
                        chosenSlot.transform.rotation
                    );
            itemToAdd.GetComponent<InventoryItem>().quantity = itemQuantity;
            itemToAdd.GetComponent<InventoryItem>().category = itemCategory;
            itemToAdd.transform.SetParent(chosenSlot.transform);
            inventoryItems[validIdx] = itemToAdd.GetComponent<InventoryItem>();
        }
        
        
        UpdateInventoryUI();
    }


    /// <summary>
    /// Removes itemQty from the existing item.quantity in the inventory
    /// Called by CraftingSystem when a crafting is available
    /// </summary>
    /// <param name="itemName"></param>
    /// <param name="itemQty"></param>
    public void RemoveFromInventory(string itemName, int itemQty = 1) {
        // removes the items from the end to start of inventory
        Assert.IsTrue(CheckQuantity(itemName, itemQty));
        int currIdx = FindFirstValidRemovalIndex(itemName);
        InventoryItem currInventoryItem = inventoryItems[currIdx];
        GameObject chosenSlot = inventorySlotsArray[currIdx]; 
        if (currInventoryItem.quantity < itemQty) {
            int currQty = currInventoryItem.quantity;
            RemoveItemFromSlot(currIdx);
            // recursively remove the item from the inventory
            RemoveFromInventory(itemName, itemQty - currQty);
        } else {
            currInventoryItem.quantity -= itemQty;
            if (currInventoryItem.quantity == 0) {
                RemoveItemFromSlot(currIdx);
            }
        }
        UpdateInventoryUI();
    }

    void RemoveItemFromSlot(int slotIndex) {
        GameObject chosenSlot = inventorySlotsArray[slotIndex];
        Destroy(chosenSlot.transform.GetChild(0).gameObject);
        chosenSlot.transform.DetachChildren();
        inventoryItems[slotIndex] = null; // remove the item from the inventory array
    }


    private int GetFirstInsertionValidIndex(string itemName, bool itemFoundInInventory) {
        if (itemFoundInInventory) {
            for (int i = 0; i < inventoryItems.Length; i++) {
                if (inventoryItems[i] != null && inventoryItems[i].itemName == itemName && inventoryItems[i].quantity < inventoryItems[i].maxQuantityPerStack) {
                    return i;
                }
            }
        }
        // all cells are full so return the first empty sell
        // or itemFoundInInventory = false
        for (int i = 0; i < inventoryItems.Length; i++) {
            if (inventoryItems[i] == null) {
                return i;
            }
        }
        return -1; // No valid index found so the inventory is full
    }

    private bool CheckIfItemInInventory(string itemName) {
        foreach (InventoryItem item in inventoryItems) {
            if (item != null && item.itemName == itemName) {
                return true;
            }
        }
        return false;
    }

    


    private int FindFirstValidRemovalIndex(string itemName) {
        for (int i = inventoryItems.Length-1; i >= 0; i--) {
            if (inventoryItems[i] != null && inventoryItems[i].itemName == itemName ) {
                return i;
            }
        }
        return -1;
    }


    private GameObject FindFirstEmptySlot() {
        foreach (GameObject slot in inventorySlotsArray) {
            if (slot.transform.childCount == 0) {
                return slot;
            }
        }
        return null;
    }

    public bool CheckFull() {
        foreach (var item in inventoryItems) {
            if (item == null) {
                return false;
            }
        }
        return true;
    }

    public bool CheckQuantity(string itemName, int quantity) {
        int currQty = 0;
        foreach (var item in inventoryItems) {
            if (item != null && item.itemName == itemName) {
                currQty += item.quantity;
            }
        }
        return currQty >= quantity;
    }

    
}
