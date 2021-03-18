﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed = 6.0f;
    public float jumpForce = 3.0f;
    public float xRange = 10.0f;
    public float zTopRange = -4.5f;
    public float zBottomRange = -12.5f;

    public bool gameOver = false;

    private Rigidbody playerRigidBody;
    private Animator playerAnimator;
    private bool isOnGround = true;

    // Start is called before the first frame update
    void Start()
    {
        playerRigidBody = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameOver)
        {
            move();
            checkBounds();
        }
    }

    // Move the player as long as it is not at the top Z range
    void move()
    {
        // get input
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");
        // store input
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        transform.rotation = Quaternion.LookRotation(movement);
        // move player
        transform.Translate(movement * movementSpeed * Time.deltaTime, Space.World);
        // change the animation
        if (isOnGround)
        {
            bool walking = moveHorizontal != 0.0f || moveVertical != 0.0f;
            playerAnimator.SetBool("IsWalking", walking);
        }
        // check for jump input
        if (Input.GetKeyDown(KeyCode.Space) && isOnGround)
        {
            playerRigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isOnGround = false;
            // animate the jump
            playerAnimator.SetBool("IsJumping", true);
        }
    }

    // Keep the player from walking out of bounds
    void checkBounds()
    {
        // check left bound
        if (transform.position.x > xRange)
        {
            transform.position = new Vector3(xRange, transform.position.y, transform.position.z);
        }

        // check right bound
        if (transform.position.x < -xRange)
        {
            transform.position = new Vector3(-xRange, transform.position.y, transform.position.z);
        }

        // check top bound
        if (transform.position.z > zTopRange)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, zTopRange);
        }

        // check bottom bound
        if (transform.position.z < zBottomRange)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, zBottomRange);
        }
    }

    // Check for a collision
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Road"))
        {
            isOnGround = true;
            playerAnimator.SetBool("IsJumping", false);
        }

        if (collision.gameObject.CompareTag("Car"))
        {
            gameOver = true;
            Debug.Log("Game Over");

            // animate the death
            playerAnimator.SetTrigger("DeathTrigger");
            // wait 2 seconds before going to next scene
            Invoke("nextScene", 2);
        }
    }

    // Load the GameOverScene
    void nextScene()
    {
        SceneManager.LoadScene("GameOverScene");
    }
}