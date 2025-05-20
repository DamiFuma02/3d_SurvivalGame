using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; set; }

    public GameObject inventoryScreenUI;
    public bool isOpen = false;


    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        }
        else {
            Instance = this;
        }
    }


    void Start() {
        isOpen = false;
    }


    void Update() {

        if (Input.GetKeyDown(KeyCode.E) && !isOpen) {

            Debug.Log("E is pressed");
            inventoryScreenUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            isOpen = true;

        }
        else if (Input.GetKeyDown(KeyCode.E) && isOpen) {
            inventoryScreenUI.SetActive(false);
            isOpen = false;
        }
    }
}
