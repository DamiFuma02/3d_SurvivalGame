using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; set; }

    public GameObject inventoryScreenUI;
    
    public List<GameObject> slotList = new List<GameObject>();     
    public List<string> itemNamesList = new List<string>();

    GameObject itemToAdd;
    GameObject chosenSlot;
    
    
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
        foreach (Transform child in inventoryScreenUI.transform) {
            if (child.CompareTag("invSlot")) {  
                slotList.Add(child.gameObject);
            }
        }        
    }


    void Update() {
        ToggleInventory();


    }

    void ToggleInventory() {
        if (Input.GetKeyDown(KeyCode.E) && !isOpen) {

            Debug.Log("E is pressed");
            inventoryScreenUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            isOpen = true;

        }
        else if (Input.GetKeyDown(KeyCode.E) && isOpen) {
            inventoryScreenUI.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            isOpen = false;
        }
    }

    public void AddToInventory(string itemName) {
        chosenSlot = FindFirstEmptySlot();
        Assert.IsNotNull(chosenSlot, "Assertion Error: cannot find any empty slot");
        itemToAdd = GameObject.Instantiate(
            Resources.Load<GameObject>(itemName),
            chosenSlot.transform.position,
            chosenSlot.transform.rotation
        );
        itemToAdd.transform.SetParent(chosenSlot.transform);
        itemNamesList.Add(itemName);
        Debug.Log("Moved " + itemName + " to the inventory in " + chosenSlot.name);
    }

    private GameObject FindFirstEmptySlot() {
        foreach (GameObject slot in slotList) {
            if (slot.transform.childCount == 0) {
                return slot;
            }
        }
        return null;
    }

    public bool CheckFull() {
        foreach (GameObject slot in slotList) {
            // if at least one slot is empty the inventory is not full
            if (slot.transform.childCount == 0) {
                isFull = false;
                return false;
            }
        }
        isFull = true;
        return true;
    }
}
