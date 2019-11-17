﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RatNavScript : MonoBehaviour
{
    // Public
    public enum ratStates
    {
        patrol,
        curious,
        chase,
        dead,
        idle
    }
    public ratStates currentState;

    // Serialized Field
    //[SerializeField] float torqueConstant;
    //[SerializeField] float forceConstant;
    /// <summary>
    /// AI follow path reference
    /// </summary>
    [SerializeField] GameObject pathContainer;

    // Private
    Rigidbody body;
    /// <summary>
    /// Angle in which it can view the player directly ahead of it
    /// </summary>
    const float viewAngle = 135.0f;
    List<Transform> path;
    /// <summary>
    /// Currently followed node on path index
    /// </summary>
    int targetNode = 0;

    float timeSinceSawPlayer = 0.0f;
    [SerializeField] float giveUpTime = 3.0f;

    float timeSpottingPlayer = 0.0f;
    [SerializeField] float spotChaseTime = 1.0f;

    NavMeshAgent thisAgent;

    Vector3 agentTarget;

    void Awake()
    {
        thisAgent = GetComponent<NavMeshAgent>();
        if (!thisAgent)
        {
            Debug.LogError("thisAgent returned null on " + gameObject.name);
        }        
        body = GetComponent<Rigidbody>();
        // if the rat has a path
        if (pathContainer != null)
        {
            // fetch the path
            path = new List<Transform>();
            for (int i = 0; i < pathContainer.transform.childCount; i++)
            {
                Transform nodeI = pathContainer.transform.GetChild(i);
                path.Add(nodeI);
            }
        }
    }

    // Update is called once per frame (Should be fixed)
    void Update()
    {
        bool playerInSight = CanSeePlayer();
        if (playerInSight)
        {
            Debug.DrawRay(transform.position, GameObject.Find("Player").transform.position - transform.position, Color.red);
        }
        // state machine
        switch (currentState)
        {
            case ratStates.patrol:
                if (pathContainer != null) {
                    // if the agent isn't moving, get them moving towards the target
                    if (thisAgent.isStopped) { thisAgent.isStopped = false; }

                    Vector3 distance = transform.position - agentTarget;
                    Vector2 XZdistance = new Vector2(distance.x, distance.z);
                    // if we're already at that point
                    if (XZdistance.magnitude <= 0.1f)
                    {
                        // target the next node
                        targetNode++;
                        // if the next node does not exist
                        if (targetNode >= path.Count)
                        {
                            // target the first node
                            targetNode = 0;
                        }
                    }
                    // set the target to the current node
                    agentTarget = GetTargetNodePosition();
                }
                else { currentState = ratStates.idle; }
                // if we spot the player while patrolling, start searching for the player
                if (playerInSight) {
                    currentState = ratStates.curious;
                    timeSpottingPlayer = 0.0f;
                    timeSinceSawPlayer = 0.0f;
                }
                break;
            case ratStates.curious:
                //stop the agent if it's not stopped already
                if (!thisAgent.isStopped) { thisAgent.isStopped = true; }
                // turn towards the player
                RotateTowardsPosXZ(GameObject.Find("Player").transform.position);
                // increment how long we've been spotting the player for
                if (playerInSight) { timeSpottingPlayer += Time.deltaTime; }
                else
                {
                    // decrement
                    timeSpottingPlayer -= Time.deltaTime;
                    // if the time has reached 0
                    if (timeSpottingPlayer <= 0.0f)
                    {
                        timeSpottingPlayer = 0.0f;
                        currentState = ratStates.patrol;
                    }
                }
                if (timeSpottingPlayer >= spotChaseTime) {
                    currentState = ratStates.chase;
                    agentTarget = GameObject.Find("Player").transform.position;
                }
                //if ()
                break;
            case ratStates.chase:
                if (thisAgent.isStopped) {
                    thisAgent.isStopped = false;
                    timeSinceSawPlayer = 0.0f;
                }
                if (NavMesh.SamplePosition(GameObject.Find("Player").transform.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
                {
                    agentTarget = hit.position;
                }


                //agentTarget = GameObject.Find("Player").transform.position;
                //RotateTowardsPosXZ(GameObject.Find("Player").transform.position);
                if (playerInSight)
                {
                    timeSinceSawPlayer = 0.0f;
                }
                else
                {
                    timeSinceSawPlayer += Time.deltaTime;
                }
                if (timeSinceSawPlayer >= giveUpTime)
                {
                    currentState = ratStates.patrol;
                    timeSpottingPlayer = 0.0f;
                }
                break;
            case ratStates.dead:
                thisAgent.isStopped = true;
                GetComponentInChildren<Animator>().enabled = false;
                break;
            default:
                break;
        }

        if (thisAgent)
        {
            thisAgent.SetDestination(agentTarget);
        }
    }


    /// <summary>
    /// Returns the currently followed node's position
    /// </summary>
    /// <returns></returns>
    Vector3 GetTargetNodePosition()
    {
        return path[targetNode].position;
    }

    /// <summary>
    /// Kills the rat
    /// </summary>
    public void Killed()
    {
        // Change state
        currentState = ratStates.dead;
    }

    /// <summary>
    /// Moves towards a point smoothly. Returns false if already at point.
    /// </summary>
    /// <param name="point"> Point that it needs to follow. i.e. the player </param>
    /// <returns></returns>
    bool SmoothMovementPosXZ(Vector3 point)
    {
        bool returnValue = true;
        RotateTowardsPosXZ(point);
        if (!VariablePushTowardsPosXZ(point))
        {
            returnValue = false;
        }
        return returnValue;
    }

    /// <summary>
    /// Rotates towards a point
    /// </summary>
    /// <param name="point"> The point it needs to rotate towards </param>
    void RotateTowardsPosXZ(Vector3 point)
    {
        Vector2 pointXZ = new Vector2(point.x, point.z);

        Vector2 ratPositionXZ = new Vector2(transform.position.x, transform.position.z);
        Vector2 ratDirectionXZ = new Vector2(transform.forward.x, transform.forward.z);

        Vector2 direction = pointXZ - ratPositionXZ;

        Vector2 directionNormalized = direction.normalized;
        Vector2 ratDirectionXZnormalized = ratDirectionXZ.normalized;
        // if the rat is not facing the point
        if (directionNormalized != ratDirectionXZnormalized)
        {
            // if the two are very similar, set the rotation and kill the angular velocity
            bool directionSimilar = false;
            // if x is similar
            if (directionNormalized.x > ratDirectionXZnormalized.x - 0.1F
                && directionNormalized.x < ratDirectionXZnormalized.x + 0.1F)
            {
                // if z is similar (registers as y since it's a Vector2)
                if (directionNormalized.y > ratDirectionXZnormalized.y - 0.1F
                    && directionNormalized.y < ratDirectionXZnormalized.y + 0.1F)
                {
                    directionSimilar = true;
                }
            }
            if (directionSimilar)
            {
                Vector3 temp = transform.forward;
                temp.x = direction.x;
                // once again, registers as y since Vector2
                temp.y = 0.0F;
                temp.z = direction.y;
                transform.forward = temp;
                body.angularVelocity = Vector3.zero;
            }
            else // direction is not similar.
            {
                // starts with anti-clockwise rotation
                float rotationDirection = -1.0F;
                bool flip = false;
                // intended direction.z is negative
                if (Mathf.Sign(directionNormalized.y) != 1.0F)
                {
                    directionNormalized *= -1.0F;
                    flip = !flip;
                }
                // our direction.z is negative
                if (Mathf.Sign(ratDirectionXZnormalized.y) != 1.0F)
                {
                    ratDirectionXZnormalized *= -1.0F;
                    flip = !flip;
                }
                if (flip)
                {
                    if (directionNormalized.x < ratDirectionXZnormalized.x)
                    {
                        // sets the rotation to be clockwise
                        rotationDirection = 1.0F;
                    }
                }
                else
                {
                    if (directionNormalized.x > ratDirectionXZnormalized.x)
                    {
                        // sets the rotation to be clockwise
                        rotationDirection = 1.0F;
                    }
                }

                // we need to rotate the rat towards the direciton
                Vector3 torque = transform.up * 100.0f * rotationDirection * Time.deltaTime;
                body.AddTorque(torque);
            }
        }
    }

    /// <summary>
    /// Pushes rat towards the point
    /// </summary>
    /// <param name="point"> Point that needs to be pushed towards </param>
    /// <returns></returns>
    bool VariablePushTowardsPosXZ(Vector3 point)
    {
        bool returnValue = true;
        Vector2 pointXZ = new Vector2(point.x, point.z);

        Vector2 ratPositionXZ = new Vector2(transform.position.x, transform.position.z);
        Vector2 ratDirectionXZ = new Vector2(transform.forward.x, transform.forward.z);

        Vector2 direction = pointXZ - ratPositionXZ;

        Vector2 directionNormalized = direction.normalized;
        Vector2 ratDirectionXZnormalized = ratDirectionXZ.normalized;

        float angle = Vector3.Angle(directionNormalized, Vector3.forward);
        Vector3 rotated = Vector3.RotateTowards(ratDirectionXZnormalized, Vector3.forward, Mathf.Deg2Rad * angle, 0.0F);

        float amount = rotated.z;

        float distance = direction.magnitude;
        if (distance > 0.1F) // we aren't very close to the target
        {
            body.AddForce(transform.forward * 200.0f * amount * Time.deltaTime);
            //Debug.Log(amount);
        }
        else // we are very close <3
        {
            returnValue = false;
        }

        return returnValue;
    }

    /// <summary>
    /// Returns true if the rat can see the player, false if not
    /// </summary>
    /// <returns></returns>
    bool CanSeePlayer()
    {
        GameObject oPlayer = GameObject.Find("Player");
        RaycastHit hit;

        // If the raycast hit something (Of course it will)
        if (Physics.Raycast(gameObject.transform.position, oPlayer.transform.position - gameObject.transform.position, out hit))
        {
            // If it hit the player
            if (hit.transform == oPlayer.transform)
            {
                // If the player is within the radius
                float playerAngle = Vector3.Angle(oPlayer.transform.position - gameObject.transform.position, gameObject.transform.forward);
                if (playerAngle < viewAngle * 0.5f)
                {
                    // Player's been caught
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Checks if the player is in the rat's line of sight
    /// </summary>
    /// <returns></returns>
    bool PlayerInLineOfSight()
    {
        GameObject oPlayer = GameObject.Find("Player");
        RaycastHit hit;

        // If the raycast hit something (Of course it will)
        if (Physics.Raycast(this.transform.position, oPlayer.transform.position - this.transform.position, out hit))
        {
            // If it hit the player
            if (hit.transform == oPlayer.transform)
            {
                // Player's been caught
                return true;
            }
        }

        return false;
    }

}
