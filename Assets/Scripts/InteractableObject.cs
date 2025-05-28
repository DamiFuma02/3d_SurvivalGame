using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable] public class DropItemsDictionary : SerializableDictionary<string, int> { }


public class InteractableObject : MonoBehaviour {
    public string ItemName;
    public ItemCategory itemCategory = ItemCategory.CraftingItem;
    public bool playerInRange;
    public bool pickable = true;
    public DropItemsDictionary drops = new DropItemsDictionary();
    
    public int health = 10;

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
        if (!InventorySystem.Instance.isOpen && playerInRange && Input.GetKeyDown(KeyCode.Mouse0) && SelectionManager.instance.lookingAtTarget && SelectionManager.instance.selectedObject == gameObject) {
            
            if (pickable) {
                InventorySystem.Instance.AddToInventoryOrDropItems(ItemName,itemCategory,drops);
                Destroy(gameObject);
            } else {
                health--;
                if (health<=0) {
                    InventorySystem.Instance.AddToInventoryOrDropItems(ItemName,itemCategory,drops);
                    Destroy(gameObject);

                }
            }
        }
        //if (health <= 0) {
        //    // drop the objects on the ground as 3d objects
        //    foreach (var drop in drops) {
        //        for (int i = 0; i < drop.Item2; i++) {
        //            GameObject droppedItem = Instantiate(Resources.Load<GameObject>(drop.Item1), 
        //                transform.position,  // current gameObject position
        //                Quaternion.identity);  
        //        }
        //    }
        //    Destroy(gameObject);
        //}
    }

}