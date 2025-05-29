using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[Serializable] public class DropItemsDictionary : SerializableDictionary<string, int> { }


public class InteractableObject : MonoBehaviour {
    public string ItemName;
    public ItemCategory itemCategory;
    public bool playerInRange;
    // if true, the object can be picked up by hand otherwise it can only be broken with a tool
    //  with the brakeableMaterial property matching the current itemMaterial
    public bool pickableByHand = true;
    public DropItemsDictionary drops = new DropItemsDictionary();
    public ItemMaterial itemMaterial = ItemMaterial.NoMaterial; // default material

    public int initialHealth = 10;
    public int currHealth;


    private void OnTriggerEnter(Collider collidedObject) {
        playerInRange = collidedObject.CompareTag("Player");
    }

    private void OnTriggerExit(Collider collidedObject) {
        playerInRange = false;
    }


    private void Start() {
        currHealth = initialHealth;
        // assert that all items in the drops dictionary have a positive quantity
        if (drops.Count == 0) {
            return;   
        }
        foreach (string key in drops.Keys.ToList()) {
            drops[key] = Mathf.Max(1, drops[key]); // ensure at least 1 item is dropped
        }

    }

    private void Update() {
        // pickup an object with left click
        if (playerInRange && !InventorySystem.Instance.isOpen &&  Input.GetKeyDown(KeyCode.Mouse0) && SelectionManager.instance.lookingAtTarget && SelectionManager.instance.selectedObject == gameObject) {
            if (InventorySystem.Instance.equippedItemFlag) {
                Debug.Log("Start Coroutine");
                StartCoroutine(InventorySystem.Instance.AnimateEquippedItem(KeyCode.Mouse0));
            }
            if (pickableByHand) {
                InventorySystem.Instance.AddToInventoryOrDropItems(ItemName, itemCategory, drops);
                Destroy(gameObject);
            } else {
                // if the object is not pickable by hand, it can be damaged and finally broken with a tool or weapon
                if (InventorySystem.Instance.equippedItemFlag) {
                    int equippedPlayerBarIdx = InventorySystem.Instance.equippedPlayerBarIdx;
                    InventoryItem equippedInventoryItem = InventorySystem.Instance.inventoryItems[equippedPlayerBarIdx];
                    TakeDamage(equippedInventoryItem);
                }
                PlayerDynamicBarsSystem.Instance.UpdateBarUI(BarType.Health, currHealth, initialHealth,player:false);
                if (currHealth <= 0) {
                    InventorySystem.Instance.AddToInventoryOrDropItems(ItemName,itemCategory,drops);
                    Destroy(gameObject);
                }
            }
        }
    }

    private void TakeDamage(InventoryItem equippedInventoryItem) {
        int damageBonus = 1; // default damage bonus
        if (equippedInventoryItem.category == ItemCategory.Tool) {
            // check if the equipped tool can break the current object
            if (equippedInventoryItem.categoryProperties["maxBreakableMaterial"] >= (int)itemMaterial) {
                // STAB: Same Type Attack Bonus
                if (equippedInventoryItem.categoryProperties["maxBreakableMaterial"] == (int)itemMaterial) {
                    damageBonus = 2;
                }
                // take damage
                currHealth -= equippedInventoryItem.categoryProperties["damage"] * damageBonus;
                // reduce the durability of the tool more if the object is not of the same breakable material type
                equippedInventoryItem.categoryProperties["durability"] -= 2/damageBonus;
                if (equippedInventoryItem.categoryProperties["durability"] <= 0) {
                    // remove the tool from the inventory
                    InventorySystem.Instance.RemoveFromInventory(equippedInventoryItem.itemName, 1);
                }
            }
            else {
                Debug.Log("The equipped tool cannot break this object.");
            }
        }
        else if (equippedInventoryItem.category == ItemCategory.Weapon) {
            if (itemMaterial == ItemMaterial.NoMaterial) {
                // take damage
                currHealth -= equippedInventoryItem.categoryProperties["damage"];
                equippedInventoryItem.categoryProperties["durability"] -= 1;
            }
        } // else {no damage inflicted with ItemCategory.Consumable}
    }
}