﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatScript : MonoBehaviour
{
    // Public variables
    public GameObject pointA;
    public GameObject pointB;

    // Private variables
    private const float turnSpeed = 2.0f; // Speed the cat turns
    private GameObject followPoint;
    private const float speed = 25.0f;
    private Vector3 velocity = new Vector3(0.0f, 0.0f, 0.0f);
    private const float angle = 5.0f;

    // Timer
    private float timerMax = 5.0f; // In seconds
    private float waitTimer = 0.0f;
    
    private enum State
    {
        TURNING, // Is stationary and turning
        WALKING, // Is walking in a straight line to next point
        WAITING, // Is sitting still watching for a mouse/rat
        HUNTING // Is chasing player (For Sam?)
    }
    private State catState = State.WALKING;

    // Start function
    private void Start()
    {
        // Set default follow point
        followPoint = pointA;

        // Set waiting timer
        waitTimer = timerMax;
    }

    // Update function
    private void FixedUpdate()
    {
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

                    break;
                }

            default:
                break;
        }
    }

    // Swaps points to follow
    private void SwapPoints()
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
