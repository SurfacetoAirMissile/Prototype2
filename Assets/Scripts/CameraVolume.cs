using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraVolume : MonoBehaviour
{
    [SerializeField] private GameObject camera;

    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        string otherTag = other.gameObject.tag;
        if (otherTag == "Player")
        {
            camera.GetComponent<CinemachineVirtualCamera>().Priority += 2;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        string otherTag = other.gameObject.tag;
        if (otherTag == "Player")
        {
            camera.GetComponent<CinemachineVirtualCamera>().Priority -= 2;
        }
    }
}
