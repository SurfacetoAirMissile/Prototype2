using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraScript : MonoBehaviour
{
    public GameObject player;
    
    public Dictionary<string, Vector3> cameraPositions;

    public float cameraStopwatch;

    public float cameraForceConstant;

    public enum CameraStates
    {
        CAMERA_STATIONARY,
        CAMERA_MOVING
    }

    public CameraStates currentState = CameraStates.CAMERA_STATIONARY;

    public Vector3 cameraMoveTarget;

    void Awake()
    {
        Vector3 startPosition = transform.position;
        Vector3 position1 = new Vector3(-5.0F, 4.0F, -10.0F);
        Vector3 position2 = new Vector3(5.0F, 7.0F, -10.0F);
        cameraPositions = new Dictionary<string, Vector3>
        {
            { "Starting Position", startPosition },
            { "Position 1", position1 },
            { "Position 2", position2 }
        };
    }

    // Start is called before the first frame update
    void Start()
    {
        //TeleportCameraTo(cameraPositions["Starting Position"]);
    }

    // Update is called once per frame
    void Update()
    {
        cameraStopwatch += Time.deltaTime;

        switch (currentState)
        {
            case CameraStates.CAMERA_STATIONARY:
                
                break;
            case CameraStates.CAMERA_MOVING:
                PushCameraTowards(cameraMoveTarget);
                if (this.transform.position == cameraMoveTarget)
                {
                    currentState = CameraStates.CAMERA_STATIONARY;
                }
                break;
            default:
                break;
        }

        if (player.transform.position.x < -5.0F)
        {
            MoveCameraTo(cameraPositions["Position 1"]);
        }
        else if (player.transform.position.x > 5.0F)
        {
            MoveCameraTo(cameraPositions["Position 2"]);
        }

        LookAtPlayer();
    }
    
    // Applies a force to the camera in the direction of the position _target
    private void PushCameraTowards(Vector3 _target)
    {
        // Makes sure that we're not already at the target.
        if (transform.position != _target)
        {
            // Calculates the difference vector between the camera and _target.
            Vector3 difference = _target - transform.position;

            // Calculates the distance between the camera and _target.
            float distance = difference.magnitude;

            // if the distance is less that 0.05 units
            if (distance < 0.05F)
            {
                // Sets the camera's position to the target.
                TeleportCameraTo(_target);

                // Holds the camera still.
                GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
            else
            {
                // Fixes the distance between 0.1F and 2.0F.
                float clampedDistance = Mathf.Clamp(distance * 2.0F, 0.1F, 5.0F);

                // Calculates a force to move the camera by.
                Vector3 force = Vector3.Normalize(difference) * clampedDistance * cameraForceConstant;

                // Applies the force to the camera.
                //mainCamera.GetComponent<Rigidbody>().AddForce(force);
                GetComponent<Rigidbody>().AddForce(force);
            }
        }
    }

    private void TeleportCameraTo(Vector3 _target)
    {
        transform.position = _target;
    }

    private void MoveCameraTo(Vector3 _target)
    {
        // need to define where we're going
        cameraMoveTarget = _target;
        currentState = CameraStates.CAMERA_MOVING;
    }

    private void LookAt(Vector3 _target)
    {
        // Turns the camera to _target
        transform.LookAt(_target);
    }

    private void LookAtPlayer()
    {
        // Turns the camera to _target
        transform.LookAt(player.transform.position);
    }
}
