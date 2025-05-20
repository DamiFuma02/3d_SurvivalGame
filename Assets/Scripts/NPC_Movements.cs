using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class NPC_Movements : MonoBehaviour
{
    Animator animator;

    public float moveSpeed = 0.2f;

    Vector3 stopPosition;

    float walkTime;
    public float walkCounter;
    float waitTime;
    public float waitCounter;

    int WalkDirection;

    public bool isWalking;

    // Start is called before the first frame update
    void Start() {
        animator = GetComponent<Animator>();

        // in order to have different walkTime and waitTime values 
        // across the different gameObjects with this script attached
        walkTime = Random.Range(3, 6);
        waitTime = Random.Range(5, 7);


        waitCounter = waitTime;
        walkCounter = walkTime;

        ChooseDirection();
    }

    // Update is called once per frame
    void Update() {
        if (isWalking) {

            animator.SetBool("isRunning", true);

            walkCounter -= Time.deltaTime;

            transform.localRotation = Quaternion.Euler(0f, 90f* WalkDirection, 0f);
            transform.position += transform.forward * moveSpeed * Time.deltaTime;

            
            if (walkCounter <= 0) {
                stopPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                isWalking = false;
                //stop movement
                transform.position = stopPosition;
                animator.SetBool("isRunning", false);
                //reset the waitCounter
                waitCounter = waitTime;
            }


        }
        else {

            waitCounter -= Time.deltaTime;

            if (waitCounter <= 0) {
                ChooseDirection();
            }
        }
    }


    public void ChooseDirection() {
        WalkDirection = Random.Range(-1, 3);

        isWalking = true;
        walkCounter = walkTime;
    }
}
