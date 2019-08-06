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

    public int currentZone = 0;

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
        zones = new Dictionary<int, BoxCollider>();
        for (int i = 0; i < zoneManagerObject.transform.childCount; i++)
        {
            BoxCollider zoneI = zoneManagerObject.transform.GetChild(i).gameObject.GetComponent<BoxCollider>();
            zones.Add(i, zoneI);
        }
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
            { 4, new Vector3(-4.7F, 1.2F, -2.5F) },
            { 5, new Vector3(-4.7F, 1.0F, -1.5F) },
            { 6, new Vector3(-3.3F, 1.0F, -0.35F) },
            { 7, new Vector3(-4.0F, 1.1F, -3.2F) },
            { 8, new Vector3(-1.0F, 1.5F, -3.2F) },
            { 9, new Vector3(-0.2F, 0.8F, -2.4F) },
            { 10, new Vector3(-0.2F, 1.7F, -2.8F) },
            { 11, new Vector3( 2.45F, 1.7F, -2.4F) },
            { 12, new Vector3( 4.98F, 0.92F, -2.57F) },
            { 13, new Vector3( 4.68F, 0.92F, -2.57F) },
            { 14, new Vector3( 4.28F, 2.27F, -2.68F) },
            { 15, new Vector3( 2.96F, 1.27F, -3.0F) },
            { 16, new Vector3( 3.28F, 1.55F, 0.3F) },
            { 17, new Vector3(-3.8F, 2.6F, -3.0F) },
            { 18, new Vector3(-3.66F, 1.79F, -4.08F) },
            { 19, new Vector3(-1.69F, 1.84F, -2.38F) },
            { 20, new Vector3(4.87F, 1.52F, -2.91F) },
            { 21, new Vector3(-5.16F, 1.74F, -1.44F) },
            { 22, new Vector3(-4.75F, 1.67F, -4.07F) },
            { 23, new Vector3(-0.35F, 0.69F, -3.07F) },
            { 24, new Vector3(-5.97F, 1.66F, -4.05F) }
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
