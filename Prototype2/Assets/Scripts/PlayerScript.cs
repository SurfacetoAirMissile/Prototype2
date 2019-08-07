using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    public GameObject animatorChild;
    public Animator animator;

    // Public Variables
    public Vector3 velocity = new Vector3(0, 0, 0);
    public float xSpeed = 0.0f;
    public float zSpeed = 0.0f;
    public float drag = 2.0f;
    public int foodMeter = 0;

    // Sounds
    public AudioSource munchSfx;

    // Private Variables
    private float misnomer = 0.0f;
    private int winCondition = 1;
    // Movement speeds
    private float speed = 0.0f; // Current speed
    private const float wSpeed = 25.0f; // Walk speed
    private const float sSpeed = 50.0f; // Sprint speed
    // Jump Heights
    private float jumpForce = 150.0f;
    private float highJumpForce = 200.0f;
    // Rotation speeds
    private float rSpeed = 5.0f;

    private void Awake()
    {
        animator = animatorChild.GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        // Manual Drag (X and Z axis)
        Vector3 vel = this.GetComponent<Rigidbody>().velocity;
        vel.x *= (0.98f / drag);
        vel.z *= (0.98f / drag);
        this.GetComponent<Rigidbody>().velocity = vel;

        // Update distance to ground
        misnomer = this.GetComponent<CapsuleCollider>().bounds.extents.y;

        // Jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        // Sprint
        if (Input.GetKey(KeyCode.LeftShift))
        {
            // Sprinting
            speed = sSpeed;
        }
        else
        {
            // Walking
            speed = wSpeed;
        }

        /*
        // DEBUG
        if (Input.GetKey(KeyCode.K))
        {
            GameObject.Find("SoundManager").GetComponent<AudioHandler>().PlayCatDeath();
            SceneManager.LoadScene("lose");
        }
        */

        // Horiztonal movement
        MovementV2();
    }

    private void Jump()
    {
        // If touching the ground
        if (Physics.Raycast(this.transform.position, -Vector3.up, misnomer + 0.1f))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                // High Jump
                this.GetComponent<Rigidbody>().AddForce(Vector3.up * highJumpForce);
            }
            else
            {
                // Normal Jump
                this.GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce);
            }
        }
    }

    private void MovementV2()
    {
        xSpeed = Input.GetAxisRaw("Horizontal") * rSpeed;
        zSpeed = Input.GetAxisRaw("Vertical");

        this.transform.Rotate(new Vector3(0.0f, xSpeed, 0.0f));

        this.GetComponent<Rigidbody>().AddForce(this.transform.forward * zSpeed * speed);
    }

    // When collided with trigger object
    private void OnTriggerEnter(Collider other)
    {
        // Collided with cheese
        if (other.tag == "Cheese")
        {
            // Destroy collectable
            Destroy(other.gameObject);

            foodMeter += 2;

            munchSfx.Play();
        }

        // Collided with bread
        if (other.tag == "Bread")
        {
            // Destroy the bread
            Destroy(other.gameObject);

            foodMeter += 1;

            munchSfx.Play();
        }

        if (other.tag == "CameraZone")
        {
            GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
            camera.GetComponent<CameraScript>().UpdatePlayerPosition(other.gameObject.GetComponent<BoxCollider>(), true);
        }

        // When player reaches home
        if (other.tag == "Home")
        {
            // Check score for win
            CheckWin();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "CameraZone")
        {
            GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
            camera.GetComponent<CameraScript>().UpdatePlayerPosition(other.gameObject.GetComponent<BoxCollider>(), false);
        }
    }

    private void CheckWin()
    {
        // If had more than a certain amount of food
        if (foodMeter >= winCondition)
        {
            SceneManager.LoadScene("win");
        }
        else
        {
            Dead();
        }
    }

    public void Dead()
    {
        SceneManager.LoadScene("lose");
    }
}
