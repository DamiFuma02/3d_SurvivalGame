using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Linq;
using TMPro;


public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; set; }

    public string crafting2dIconsDirectory = "2D_Prefabs/CraftingSystemPrefabs/";
    public string inventory2dIconsDirectory = "2D_Prefabs/InventorySystemPrefabs/";
    public string interactableObjects3dprefabsDirectory = "3d_Prefabs/InteractableObjects3DPrefabs/";

    


    public int inventorySize = 20; // total number of slots in the inventory
    public List<int> inventoryRowsColsLimits = new List<int>() { 400, 200 };

    public int playerBarSize = 5; // total number of slots in the inventory
    public int playerBarXOffset = 320;
    int row, col;

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
    public bool equippedItemFlag;
    private int equippedPlayerBarIdx;
    private Transform firstPersonCamera;
    private Transform equippedItemUI;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        }
        else {
            Instance = this;
        }
    }


    void Start() {
        Debug.Assert(playerBarSize < inventorySize, "The number of slots in the playerBar must be less then the number in the inventory screen");
        Assert.IsFalse(IsPrime(inventorySize),"The number of items inside the inventory cannot be a prime value");
        Assert.IsTrue(playerBarSize<=9,"Not enough hotkeys to assing to each slot in the player bar. Maximum=9");
        isOpen = false;
        Cursor.visible = false;
        ConfigInventorySlots(GetGridSize(inventorySize));
    }

    private int[] GetGridSize(int inventoryDimension) {
        int[] sizes = new int[2];
        for (int i = (int)Math.Sqrt(inventoryDimension); i >= 2; i--) // Only check up to sqrt(num)
        {
            if (inventoryDimension % i == 0) // Check if i is a factor
            {
                int pair = inventoryDimension / i;
                if (pair<i) {
                    sizes[0] = i; // rows
                    sizes[1] = pair; // cols
                } else {
                    sizes[0] = pair;
                    sizes[1] = i;
                }
                break;
            }
        }
        return sizes;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gridSizes">rows x cols</param>
    void ConfigInventorySlots(int[] gridSizes) {
        int nRows = gridSizes[0];
        int nCols = gridSizes[1];
        Debug.Assert(nRows >= nCols, "The number of rows must be >= the number of columns");
        GameObject currInvSlot;
        RectTransform currRectTransform;
        List<GameObject> inventoryScreenSlots = new List<GameObject>();
        float parentWidth = inventoryScreenUI.transform.GetComponent<RectTransform>().rect.width;
        float parentHeight = inventoryScreenUI.transform.GetComponent<RectTransform>().rect.height;
        float slotWidth = parentWidth / nRows;
        float slotHeight = parentHeight / nCols;
        float xPos, yPos;
        for (int j = 0; j < nCols; j++) {
            for (int i = 0; i < nRows; i++) {
                currInvSlot = Instantiate(Resources.Load<GameObject>(inventory2dIconsDirectory+"invSlot"),
                    inventoryScreenUI.transform
                );
                currRectTransform = currInvSlot.GetComponent<RectTransform>();
                xPos = (i * slotWidth) - (parentWidth / 2) + (slotWidth / 2);
                yPos = (-j * slotHeight) + (parentHeight / 2) - (slotHeight / 2);
                currRectTransform.anchoredPosition = new Vector2(xPos, yPos);
                currInvSlot.gameObject.SetActive(true);
                inventoryScreenSlots.Add(currInvSlot);  
            }
        }
        
        List<GameObject> playerBarSlots = new List<GameObject>();
        GameObject currPlayerBarInvSlot;
        parentWidth = playerBarUI.transform.GetComponent<RectTransform>().rect.width;
        slotWidth = parentWidth / playerBarSize;
        for (int i = 0; i < playerBarSize; i++) {
            currPlayerBarInvSlot = Instantiate(Resources.Load<GameObject>(inventory2dIconsDirectory + "playerBarInvSlot"),
                    playerBarUI.transform
                );
            currRectTransform = currPlayerBarInvSlot.GetComponent<RectTransform>();
            xPos = (i * slotWidth) - (parentWidth / 2) + (slotWidth / 2);
            currRectTransform.anchoredPosition = new Vector2(xPos, 0);
            currPlayerBarInvSlot.gameObject.SetActive(true);
            currPlayerBarInvSlot.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (i + 1).ToString();
            playerBarSlots.Add(currPlayerBarInvSlot);
        }
        inventorySlotsArray = inventoryScreenSlots.ToArray();
        playerBarSlotsArray = playerBarSlots.ToArray();
        Assert.IsTrue(playerBarSlotsArray.Length <= inventorySlotsArray.Length);
        Assert.AreEqual(inventorySlotsArray.Length, inventorySize);
        inventoryItems = new InventoryItem[inventorySlotsArray.Length];
    }

    
    void Update() {
        ToggleInventory();
        CheckPlayerKeyPress();
    }

    /// <summary>
    /// Checks if the player has pressed a number key from 0 to 9 
    /// or the right mouse button to consume an ItemCategory.Consumable item equipped from the the player bar.
    /// </summary>
    private void CheckPlayerKeyPress() {
        int startNumber = (int)KeyCode.Alpha0;  
        for (int i = (int)KeyCode.Alpha0; i <= (int)KeyCode.Alpha9; i++) {
            if (Input.GetKeyDown((KeyCode)i)) {
                SelectItemInPlayerBar(i-startNumber-1);
            }
        }
        if (equippedItemFlag && Input.GetKeyDown(KeyCode.Mouse1)) {
            equippedItemUI = firstPersonCamera.transform.Find("EquippedItemUI");
            // get the InteractableObject component of the equipped item
            if (inventoryItems[equippedPlayerBarIdx].category == ItemCategory.Consumable) {
                StartCoroutine(AnimateEquippedItem());
                // consume 1 item and 
                PlayerDynamicBarsSystem.Instance.ConsumeItem(inventoryItems[equippedPlayerBarIdx]);
                RemoveFromInventory(inventoryItems[equippedPlayerBarIdx].itemName, 1);
                if (inventoryItems[equippedPlayerBarIdx] == null) {
                    Destroy(equippedItemUI.transform.GetChild(0).gameObject);
                    equippedItemFlag = false;
                    equippedPlayerBarIdx = -1;
                    equippedItemUI.gameObject.SetActive(false);
                    // resets all the playerbar icons to white whitout calling the toggle method
                    SelectItemInPlayerBar(playerBarSize + 1);
                }
            }
            else {
                Debug.LogWarning("Cannot consume an item that is not a Consumable type");
            }
        }
    }



    /// <summary>
    /// if selectedPlayerBarSlotIdx>playerBarSize sets all to white whitout calling the Toggle Method
    /// </summary>
    /// <param name="selectedPlayerBarSlotIdx"></param>
    private void SelectItemInPlayerBar(int selectedPlayerBarSlotIdx) {
        for (int i = 0; i < playerBarSize; i++) {
            Color currColor = Color.white;
            Color prevColor = Color.white;
            // other than the hotKey child, check if thereis another child, the 2d prefab of an item in the inventory
            if (i == selectedPlayerBarSlotIdx && inventoryItems[selectedPlayerBarSlotIdx] != null) {        
                prevColor = playerBarSlotsArray[i].transform.Find("hotKey").GetComponent<TextMeshProUGUI>().color;
                currColor = prevColor == Color.black ? Color.white : Color.black;
                // not valid because the items has been removed from inventoryItems so it tries to access at null
                ToggleEquippedItem(selectedPlayerBarSlotIdx, prevColor == Color.black);
            }
            playerBarSlotsArray[i].transform.Find("hotKey").GetComponent<TextMeshProUGUI>().color = currColor;
        }
    }


    /// <summary>
    /// Toggles the equipped item with the animator component inside the first person camera UI 
    /// </summary>
    /// <param name="playerBarSlotIdx"></param>
    /// <param name="prevEquipped"></param>
    private void ToggleEquippedItem(int playerBarSlotIdx,bool prevEquipped) {
        firstPersonCamera = PlayerMovement.instance.firstPersonCamera;
        equippedItemUI = firstPersonCamera.Find("EquippedItemUI");
        if (equippedItemUI.transform.childCount>0) {
            Destroy(equippedItemUI.transform.GetChild(0).gameObject);
            equippedItemFlag = false;
            equippedPlayerBarIdx = -1;
            equippedItemUI.gameObject.SetActive(false);  
        }
        if (!prevEquipped && inventoryItems[playerBarSlotIdx].category != ItemCategory.CraftingItem && inventoryItems[playerBarSlotIdx].category!=ItemCategory.Armor) {
            Instantiate(Resources.Load<GameObject>(interactableObjects3dprefabsDirectory + inventoryItems[playerBarSlotIdx].itemName+"Equipped"),
                equippedItemUI.transform
            );
            equippedItemFlag = true;
            equippedPlayerBarIdx = playerBarSlotIdx;
            equippedItemUI.gameObject.SetActive(true);
        }
    }

    void ToggleInventory() {
        if (Input.GetKeyDown(KeyCode.E) && !isOpen) {
            Debug.Log("E is pressed");
            UpdateInventoryUI();
            inventoryScreenUI.SetActive(true);
            playerBarUI.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

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
            Cursor.visible = false;

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
                if (playerBarSlotsArray[i].transform.childCount > 1) {
                    playerBarSlotsArray[i].transform.GetChild(1).GetComponent<InventoryItem>().quantity = inventorySlotsArray[i].transform.GetChild(0).GetComponent<InventoryItem>().quantity;
                    playerBarSlotsArray[i].transform.GetChild(1).Find("ItemProperties").GetComponent<TextMeshProUGUI>().text = inventorySlotsArray[i].transform.GetChild(0).Find("ItemProperties").GetComponent<TextMeshProUGUI>().text;
                } else { 
                    Instantiate(inventorySlotsArray[i].transform.GetChild(0).gameObject,
                        playerBarSlotsArray[i].transform.position,
                        playerBarSlotsArray[i].transform.rotation
                    ).transform.SetParent(playerBarSlotsArray[i].transform);
                }
                // if the inventory slot is empty but the player bar no
                // it means that item has been removed from the inventory
            } else if (playerBarSlotsArray[i].transform.childCount > 1) {
                DestroyImmediate(playerBarSlotsArray[i].transform.GetChild(1).gameObject);
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
        if (validIdx == -1) { // no valid index found so the inventory is full
            Debug.LogWarning("Inventory is full. Cannot add item: " + itemName);
            DropItems(itemName, itemCategory,itemQuantity);
            return;
        }
        
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
                        Resources.Load<GameObject>(inventory2dIconsDirectory + itemName),
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

    private void DropItems(string itemName, ItemCategory itemCategory, int itemQuantity) {
        GameObject item3dPrefab = Resources.Load<GameObject>(interactableObjects3dprefabsDirectory + itemName);
        Assert.AreEqual(item3dPrefab.GetComponent<InteractableObject>().itemCategory, itemCategory,"Error the item categories don't match");
        for (int i = 0; i < itemQuantity; i++) {
            Instantiate(item3dPrefab,
                transform.position + Vector3.up * 0.5f + Vector3.forward * i,  // slightly above the ground
                Quaternion.identity);
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


    bool IsPrime(int number) {
        if (number <= 1) return false;
        if (number == 2) return true; // 2 is the smallest prime
        if (number % 2 == 0) return false; // Even numbers other than 2 are not prime

        for (int i = 3; i <= Math.Sqrt(number); i += 2) // Check only odd numbers up to sqrt(number)
        {
            if (number % i == 0) return false;
        }

        return true;
    }

    public void AddToInventoryOrDropItems(string itemName, ItemCategory itemCategory, DropItemsDictionary drops=null) {
        if (drops.Count > 0) {
            foreach (var drop in drops) {
                AddToInventory(drop.Key, Resources.Load<GameObject>(inventory2dIconsDirectory + drop.Key).GetComponent<InventoryItem>().category, drop.Value);
            }
        } else {
            // if no drops are specified, just add the item to the inventory
            AddToInventory(itemName, itemCategory, 1);
        }
    }

    /// <summary>
    /// The equipped item cannot have category=ItemCategory.CraftingItem and ItemCategory.Armor
    /// </summary>
    public IEnumerator AnimateEquippedItem() {
        firstPersonCamera = PlayerMovement.instance.firstPersonCamera;
        equippedItemUI = firstPersonCamera.Find("EquippedItemUI");

        if (equippedItemUI.childCount==0) {
            Debug.Log("NO Animation");
            yield return null;  // no equipped item to animate
        }
        ItemCategory currentItemCategory = equippedItemUI.GetChild(0).GetComponent<InteractableObject>().itemCategory;
        string triggerName = currentItemCategory == ItemCategory.Consumable ? "Hit" : "Hit";
        equippedItemUI.GetComponent<Animator>().SetTrigger(triggerName);
        yield return new WaitUntil(() => !equippedItemUI.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("equippedItem_action"));
    }
}
