using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour {
    public string ItemName;
    public bool playerInRange;
    public bool pickable = true;

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
        if (pickable && playerInRange && Input.GetKeyDown(KeyCode.Mouse0) && SelectionManager.instance.lookingAtTarget && SelectionManager.instance.selectedObject == gameObject) {
            if (!InventorySystem.Instance.CheckFull()) {
                InventorySystem.Instance.AddToInventory(ItemName);
                Destroy(gameObject);
            }
            else {
                Debug.Log("Can't pickup the item because the inventory is full");
            }
        }
    }



}