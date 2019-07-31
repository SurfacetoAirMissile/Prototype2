using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    // Public Variables
    public Vector3 velocity = new Vector3(0, 0, 0);
    public float xSpeed = 0.0f;
    public float zSpeed = 0.0f;
    public float speed = 10.0f;

    // Private Variables

    private void FixedUpdate()
    {
        Movement();
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
