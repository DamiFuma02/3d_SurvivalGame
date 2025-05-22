using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using NUnit.Framework;
using System.Linq;
using NUnit.Framework.Interfaces;
using System;

public class CraftingSystem : MonoBehaviour
{
    // SINGLETON CraftingSystem
    public static CraftingSystem instance { get; set; }

    // Item list retrieved from the InventorySystem
    public List<string> inventoryItemList;

    // Crafting Menu 
    public GameObject craftingMenuUI;
    public List<Button> categoryButtons;
    // empty GameObject parent for all the different categories UIs
    public GameObject craftCategoriesScreenUIParent;
    public List<string> availableCategories = new List<string> { "tools" };
    // for each Category there is a CraftingScreenUI 
    public List<Image> craftCategoryUIScreens;
    // inside each single category crafting UI screen there are
    // multiple craftable objects with a single UI (button and recipe) for each object
    public List<List<Image>> craftObjectUISlots;
    // for each Image there is one button and one Text with the crafting recipe
    public List<List<Button>> craftObjectButtons = new List<List<Button>>();
    public List<List<TextMeshProUGUI>> objectsCraftRecipes = new List<List<TextMeshProUGUI>>();
    





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
        GetInventoryItems();
        GetCategoriesCraftingUIScreens();
        GetCategoryButtonsFromMenu();
        
    }

    private void GetInventoryItems() {
        inventoryItemList = InventorySystem.Instance.itemNamesList;
    }

    

    private void GetCategoriesCraftingUIScreens() {
        Image[] allImages = craftCategoriesScreenUIParent.transform.GetComponentsInChildren<Image>();
        // check if the images have the right tag
        int validImgsCount = 0;
        foreach (Image img in allImages) {
            if (img.gameObject.CompareTag("UI_CategoryCraft")) {
                int currIdx = validImgsCount;
                GetCategoryCraftingScreenChildComponents(img, currIdx);
                craftCategoryUIScreens.Add(img);
                validImgsCount++;  
            }
        }
        Assert.AreEqual(availableCategories.Count, craftCategoryUIScreens.Count);
    }

    private void GetCategoryCraftingScreenChildComponents(Image categoryUIScreen, int categoryIndex) {
        // set the title of the current Screen as the name of the category
        categoryUIScreen.transform.Find("TitleText").GetComponent<TextMeshProUGUI>().text = "Craft "+availableCategories[categoryIndex];
        // get all the objects crafting UI slot (tag CraftItemUI)
        Image[] allObjsCraftingUI = categoryUIScreen.transform.GetComponentsInChildren<Image>();
        List<Button> allObjsButtons = new List<Button>();
        List<TextMeshProUGUI> allObjsRecipes = new List<TextMeshProUGUI>();
        int validImgCount = 0;
        foreach (Image objUI in allObjsCraftingUI) {
            // check if the objUI as the CraftItemUI tag
            if (objUI.gameObject.CompareTag("CraftItemUI")) {
                Button objCraftButton = objUI.transform.GetComponentInChildren<Button>();
                objCraftButton.gameObject.SetActive(false);// the button will be unvisible by default
                int currBtnIdx = validImgCount;
                // craft an object with the name of the categoryUIScreen ItemText Component
                objCraftButton.onClick.AddListener(() => CraftObject(objUI, categoryIndex));
                allObjsButtons.Add(objCraftButton);
                TextMeshProUGUI objCraftingRecipe = objUI.transform.Find("CraftingRecipeText").GetComponent<TextMeshProUGUI>();
                allObjsRecipes.Add(objCraftingRecipe);
                validImgCount++;
            }
        }
        craftObjectButtons.Add(allObjsButtons);
        objectsCraftRecipes.Add(allObjsRecipes);
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
        CategoryCheckAllCraftingRecipes(categoryIndex);
    }


    /// <summary>
    /// when Start() retrieves all the buttons inside crafting menu UI
    /// </summary>
    private void GetCategoryButtonsFromMenu() {
        Button[] allButtons = craftingMenuUI.transform.GetComponentsInChildren<Button>();
        // check if the buttons have the right tag   
        int validBtnsCount = 0;
        foreach (Button btn in allButtons) {
            if (btn.gameObject.CompareTag("categoryButton")) {
                int currCatIdx = validBtnsCount;
                btn.onClick.AddListener(() => OpenCategoryCraftingUI(currCatIdx));
                categoryButtons.Add(btn);
                validBtnsCount++;
            }
        }
        Assert.AreEqual(availableCategories.Count, categoryButtons.Count);
    }

    /// <summary>
    /// Click Event Listener for each button in the crafting menu UI
    /// responsable for activating the right category crafting UI
    /// </summary>
    /// <param name="categoryIdx"></param>
    void OpenCategoryCraftingUI(int categoryIdx) {
        // enables the parent and this categoryCraftingUISlot 
        craftCategoriesScreenUIParent.SetActive(true);
        craftCategoryUIScreens[categoryIdx].gameObject.SetActive(true);
        // disables craftingMenuUI
        craftingMenuUI.SetActive(false);
        CategoryCheckAllCraftingRecipes(categoryIdx);
    }

    /// <summary>
    /// Enable/Disable all single objects crafting button after opening the category crafting UI 
    /// and checking all the requirements for each object 
    /// </summary>
    /// <param name="categoryIndex"></param>
    private void CategoryCheckAllCraftingRecipes(int categoryIndex) {
        GetInventoryItems();  // update inventory items
        List<Button> objsCraftButtons = craftObjectButtons[categoryIndex];
        List<TextMeshProUGUI> categoryRecipes = objectsCraftRecipes[categoryIndex];
        for (int i = 0; i < categoryRecipes.Count; i++) {
            Debug.Log(categoryRecipes[i].text);
            string[] requirements = categoryRecipes[i].text.Split('\n');
            bool validReqs = true;
            foreach (string req in requirements) {
                // req= "itemName [itemQty]"
                string itemName = req.Substring(0, req.IndexOf("[")).Trim(); // Extract item name
                int itemQty = int.Parse(req.Substring(req.IndexOf("[")+1, req.IndexOf("]") - req.IndexOf("[") - 1)); // Extract item quantity
                // check if there are enough items in the inventory
                if (!InventorySystem.Instance.CheckQuantity(itemName,itemQty)) {
                    validReqs = false;
                    break;
                }
            }
            objsCraftButtons[i].gameObject.SetActive(validReqs);
        }
    }


    

    
}
