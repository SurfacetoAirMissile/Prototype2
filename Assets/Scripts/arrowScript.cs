using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arrowScript : MonoBehaviour
{
    GameObject home;
    GameObject player;

    void Awake()
    {
        home = GameObject.Find("HomeTrigger");
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        // Player is holding some cheese
        if (player.GetComponent<PlayerScript>().cheeseHeld)
        {
            // Point towards home
            Vector3 pointDirection = Vector3.up;
            Vector3 vectorTowardsHome = home.transform.position - transform.position;
            Vector3 newPointDirection = Vector3.RotateTowards(pointDirection, vectorTowardsHome, 2.0F * Mathf.PI, 0.0F);
            transform.up = newPointDirection;
        }
        // Player is not holding cheese
        else
        {
            // Find nearest cheese
            GameObject[] cheeses = GameObject.FindGameObjectsWithTag("Cheese");
            float nearestDist = 1000.0f;
            GameObject nearestCheese = cheeses[0];
            for (uint i = 0; i < cheeses.Length; i++)
            {
                float cheeseDist = (cheeses[i].transform.position - this.transform.position).magnitude;
                if (cheeseDist < nearestDist) { nearestDist = cheeseDist; nearestCheese = cheeses[i]; }
            }

            // Point towards nearest cheese
            Vector3 pointDir = Vector3.up;
            Vector3 towardsCheese = nearestCheese.transform.position - transform.position;
            Vector3 newPointDir = Vector3.RotateTowards(pointDir, towardsCheese, 2.0f * Mathf.PI, 0.0f);
            transform.up = newPointDir;
        }
    }
}
