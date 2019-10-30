using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatScript : MonoBehaviour
{
    // Public variables
    [SerializeField] GameObject pointA;
    [SerializeField] GameObject pointB;

    // Private variables
    const float turnSpeed = 2.0f; // Speed the cat turns
    /// <summary>
    /// Current point being followed
    /// </summary>
    GameObject followPoint;
    const float speed = 25.0f;
    Vector3 velocity = new Vector3(0.0f, 0.0f, 0.0f);
    /// <summary>
    /// Deadzone for turning (Stops it indefinitely turning
    /// </summary>
    const float angle = 5.0f;
    /// <summary>
    /// Angle that it see the player in
    /// </summary>
    const float viewAngle = 90.0f;

    // Timer
    float timerMax = 5.0f; // In seconds
    float waitTimer = 0.0f;
    
    enum State
    {
        TURNING, // Is stationary and turning
        WALKING, // Is walking in a straight line to next point
        WAITING, // Is sitting still watching for a mouse/rat
        HUNTING // Is chasing player (For Sam?)
    }
    State catState = State.WALKING;

    // Start function
    void Awake()
    {
        // Set default follow point
        followPoint = pointA;

        // Set waiting timer
        waitTimer = timerMax;
    }

    // Update function
    void FixedUpdate()
    {
        // If cat can see player
        if (CanSeePlayer())
        {
            catState = State.HUNTING;
        }

        switch (catState)
        {
            case State.WALKING:
                {
                    // Make sure cat is facing point
                    transform.LookAt(followPoint.transform);

                    // Update velocity
                    velocity = followPoint.transform.position - this.transform.position;

                    // Move towards point
                    this.GetComponent<Rigidbody>().AddForce(velocity.normalized * speed);

                    // Check if colliding with point
                    if (this.GetComponent<Collider>().bounds.Intersects(followPoint.GetComponent<BoxCollider>().bounds))
                    {
                        // Switch to next point
                        SwapPoints();

                        // Update velocity
                        velocity = followPoint.transform.position - this.transform.position;

                        // Enter turning state
                        catState = State.WAITING;
                    }

                    break;
                }

            case State.TURNING:
                {
                    // Rotate cat
                    this.transform.Rotate(new Vector3(0.0f, turnSpeed, 0.0f));

                    // Check if facing next point
                    if (Vector3.Angle(this.transform.forward, velocity) < angle)
                    {
                        // Change state
                        catState = State.WALKING;
                    }

                    break;
                }

            case State.WAITING:
                {
                    // Tick timer
                    waitTimer -= Time.fixedDeltaTime;

                    // Check timer
                    if (waitTimer <= 0.0f)
                    {
                        // Reset timer
                        waitTimer = timerMax;

                        // Change state
                        catState = State.TURNING;
                    }

                    //GetComponent<Animation>()

                    break;
                }

            case State.HUNTING:
                {
                    // Cat has seen the mouse
                    //GameObject.Find("Player").GetComponent<PlayerScript>().Dead();
                    //catState = State.WALKING;

                    GameObject oPlayer = GameObject.Find("Player");

                    // Look at player
                    transform.LookAt(oPlayer.transform);

                    // Chase player
                    velocity = oPlayer.transform.position - this.transform.position;
                    this.GetComponent<Rigidbody>().AddForce(velocity.normalized * speed);
                    
                    // Check if lost player
                    if (!CanSeePlayer())
                    {
                        catState = State.WALKING;
                    }

                    break;
                }

            default:
                break;
        }
    }

    /// <summary>
    /// Returns true if the cat can see the player, false if not
    /// </summary>
    /// <returns></returns>
    bool CanSeePlayer()
    {
        GameObject oPlayer = GameObject.Find("Player");
        RaycastHit hit;
        Vector3 diff = oPlayer.transform.position - this.transform.position;
        float distance = diff.magnitude;

        // If player is within distance of the cat
        if ((distance < 4.0f) && ((oPlayer.transform.position.y - this.transform.position.y) < 1))
        {
            // If the raycast hit something (Of course it will)
            if (Physics.Raycast(this.transform.position, diff, out hit))
            {
                // If it hit the player
                if (hit.transform == oPlayer.transform)
                {
                    // If the player is within the radius
                    float playerAngle = Vector3.Angle(oPlayer.transform.position - this.transform.position, this.transform.forward);
                    if (playerAngle < viewAngle * 0.5f)
                    {
                        // Player's been caught
                        return true;
                    }
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Swaps the point that is to be followed
    /// </summary>
    void SwapPoints()
    {
        if (followPoint == pointA)
        {
            followPoint = pointB;
        }
        else
        {
            followPoint = pointA;
        }
    }
}
