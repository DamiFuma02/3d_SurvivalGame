using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour {
    public string ItemName;
    public bool playerInRange;

    public string GetItemName() {
        return ItemName;
    }

    private void OnTriggerEnter(Collider collidedObject) {
        playerInRange = collidedObject.CompareTag("Player");
    }

    private void OnTriggerExit(Collider collidedObject) {
        playerInRange = false;
    }


    private void Update() {
        // pickup an object with left click
        if (playerInRange && Input.GetKeyDown(KeyCode.F) && SelectionManager.instance.lookingAtTarget ) {
            Debug.Log("Moved " + gameObject.name + " to the inventory");
            Destroy(gameObject);
        }
    }



}