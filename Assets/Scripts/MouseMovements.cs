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
        // lock the cursor to the middle of the screen and make it invisible
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        _mouseHorizontalMovement = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
        _mouseVerticalMovement = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotation along X axis to change the view up and down
        _xRotation -= _mouseVerticalMovement;
        // Set the rotation range in [-90;90] degrees
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);
        // Rotation along Y axis to change the view left and right
        _yRotation += _mouseHorizontalMovement;

        if (lockCameraDirection) {
            // this does rotate the player container along with the shape and the camera
            gameObject.transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0f);
        } else {
            // this does only rotate the camera inside the player container leaving the player shape unchanged
            // this creates problems in the keyboard movements when the player local coords are different from camera coords
            // the axis in the player and camera are positioned differently
            firstPersonCamera.transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0f);
        }


    }
}
