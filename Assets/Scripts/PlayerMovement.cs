using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance { get; set; }


    public CharacterController characterController;

    public Transform firstPersonCamera;
    Vector3 velocityVector = Vector3.zero;
    public float speed = 2f;
    public float gravity = -9.81f;   // downwards acceleration gravity
    public float jumpHeight = 1f;

    public bool isGrounded;


    float x;
    float z;

    Vector3 move;

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(gameObject);
        }
        else {
            instance = this;
        }
    }



    // Update is called once per frame
    void Update()
    {
        // checking if it the ground
        isGrounded = characterController.isGrounded;
        if (isGrounded && velocityVector.y < 0) {
            velocityVector.y = 0f;  // upwards velocity
        }
        // read keyboard movements
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");
        // define the move with a Vector3 based on 
        // the first person camera local coordinates and looking direction
        move = (gameObject.transform.right*x) + (gameObject.transform.forward*z);
        move = Vector3.ClampMagnitude(move, 1f);  // Prevents faster diagonal movements normalizing the magnitude 


        // check if the player is on the ground so it can jump
        if (Input.GetButtonDown("Jump") && isGrounded) {
            StartCoroutine(Jump());    
        }
        // Apply gravity 
        velocityVector.y += gravity * Time.deltaTime;
        // Combine Horizontal and Vertical movements
        move = (move * speed) + (velocityVector.y * Vector3.up);
        characterController.Move(move*Time.deltaTime);
    }

    private IEnumerator Jump() {
        Debug.Log("JUMPING");
        // jumping equation
        velocityVector.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        // move the character and then wait 2secs before falling down
        characterController.Move(velocityVector*Time.deltaTime);
        yield return new WaitForSeconds(2f);
        
    }
}
