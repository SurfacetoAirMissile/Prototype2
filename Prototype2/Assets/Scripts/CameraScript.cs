using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraScript : MonoBehaviour
{
    private enum CameraStates
    {
        CAMERA_MOVING,
        CAMERA_TRACKING
    }

    public Camera mainCamera;

    public float cameraStopwatch;

    public float cameraForceConstant;

    public Dictionary<string, Vector3> cameraPositions;

    void Awake()
    {
        cameraPositions = new Dictionary<string, Vector3>();
        Vector3 startPosition = new Vector3(0.0F, 0.0F, -10.0F);
        Vector3 position1 = new Vector3(3.0F, 0.0F, -10.0F);
        Vector3 position2 = new Vector3(0.0F, 5.0F, -10.0F);
        cameraPositions.Add("Starting Position", startPosition);
        cameraPositions.Add("Position 1", position1);
        cameraPositions.Add("Position 2", position2);
    }

    // Applies a force to the camera in the direction of the position _target
    private void PushCameraTowards(Vector3 _target)
    {
        Vector3 difference = _target - mainCamera.transform.position;
        float distance = difference.magnitude;
        float clampedDistance = Mathf.Clamp(distance, 0.2F, 1.0F);
        Vector3 force = difference * clampedDistance * cameraForceConstant;
        mainCamera.GetComponent<Rigidbody>().AddForce(force);
    }

    private void TeleportCameraTo(Vector3 _target)
    {
        mainCamera.GetComponent<Transform>().position.Set(_target.x, _target.y, _target.z);
    }

    // Start is called before the first frame update
    void Start()
    {
        TeleportCameraTo(cameraPositions["Starting Position"]);
    }

    // Update is called once per frame
    void Update()
    {
        cameraStopwatch += Time.deltaTime;


        /*
        if (cameraStopwatch > 2.0F)
        {
            if (mainCamera.transform.position != cameraPositions["Position 1"])
            {
                PushCameraTowards(cameraPositions["Position 1"]);
            }
        }
        */
    }
}
