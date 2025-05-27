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
    private GameObject PickUpPointer;   
    private GameObject ObjectInfoUI;   
    private GameObject ObjectLogo;   
    private GameObject ObjectName;   
    Ray lightRay;
    RaycastHit hittenObj;
    Transform hittenObjectTransform;

    public bool lookingAtTarget = false;

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

        PickUpPointer = UI_Pointer.transform.Find("PickUpPointer").gameObject;
        PickUpPointer.SetActive(true);

        ObjectInfoUI = UI_Pointer.transform.Find("ObjectInfoUI").gameObject;
        ObjectInfoUI.SetActive(false);

        ObjectLogo = ObjectInfoUI.transform.Find("ObjectLogo").gameObject;
        ObjectName = ObjectInfoUI.transform.Find("ObjectName").gameObject;
    }

    void Update() {
        lightRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(lightRay, out hittenObj)) {
            hittenObjectTransform = hittenObj.transform;
            interactableObject = hittenObjectTransform.GetComponent<InteractableObject>();
            // check if the object hitten is an InteractableObject
            if (interactableObject && interactableObject.playerInRange && !InventorySystem.Instance.isOpen) {
                lookingAtTarget = true;
                ObjectName.GetComponent<TextMeshProUGUI>().text = hittenObjectTransform.GetComponent<InteractableObject>().GetItemName();
                GameObject currSprite = Resources.Load<GameObject>("InventorySystemPrefabs/" + hittenObjectTransform.GetComponent<InteractableObject>().GetItemName());
                // disable itemproperties in the ui 
                ObjectLogo.GetComponent<Image>().overrideSprite = currSprite.transform.Find("ItemImage").gameObject.GetComponent<Image>().sprite; 
                ObjectInfoUI.SetActive(true);
                DefaultPointer.SetActive(!interactableObject.pickable);
                PickUpPointer.SetActive(interactableObject.pickable);
                selectedObject = interactableObject.gameObject;
            }
            else {
                ObjectInfoUI.SetActive(false);
                DefaultPointer.SetActive(true);
                PickUpPointer.SetActive(false);
                lookingAtTarget = false;
            }

        }
        else {
            ObjectInfoUI.SetActive(false);
            DefaultPointer.SetActive(true);
            PickUpPointer.SetActive(false);
            lookingAtTarget = false;
        }
    }
}
