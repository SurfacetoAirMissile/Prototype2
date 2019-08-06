using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    private void Awake()
    {
        //Dictionary<int, BoxCollider> zones = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScript>().zones;
        for (int i = 0; i < this.transform.childCount; i++)
        {
            BoxCollider zoneI = this.transform.GetChild(i).gameObject.GetComponent<BoxCollider>();
            //zones.Add(i, zoneI);
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScript>().zones.Add(i, zoneI);
        }
        //GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScript>().zones = zones;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
