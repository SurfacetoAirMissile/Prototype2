using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ratScript : MonoBehaviour
{
    private Rigidbody body;

    public float torqueConstant;
    public float forceConstant;

    public enum ratStates
    {
        patrol,
        chase,
        dead,
        idle
    }

    public ratStates currentState;

    private const float viewAngle = 90.0f;

    public GameObject pathContainer;
    private List<Transform> path;
    int targetNode = 0;

    private void Awake()
    {
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        // state machine
        switch (currentState)
        {
            case ratStates.patrol:
                if (pathContainer != null)
                {
                    if (!MoveTowardsPosXZ(GetTargetNodePosition())) // if we're already at that point
                    {
                        // target the next node
                        targetNode++;
                        if (targetNode >= path.Count) // if the next node does not exist
                        {
                            // target the first node
                            targetNode = 0;
                        }
                    }
                }
                if (CanSeePlayer())
                {
                    currentState = ratStates.chase;
                }
                break;
            case ratStates.chase:
                SmoothMovementPosXZ(GameObject.Find("Player").transform.position);
                //if (!CanSeePlayer())
                //{
                //    currentState = ratStates.idle;
                //}
                break;
            case ratStates.idle:
                if (CanSeePlayer())
                {
                    currentState = ratStates.chase;
                }
                break;
            default:
                break;
        }


        
    }

    // This function will rotate the rat to face the position and the push the rat towards the point
    // Returns false if the rat is already at the position.
    private bool MoveTowardsPosXZ(Vector3 point)
    {
        bool returnValue = true;
        Vector2 pointXZ = new Vector2(point.x, point.z);

        Vector2 ratPositionXZ = new Vector2(transform.position.x, transform.position.z);
        Vector2 ratDirectionXZ = new Vector2(transform.forward.x, transform.forward.z);

        Vector2 direction = pointXZ - ratPositionXZ;

        Vector2 directionNormalized = direction.normalized;
        Vector2 ratDirectionXZnormalized = ratDirectionXZ.normalized;
        float distance = direction.magnitude;
        // if the rat is reasonably far away
        if (distance > 0.1F)
        {
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
                    Vector3 torque = transform.up * torqueConstant * rotationDirection * Time.deltaTime;
                    body.AddTorque(torque);
                }
            }
            else // the rat is facing the point
            {
                if (distance > 0.1F) // we aren't very close to the target
                {
                    body.AddForce(transform.forward * forceConstant * Time.deltaTime);
                }
                else // we are very close <3
                {
                    returnValue = false;
                }
            }
            
        }
        else
        {
            returnValue = false;
        }
        
        return returnValue;
    }

    private Vector3 GetTargetNodePosition()
    {
        return path[targetNode].position;
    }

    // Rat is killed
    public void Killed()
    {
        // Change state
        currentState = ratStates.dead;
    }

    // Returns false if the rat is already at the position.
    private bool SmoothMovementPosXZ(Vector3 point)
    {
        bool returnValue = true;
        RotateTowardsPosXZ(point);
        if (!VariablePushTowardsPosXZ(point))
        {
            returnValue = false;
        }
        return returnValue;
    }

    private void RotateTowardsPosXZ(Vector3 point)
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
                Vector3 torque = transform.up * torqueConstant * rotationDirection * Time.deltaTime;
                body.AddTorque(torque);
            }
        }
    }

    private bool VariablePushTowardsPosXZ(Vector3 point)
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
            body.AddForce(transform.forward * forceConstant * amount * Time.deltaTime);
        }
        else // we are very close <3
        {
            returnValue = false;
        }

        return returnValue;
    }

    private bool CanSeePlayer()
    {
        GameObject oPlayer = GameObject.Find("Player");
        RaycastHit hit;

        // If the raycast hit something (Of course it will)
        if (Physics.Raycast(this.transform.position, oPlayer.transform.position - this.transform.position, out hit))
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

        return false;
    }

    private bool PlayerInLineOfSight()
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
