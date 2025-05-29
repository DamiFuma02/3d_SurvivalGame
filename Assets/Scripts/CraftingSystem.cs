using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using NUnit.Framework;
using System.Linq;
using NUnit.Framework.Interfaces;
using System;
using UnityEditor;

public class CraftingSystem : MonoBehaviour
{
    // SINGLETON CraftingSystem
    public static CraftingSystem instance { get; set; }

    // all the items that can be crafted from the Resources/InventoryItemIcons folder
    public Dictionary<ItemCategory, List<InventoryItem>> availableCraftableInventoryItemsByCategory = new Dictionary<ItemCategory, List<InventoryItem>>();
     

    // Crafting Menu 
    public GameObject craftingMenuUI;
    public Button[] itemCategoryMenuButtons;


    // empty GameObject parent for all the different categories UIs
    public GameObject craftCategoriesScreenUIParent;
    // for each Category there is a CraftingScreenUI 
    public GameObject[] craftCategoryScreensArray;
    // inside each single category crafting UI screen there are
    // multiple craftable objects with a single UI (button and recipe) for each object

    public int gridSize = 3; // 3 x 3 grid
    public float spacing = 2f;





    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(gameObject);
        } else {
            instance = this;
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        GetAllAvailableCraftableInventoryItems();
        ConfigAllCategoriesCraftingUIScreen();
        ConfigCategoryButtonsFromMenu();
        
    }

    private void GetAllAvailableCraftableInventoryItems() {
        GameObject[] invIconsGameObjects = Resources.LoadAll<GameObject>(InventorySystem.Instance.inventory2dIconsDirectory);
        InventoryItem curr;
        foreach (GameObject item in invIconsGameObjects) {
            curr = item.GetComponent<InventoryItem>();
            // check if the item has the InventoryItem component attached
            if (curr != null && curr.craftingRecipe.Count > 0) {
                if (!availableCraftableInventoryItemsByCategory.ContainsKey(curr.category)) {
                    availableCraftableInventoryItemsByCategory[curr.category] = new List<InventoryItem>();
                }
                availableCraftableInventoryItemsByCategory[curr.category].Add(curr);
            }            
        }
    }

    
    private void ConfigAllCategoriesCraftingUIScreen() {
        int categoryCount = Enum.GetValues(typeof(ItemCategory)).Length;
        craftCategoryScreensArray = new GameObject[categoryCount];
        // get the first default crafting category screen UI 
        // create all the others category crafting screens UI
        GameObject currCategoryCraftingUI;
        Debug.Log($"Test print {InventorySystem.Instance.crafting2dIconsDirectory + "craftCategoryUi"}");
        for (int i = 0; i < categoryCount; i++) {
            currCategoryCraftingUI  = Instantiate(Resources.Load<GameObject>(InventorySystem.Instance.crafting2dIconsDirectory+"craftCategoryUi"), craftCategoriesScreenUIParent.transform);
            currCategoryCraftingUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Craft " + ((ItemCategory)i).ToString();
            AddItemsScreenUIsByCategory(currCategoryCraftingUI,(ItemCategory)i);
            currCategoryCraftingUI.transform.SetParent(craftCategoriesScreenUIParent.transform);
            currCategoryCraftingUI.gameObject.SetActive(false);
            craftCategoryScreensArray[i] = currCategoryCraftingUI;
        }
    }

    /// <summary>
    /// Configures the crafting object UI for the current category in a maximum grid 3 x 3 layout, 
    /// each screen containing the items that can be crafted in that category.
    /// If the category count > 9 add a button to go to the text pagee
    /// </summary>
    private void AddItemsScreenUIsByCategory(GameObject craftingCategoryUI, ItemCategory currCategory) {
        GameObject craftItemUIPrefab = Resources.Load<GameObject>(InventorySystem.Instance.crafting2dIconsDirectory+"craftItemUI");
        // iterate through all the items in the current category
        int categoryIndex = (int)currCategory;
        if (!availableCraftableInventoryItemsByCategory.ContainsKey(currCategory)) {
            return;
        }
        int[] xPositionOffset = { -220, 0, 220 };
        int[] yPositionOffset = { 270, 0, -270 };
        int row, col;
        for (int i = 0; i < availableCraftableInventoryItemsByCategory[currCategory].Count; i++) {
            InventoryItem item = availableCraftableInventoryItemsByCategory[currCategory][i];
            row = i / gridSize;
            col = i % gridSize;
            // create a new CraftItemUI for each item in the current category
            GameObject currCraftItemUI = Instantiate(craftItemUIPrefab, craftingCategoryUI.transform.position, craftingCategoryUI.transform.rotation);
            // translate the current object based on the offset row(Y) and columns(X)
            currCraftItemUI.transform.Translate(new Vector3(xPositionOffset[col], yPositionOffset[row],0), Space.Self);
            currCraftItemUI.transform.Find("ItemName").GetComponent<TextMeshProUGUI>().text = item.itemName;
            currCraftItemUI.transform.Find("ItemImage").GetComponent<Image>().overrideSprite = Resources.Load<GameObject>(InventorySystem.Instance.inventory2dIconsDirectory+item.itemName).transform.Find("ItemImage").GetComponent<Image>().sprite;
            currCraftItemUI.transform.Find("CraftingRecipeText").GetComponent<TextMeshProUGUI>().text = CraftingRecipeToString(item.craftingRecipe);
            InventoryItem currItem = item; // capture the current item
            currCraftItemUI.transform.Find("CraftItemButton").GetComponent<Button>().onClick.AddListener(() => CraftObject(currItem));
            currCraftItemUI.SetActive(true); // set the UI as active 
            // add the current CraftItemUI as a child of the current crafting category UI
            currCraftItemUI.transform.SetParent(craftingCategoryUI.transform);
        }
    }

    /// <summary>
    /// On click event listener for the CraftItemButton in the CraftItemUI
    /// called after checking all the requirements for the item to be crafted
    /// </summary>
    /// <param name="itemToCraft"></param>
    private void CraftObject(InventoryItem itemToCraft) {
        foreach (var keyValue in itemToCraft.craftingRecipe) {
            InventorySystem.Instance.RemoveFromInventory(keyValue.Key, keyValue.Value);
        }
        // add the crafted item to the inventory
        InventorySystem.Instance.AddToInventory(itemToCraft.itemName,itemToCraft.category);
        CheckAllCraftingRecipesInCategory((int)itemToCraft.category);
    }

    private string CraftingRecipeToString(CraftingRecipeDictionary craftingRecipe) {
        if (craftingRecipe.Count == 0) {
            return "";
        }
        string formattedString = "";
        foreach (var kv in craftingRecipe) {
            formattedString += $"{kv.Key}:{kv.Value}\n";
        }
        return formattedString;
    }


    


    /// <summary>
    /// Attaches the onClick event listener to each button in the crafting menu UI
    /// </summary>
    private void ConfigCategoryButtonsFromMenu() {
        // instantiate one button for each category as child of craftingMenuUI
        int categoryCount = Enum.GetValues(typeof(ItemCategory)).Length;
        int[] xPositionOffset = { -180, 0, 180 };
        int[] yPositionOffset = { 220, 0, -220 };
        int row, col;
        // counts the category that have minimum 1 craftable item in the current category 
        int validCategoryCount = 0;
        foreach (ItemCategory category in Enum.GetValues(typeof(ItemCategory))) {   
            if (availableCraftableInventoryItemsByCategory.ContainsKey(category)) {
                row = validCategoryCount / gridSize;
                col = validCategoryCount % gridSize;
                GameObject currCategoryButton = Instantiate(Resources.Load<GameObject>(InventorySystem.Instance.crafting2dIconsDirectory+"openCategoryCraftingMenu"), craftingMenuUI.transform);
                currCategoryButton.transform.Translate(new Vector3(xPositionOffset[col], yPositionOffset[row], 0), Space.Self);
                currCategoryButton.GetComponentInChildren<TextMeshProUGUI>().text = category.ToString();
                currCategoryButton.transform.Find("ButtonIcon").GetComponent<Image>().overrideSprite = Resources.Load<Sprite>(InventorySystem.Instance.crafting2dIconsDirectory + category.ToString());
                Button button = currCategoryButton.GetComponent<Button>();
                int categoryIdx = (int)category; // capture the current index
                button.onClick.AddListener(() => OpenCategoryCraftingUI(categoryIdx));
                currCategoryButton.gameObject.SetActive(true);
                validCategoryCount++;
            }
            
        }
    }

    /// <summary>
    /// Click Event Listener for each button in the crafting menu UI
    /// responsable for showing the right category crafting UI
    /// and hiding all the others
    /// </summary>
    /// <param name="categoryIdx"></param>
    void OpenCategoryCraftingUI(int categoryIdx) {
        // enables the parent and this categoryCraftingUISlot 
        craftCategoriesScreenUIParent.SetActive(true);
        for (int i = 0; i < Enum.GetValues(typeof(ItemCategory)).Length; i++) {
            craftCategoryScreensArray[i].gameObject.SetActive(i == categoryIdx);
        }
        // disables craftingMenuUI
        craftingMenuUI.SetActive(false);
        CheckAllCraftingRecipesInCategory(categoryIdx);
    }

    /// <summary>
    /// Enable/Disable all single objects crafting button after opening the category crafting UI 
    /// and checking all the requirements for each object 
    /// </summary>
    /// <param name="categoryIndex"></param>
    private void CheckAllCraftingRecipesInCategory(int categoryIndex) {
        // get all the available items in the current category
        List<InventoryItem> availableItemsInCurrentCategory = availableCraftableInventoryItemsByCategory[(ItemCategory)categoryIndex];
        // iterate through all the objects in the current category crafting UI with non empty crafting recipe
        GameObject currCategoryUI = craftCategoryScreensArray[categoryIndex];
        for (int i = 0; i < availableItemsInCurrentCategory.Count; i++) {
            InventoryItem item = availableItemsInCurrentCategory[i];
            bool validReqs = true;
            if (item.craftingRecipe.Count == 0) {
                Debug.LogWarning($"The item {item.itemName} has no crafting recipe defined.");
                validReqs = false;
            } else {
                // iterate through all the crafting recipe requirements for the current item
                Debug.Log($"Checking crafting recipe for {item.itemName} in category {((ItemCategory)categoryIndex).ToString()}");
                foreach (var req in item.craftingRecipe) {
                    if (!InventorySystem.Instance.CheckQuantity(req.Key, req.Value)) {
                        validReqs = false;
                        break;
                    }
                }
            }
            // find the button for the current item in the category crafting UI and make it visible or invisible based on the requirements
            currCategoryUI.transform.GetChild(i + 1).Find("CraftItemButton").gameObject.SetActive(validReqs);  // the first child is the title text, so we start from the second child
        }
    }


    

    
}
