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
    public GameObject cheesePrefab;

    // Private Variables
    private float misnomer = 0.0f;

    private void FixedUpdate()
    {
        // Manual Drag (X and Z axis)
        Vector3 vel = this.GetComponent<Rigidbody>().velocity;
        vel.x *= (0.98f / drag);
        vel.z *= (0.98f / drag);
        this.GetComponent<Rigidbody>().velocity = vel;

        // Update distance to ground
        misnomer = this.GetComponent<BoxCollider>().bounds.extents.y;

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
        // If touching the ground
        if (Physics.Raycast(this.transform.position, -Vector3.up, misnomer + 0.1f))
        {
            // Jump
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

    // When collided with trigger object
    private void OnTriggerEnter(Collider other)
    {
        // Collided with cheese
        if (other.tag == "Cheese")
        {
            Destroy(other.gameObject);

            Vector3 spawn = this.transform.position;
            spawn.y += 1.0f;

            GameObject newCheese = Instantiate(cheesePrefab, spawn, this.transform.rotation);
            newCheese.transform.parent = this.transform;
        }
    }
}
