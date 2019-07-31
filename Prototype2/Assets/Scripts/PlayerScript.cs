using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    // Public Variables
    public Vector3 velocity = new Vector3(0, 0, 0);
    public float xSpeed = 0.0f;
    public float zSpeed = 0.0f;
    public float jumpForce = 300.0f;
    public float speed = 10.0f;
    public float drag = 2.0f;

    // Private Variables
    

    private void FixedUpdate()
    {
        // Manual Drag (X and Z axis)
        Vector3 vel = this.GetComponent<Rigidbody>().velocity;
        vel.x *= (0.98f / drag);
        vel.z *= (0.98f / drag);
        this.GetComponent<Rigidbody>().velocity = vel;

        // Jump
        if (Input.GetKey(KeyCode.Space))
        {
            Jump();
        }

        // Horiztonal movement
        Movement();
    }

    void Jump()
    {
        // If touching the ground (Colliding with ground collider)
        if (this.GetComponent<BoxCollider>().bounds.Intersects(GameObject.Find("Floor").GetComponent<BoxCollider>().bounds))
        {
            // Add jump force
            this.GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce);
        }
    }

    // Controls player movement
    void Movement()
    {
        xSpeed = Input.GetAxisRaw("Horizontal");
        zSpeed = Input.GetAxisRaw("Vertical");

        velocity = new Vector3(xSpeed, 0.0f, zSpeed);

        this.GetComponent<Rigidbody>().AddForce(velocity.normalized * speed, ForceMode.Acceleration);
    }
}
