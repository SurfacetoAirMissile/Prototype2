using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableCheeseScript : MonoBehaviour
{
    [SerializeField] GameObject cheesePrefab;

    float yBounds = 0.0f;

    bool destroyed = false;

    private void Awake()
    {
        yBounds = this.GetComponent<Collider>().bounds.extents.y;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Doesn't hit player
        // Hit the floor, not the roof
        if (collision.collider.tag != "Player" && Physics.Raycast(this.transform.position, -Vector3.up, yBounds * 2.0f) && !destroyed)
        {
            destroyed = true;
            Instantiate(cheesePrefab, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}
