using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{

    public static SelectionManager instance { get; set; }

    // method to make SelectionManager a Singleton
    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(gameObject);
        } else {
            instance = this;
        }
    }



    InteractableObject interactableObject;
    public GameObject interaction_Info_UI;
    TextMeshProUGUI interaction_text;
    Ray lightRay;
    RaycastHit hittenObj;
    Transform hittenObjectTransform;

    public bool lookingAtTarget = false;

    private void Start() {
        // get the text component inside the interaction_Info_UI GameObject
        interaction_text = interaction_Info_UI.GetComponent<TMPro.TextMeshProUGUI>();
    }

    void Update() {
        lightRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(lightRay, out hittenObj)) {
            hittenObjectTransform = hittenObj.transform;
            interactableObject = hittenObjectTransform.GetComponent<InteractableObject>();
            // check if the object hitten is an InteractableObject
            if (interactableObject && interactableObject.playerInRange) {
                lookingAtTarget = true;
                interaction_text.text = hittenObjectTransform.GetComponent<InteractableObject>().GetItemName();
                if (interactableObject.pickable) {
                    interaction_text.text += "\n(left click to pick up)";
                }
                interaction_Info_UI.SetActive(true);
            }
            else {
                interaction_Info_UI.SetActive(false);
                lookingAtTarget = false;
            }

        }
        else {
            interaction_Info_UI.SetActive(false);
            lookingAtTarget = false;
        }
    }
}
