using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{

    public static SelectionManager instance { get; set; }
    public GameObject selectedObject;
    InteractableObject interactableObject;
    public GameObject interaction_Info_UI;
    public GameObject UI_Pointer;
    private GameObject DefaultPointer;
    private GameObject CustomPointer;   
    private GameObject ObjectInfoUI;   
    private GameObject ObjectLogo;   
    private GameObject ObjectName;   
    private GameObject ObjectHealthBarCanvas;   
    Ray lightRay;
    RaycastHit hittenObj;
    Transform hittenObjectTransform;

    public bool lookingAtTarget = false;
    public string selectionManager2dIconsDirectory = "2D_Prefabs/SelectionManagerIcons/";

    // method to make SelectionManager a Singleton
    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(gameObject);
        }
        else {
            instance = this;
        }
    }







    private void Start() {
        // get the text component inside the interaction_Info_UI GameObject
        DefaultPointer = UI_Pointer.transform.Find("DefaultPointer").gameObject;
        DefaultPointer.SetActive(true);

        CustomPointer = UI_Pointer.transform.Find("CustomPointer").gameObject;
        CustomPointer.SetActive(true);

        ObjectInfoUI = UI_Pointer.transform.Find("ObjectInfoUI").gameObject;
        ObjectInfoUI.SetActive(false);

        ObjectLogo = ObjectInfoUI.transform.Find("ObjectLogo").gameObject;
        ObjectName = ObjectInfoUI.transform.Find("ObjectName").gameObject;
        ObjectHealthBarCanvas = ObjectInfoUI.transform.Find("ObjectHealthBarCanvas").gameObject;
    }

    GameObject currSprite;
    void Update() {
        lightRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(lightRay, out hittenObj)) {
            hittenObjectTransform = hittenObj.transform;
            interactableObject = hittenObjectTransform.GetComponent<InteractableObject>();
            // check if the object hitten is an InteractableObject
            if (interactableObject && interactableObject.playerInRange && !InventorySystem.Instance.isOpen && !MenuManager.Instance.isMenuOpen) {
                lookingAtTarget = true;
                UpdateCurrentObjectInfoUI(interactableObject);
                ObjectInfoUI.SetActive(true);
                selectedObject = interactableObject.gameObject;
                ShowPointer(interactableObject);
            }
            else {
                ObjectInfoUI.SetActive(false);
                DefaultPointer.SetActive(true);
                CustomPointer.SetActive(false);
                lookingAtTarget = false;
            }

        }
        else {
            ObjectInfoUI.SetActive(false);
            DefaultPointer.SetActive(true);
            CustomPointer.SetActive(false);
            lookingAtTarget = false;
        }
    }

    private void UpdateCurrentObjectInfoUI(InteractableObject currentInteractableObject) {
        ObjectName.GetComponent<TextMeshProUGUI>().text = currentInteractableObject.ItemName;
        currSprite = Resources.Load<GameObject>(InventorySystem.Instance.inventory2dIconsDirectory + currentInteractableObject.ItemName);
        ObjectLogo.GetComponent<Image>().overrideSprite = currSprite.transform.Find("ItemImage").gameObject.GetComponent<Image>().sprite;
        // if the interactableObject isn't pickable by hand, show the healthbar
        if (!currentInteractableObject.pickableByHand) {
            ObjectHealthBarCanvas.gameObject.SetActive(true);
            PlayerDynamicBarsSystem.Instance.UpdateBarUI(BarType.Health, currentInteractableObject.currHealth, currentInteractableObject.initialHealth, player: false);
        } else {
            ObjectHealthBarCanvas.gameObject.SetActive(false);
        }

    }

    private void ShowPointer(InteractableObject interactableObject) {
        Sprite customIcon;
        string categoryString = "";
        if (InventorySystem.Instance.equippedItemFlag) {
            InventoryItem equippedInventoryItem = InventorySystem.Instance.inventoryItems[InventorySystem.Instance.equippedPlayerBarIdx];
            ItemCategory equippedItemCategory = equippedInventoryItem.category;
            if (interactableObject.pickableByHand) {
                categoryString = "hand";
            } else {
                if (equippedItemCategory == ItemCategory.Tool) {
                    if (equippedInventoryItem.categoryProperties["maxBreakableMaterial"] >= (int)interactableObject.itemMaterial) {
                        categoryString = equippedItemCategory.ToString().ToLower();
                    }
                }
            }
            
        } else {
            if (interactableObject.pickableByHand) {
                categoryString = "hand";
            }
        }
        if (categoryString != "") {
            customIcon = Resources.Load<Sprite>(selectionManager2dIconsDirectory + categoryString);
            DefaultPointer.SetActive(false);
            CustomPointer.GetComponent<Image>().overrideSprite = customIcon;
            CustomPointer.SetActive(true);
        }
        else {
            // no pointer required
            DefaultPointer.SetActive(true);
            CustomPointer.SetActive(false);
        }

    }

    


}
