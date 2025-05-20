using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{
    public GameObject interaction_Info_UI;
    TextMeshProUGUI interaction_text;
    Ray lightRay;
    RaycastHit hittenObj;
    Transform hittenObjectTransform;


    private void Start() {
        // get the text component inside the interaction_Info_UI GameObject
        interaction_text = interaction_Info_UI.GetComponent<TMPro.TextMeshProUGUI>();
    }

    void Update() {
        lightRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(lightRay, out hittenObj)) {
            hittenObjectTransform = hittenObj.transform;

            // check if the object hitten is an InteractableObject
            if (hittenObjectTransform.GetComponent<InteractableObject>()) {
                interaction_text.text = hittenObjectTransform.GetComponent<InteractableObject>().GetItemName();
                interaction_Info_UI.SetActive(true);
            }
            else {
                interaction_Info_UI.SetActive(false);
            }

        }
    }
}
