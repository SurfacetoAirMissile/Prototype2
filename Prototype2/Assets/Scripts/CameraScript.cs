using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraScript : MonoBehaviour
{
    public GameObject player;
    public GameObject zoneManagerObject;
    
    public Dictionary<int, Vector3> cameraPositions;
    public Dictionary<int, BoxCollider> zones;
    public Dictionary<int, bool> playerIsInZone;

    public int currentZone = -1;

    public float cameraStopwatch;

    public float cameraForceConstant;

    public enum CameraStates
    {
        CAMERA_STATIONARY,
        CAMERA_MOVING
    }

    public CameraStates currentState = CameraStates.CAMERA_STATIONARY;

    public Vector3 cameraMoveTarget;
    public int zoneTarget;

    void Awake()
    {
        //zones = new Dictionary<int, BoxCollider>();
        playerIsInZone = new Dictionary<int, bool>();
        for (int i = 0; i < zones.Count; i++)
        {
            playerIsInZone.Add(i, false);
        }
        cameraPositions = new Dictionary<int, Vector3>
        {
            { 0, new Vector3(-6.5F, 1.0F, -9.0F) },
            { 1, new Vector3( 0.0F, 2.0F, -9.0F) },
            { 2, new Vector3(-4.9F, 1.0F, -6.0F) },
            { 3, new Vector3(-5.8F, 1.2F, -4.0F) },
        };
    }

    // Start is called before the first frame update
    void Start()
    {
        TeleportCameraTo(cameraPositions[0]);
    }

    // Update is called once per frame
    void Update()
    {
        cameraStopwatch += Time.deltaTime;



        switch (currentState)
        {
            case CameraStates.CAMERA_STATIONARY:
                int playerZone = GetPlayerZone();
                if (currentZone != playerZone)
                {
                    MoveCameraToZone(playerZone);
                }
                break;
            case CameraStates.CAMERA_MOVING:
                if (PushCameraTowards(GetCurrentCameraPosition()))
                {
                    currentState = CameraStates.CAMERA_STATIONARY;
                    currentZone = zoneTarget;
                }
                break;
            default:
                break;
        }

        // Every frame the camera turns to look at the player
        LookAtPlayer();
    }
    
    // Applies a force to the camera in the direction of the position _target
    private bool PushCameraTowards(Vector3 _target)
    {
        bool returnValue = false;
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
                //TeleportCameraTo(_target);

                // Holds the camera still.
                //GetComponent<Rigidbody>().velocity = Vector3.zero;
                returnValue = true;
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
        return returnValue;
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
    private void MoveCameraToZone(int _zone)
    {

        cameraMoveTarget = cameraPositions[_zone];
        zoneTarget = _zone;
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

    public void UpdatePlayerPosition(BoxCollider zone, bool isInZone)
    {
        for (int i = 0; i < zones.Count; i++)
        {
            if (zones[i] == zone)
            {
                playerIsInZone[i] = isInZone;
            }
        }
    }

    private int GetPlayerZone()
    {
        for (int i = 0; i < zones.Count; i++)
        {
            if (playerIsInZone[i])
            {
                return i;
            }
        }
        return 0;
    }

    private Vector3 GetCurrentCameraPosition()
    {
        return cameraPositions[GetPlayerZone()];
    }
}
