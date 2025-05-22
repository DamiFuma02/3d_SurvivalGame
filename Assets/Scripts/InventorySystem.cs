using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Linq;
using Assets.Scripts;
using TMPro;


public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; set; }

    public GameObject inventoryScreenUI;
    public GameObject playerBarUI;
    

    public List<GameObject> inventoryUISlotList = new List<GameObject>();  
    public List<GameObject> playerBarSlotList = new List<GameObject>();  
    public List<InventoryItem> inventoryItems = new List<InventoryItem>();

    
    
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
        foreach (Transform slot in inventoryScreenUI.transform) {
            if (slot.CompareTag("invSlot")) {
                inventoryUISlotList.Add(slot.gameObject);
            }
        }
        foreach (Transform slot in playerBarUI.transform) {
            if (slot.CompareTag("invSlot")) {
                playerBarSlotList.Add(slot.gameObject);
            }
        }
    }


    void Update() {
        ToggleInventory();
    }

    void ToggleInventory() {
        if (Input.GetKeyDown(KeyCode.E) && !isOpen) {

            Debug.Log("E is pressed");
            UpdateInventoryUI();
            inventoryScreenUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            isOpen = true;
            CraftingSystem.instance.craftingMenuUI.SetActive(true);

        }
        else if (Input.GetKeyDown(KeyCode.E) && isOpen) {
            inventoryScreenUI.SetActive(false);
            CraftingSystem.instance.craftingMenuUI.SetActive(false);
            CraftingSystem.instance.craftCategoriesScreenUIParent.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            isOpen = false;
        }
    }

    private void UpdateInventoryUI() {
        foreach (GameObject slot in inventoryUISlotList) { 
            if (slot.transform.childCount > 0) {
                GameObject currItem = slot.transform.GetChild(0).gameObject;
                string itemName = currItem.name.Substring(0, currItem.name.IndexOf("(")).Trim();
                inventoryItems.ForEach(item => {
                    if (item.name == itemName) {
                        currItem.transform.Find("ItemProperties").GetComponent<TextMeshProUGUI>().text = item.quantity.ToString();
                    }
                });
            }
        }

    }

    /// <summary>
    /// Removes itemQty from the existing item.quantity in the inventory
    /// Called by CraftingSystem when a crafting is available
    /// </summary>
    /// <param name="itemName"></param>
    /// <param name="itemQty"></param>
    public void RemoveFromInventory(string itemName, int itemQty = 1) {
        // removes the items from the end to start of inventory
        GameObject chosenSlot = FindSlotWithItem(itemName, fromStart: false);
        Assert.AreNotEqual(chosenSlot, null);
        foreach (InventoryItem item in inventoryItems) {
            if (item.name == itemName) {
                item.quantity -= itemQty;
                if (item.quantity == 0) {
                    Destroy(chosenSlot.transform.Find(itemName+"(Clone)").gameObject);
                    chosenSlot.transform.DetachChildren();
                }
                break;
            }
        }
        UpdateInventoryUI();
    }



    public void AddToInventory(string itemName) {
        GameObject chosenSlot = FindSlotWithItem(itemName,fromStart:true);
        bool itemFoundInInventory = true;
        if (chosenSlot == null) {
            itemFoundInInventory = false;
            chosenSlot = FindFirstEmptySlot();
        }
        Assert.IsNotNull(chosenSlot, "Assertion Error: cannot find any empty slot");
        int currQty = 1;
        if (itemFoundInInventory) {
            // increment inside the inventoryItems list the quantity of the itemName
            foreach (InventoryItem item in inventoryItems) {
                if (item.name == itemName) {
                    item.quantity++;
                    currQty = item.quantity;
                    break;
                }
            }
            UpdateInventoryUI();
        } else { 
            inventoryItems.Add(new InventoryItem(itemName));
            GameObject itemToAdd = GameObject.Instantiate(
                        Resources.Load<GameObject>(itemName),
                        chosenSlot.transform.position,
                        chosenSlot.transform.rotation
                    );
            itemToAdd.transform.SetParent(chosenSlot.transform);
        }
        Debug.Log("Moved " + itemName + " to the inventory in " + chosenSlot.name);
    }

    private GameObject FindSlotWithItem(string itemName,bool fromStart=true) {
        if (fromStart) {
            foreach (GameObject slot in inventoryUISlotList) {
                if (slot.transform.childCount > 0 && slot.transform.Find(itemName+"(Clone)") != null) {
                    return slot;
                }
            }
        } else {
            for (int i= inventoryUISlotList.Count-1; i>=0; i--) {
                if (inventoryUISlotList[i].transform.childCount > 0 && inventoryUISlotList[i].transform.Find(itemName + "(Clone)") != null) {
                    return inventoryUISlotList[i];
                }
            }
        }
        return null;
    }


    private GameObject FindFirstEmptySlot() {
        foreach (GameObject slot in inventoryUISlotList) {
            if (slot.transform.childCount == 0) {
                return slot;
            }
        }
        return null;
    }

    public bool CheckFull() {
        return FindFirstEmptySlot() == null;
    }

    public bool CheckQuantity(string itemName, int quantity) {
        return inventoryItems.Where(item => item.name == itemName && item.quantity >= quantity).Any();
    }

    
}
