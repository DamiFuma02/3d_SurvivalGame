using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovements : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    [Tooltip("The camera must face the same direction of the face of the player")]
    public bool lockCameraDirection = false;

    [Tooltip("Required if lockCameraDirection == true")]
    public Camera firstPersonCamera;

    float _xRotation = 0f;
    float _yRotation = 0f;
    float _mouseHorizontalMovement;
    float _mouseVerticalMovement;
    

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (!InventorySystem.Instance.isOpen && !MenuManager.Instance.isMenuOpen) {
            // Mouse X: rotate player (left right) along Y axis 
            _mouseHorizontalMovement = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
            _xRotation -= _mouseVerticalMovement;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f); // Set the rotation range in [-90;90] degrees
            gameObject.transform.rotation = Quaternion.Euler(0f, _yRotation, 0f);
            
            // Mouse Y: rotate camera (up down) along X axis
            _mouseVerticalMovement = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;
            _yRotation += _mouseHorizontalMovement;
            firstPersonCamera.transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0f);


            //if (lockCameraDirection) {
            //    // this does rotate the player container along with the shape and the camera
            //    gameObject.transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0f);
            //} else {
            //    // this does only rotate the camera inside the player container leaving the player shape unchanged
            //    // this creates problems in the keyboard movements when the player local coords are different from camera coords
            //    // the axis in the player and camera are positioned differently
            //    firstPersonCamera.transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0f);
            //}
        }


    }
}
