using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    // Public Variables
    public Vector3 velocity = new Vector3(0, 0, 0);
    public float xSpeed = 0.0f;
    public float zSpeed = 0.0f;
    public float jumpForce = 300.0f;
    public float drag = 2.0f;
    public GameObject cheesePrefab;
    public int foodMeter = 0;

    // Private Variables
    private float misnomer = 0.0f;
    private int winCondition = 1;
    // Movement speeds
    private float speed = 0.0f; // Current speed
    private const float wSpeed = 50.0f; // Walk speed
    private const float sSpeed = 100.0f; // Sprint speed
    // Rotation speeds
    private float rSpeed = 5.0f;

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
        if (Input.GetKey(KeyCode.Space))
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

        // Horiztonal movement
        //Movement();
        MovementV2();
    }

    private void Jump()
    {
        // If touching the ground
        if (Physics.Raycast(this.transform.position, -Vector3.up, misnomer + 0.1f))
        {
            // Jump
            this.GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce);
        }
    }

    // Controls player movement
    private void Movement()
    {
        xSpeed = Input.GetAxisRaw("Horizontal");
        zSpeed = Input.GetAxisRaw("Vertical");

        velocity = new Vector3(xSpeed, 0.0f, zSpeed);

        this.GetComponent<Rigidbody>().AddForce(velocity.normalized * speed, ForceMode.Acceleration);

        // If moving to some capacity
        if (velocity != new Vector3(0.0f, 0.0f, 0.0f))
        {
            this.transform.rotation = Quaternion.LookRotation(velocity);
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
        }

        // Collided with bread
        if (other.tag == "Bread")
        {
            // Destroy the bread
            Destroy(other.gameObject);

            foodMeter += 1;
        }

        // When player reaches home
        if (other.tag == "Home")
        {
            // Check score for win
            CheckWin();
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
