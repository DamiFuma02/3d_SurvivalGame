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
        GetAllAvailableInventoryItems();
        ConfigAllCategoriesCraftingUIScreen();
        ConfigCategoryButtonsFromMenu();
        
    }

    private void GetAllAvailableInventoryItems() {
        GameObject[] invIconsGameObjects = Resources.LoadAll<GameObject>("InventorySystemPrefabs");
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
        for (int i = 0; i < categoryCount; i++) {
            GameObject currCategoryCraftingUI = Instantiate(Resources.Load<GameObject>("CraftingSystemPrefabs/craftCategoryUi"), craftCategoriesScreenUIParent.transform);
            currCategoryCraftingUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Craft " + ((ItemCategory)i).ToString();
            AddItemsScreenUIsByCategory(currCategoryCraftingUI,(ItemCategory)i);
            currCategoryCraftingUI.transform.SetParent(craftCategoriesScreenUIParent.transform);
            currCategoryCraftingUI.gameObject.SetActive(false);
            craftCategoryScreensArray[i] = currCategoryCraftingUI;
        }
    }

    private void AddItemsScreenUIsByCategory(GameObject craftingCategoryUI, ItemCategory currCategory) {
        GameObject craftItemUIPrefab = Resources.Load<GameObject>("CraftingSystemPrefabs/craftItemUI");
        // iterate through all the items in the current category
        int categoryIndex = (int)currCategory;
        if (!availableCraftableInventoryItemsByCategory.ContainsKey(currCategory)) {
            return;
        }
        foreach (InventoryItem item in availableCraftableInventoryItemsByCategory[currCategory]) {
            // create a new CraftItemUI for each item in the current category
            GameObject currCraftItemUI = Instantiate(craftItemUIPrefab, craftingCategoryUI.transform.position,craftingCategoryUI.transform.rotation);
            currCraftItemUI.transform.Find("ItemName").GetComponent<TextMeshProUGUI>().text = item.itemName;
            currCraftItemUI.transform.Find("ItemImage").GetComponent<Image>().overrideSprite = Resources.Load<GameObject>("InventorySystemPrefabs/"+item.itemName).transform.Find("ItemImage").GetComponent<Image>().sprite;
            currCraftItemUI.transform.Find("CraftingRecipeText").GetComponent<TextMeshProUGUI>().text = CategoryPropertiesToString(item.craftingRecipe);
            currCraftItemUI.SetActive(true); // set the UI as inactive by default
            // add the current CraftItemUI as a child of the current crafting category UI
            currCraftItemUI.transform.SetParent(craftingCategoryUI.transform);
        }
    }

    private string CategoryPropertiesToString(CraftingRecipeDictionary craftingRecipe) {
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
    /// On Click event listener for each object craft 
    /// </summary>
    /// <param name="objUISlot">The current UI slot inside a single category crafting UI</param>
    private void CraftObject(Image objUISlot, int categoryIndex) {
        // remove all the objects from the crafting requirements of the current object
        string[] craftingRecipeRequirements = objUISlot.transform.Find("CraftingRecipeText").GetComponent<TextMeshProUGUI>().text.Split('\n');
        foreach (string req in craftingRecipeRequirements) {
            // req= "itemName [itemQty]"
            string itemName = req.Substring(0, req.IndexOf("[")).Trim(); // Extract item name
            int itemQty = int.Parse(req.Substring(req.IndexOf("[") + 1, req.IndexOf("]") - req.IndexOf("[") - 1)); // Extract item quantity
            InventorySystem.Instance.RemoveFromInventory(itemName, itemQty);
        }
        Debug.Log("Items removed from the inventory");
        // add the current object to the inventory
        InventorySystem.Instance.AddToInventory(objUISlot.transform.Find("ItemName").GetComponent<TextMeshProUGUI>().text);
        // check again all the requirements for each object in the current category UI
        CheckAllCraftingRecipesInCategory(categoryIndex);
    }


    /// <summary>
    /// Attaches the onClick event listener to each button in the crafting menu UI
    /// </summary>
    private void ConfigCategoryButtonsFromMenu() {
        // instantiate one button for each category as child of craftingMenuUI
        int categoryCount = Enum.GetValues(typeof(ItemCategory)).Length;
        for (int i = 0; i < categoryCount; i++) {
            GameObject currCategoryButton = Instantiate(Resources.Load<GameObject>("CraftingSystemPrefabs/openCategoryCraftingMenu"), craftingMenuUI.transform);
            Button button = currCategoryButton.GetComponent<Button>();
            currCategoryButton.GetComponentInChildren<TextMeshProUGUI>().text = ((ItemCategory)i).ToString();
            int categoryIdx = i; // capture the current index
            button.onClick.AddListener(() => OpenCategoryCraftingUI(categoryIdx));
            currCategoryButton.gameObject.SetActive(availableCraftableInventoryItemsByCategory.ContainsKey((ItemCategory)i));
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
