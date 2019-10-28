using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arrowScript : MonoBehaviour
{
    GameObject home;

    void Awake()
    {
        home = GameObject.Find("HomeTrigger");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pointDirection = Vector3.up;
        Vector3 vectorTowardsHome = home.transform.position - transform.position;
        Vector3 newPointDirection = Vector3.RotateTowards(pointDirection, vectorTowardsHome, 2.0F * Mathf.PI, 0.0F);
        transform.up = newPointDirection;

    }
}
