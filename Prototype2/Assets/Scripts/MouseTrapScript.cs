using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTrapScript : MonoBehaviour
{
    // Public variables
    public Mesh trapReadyModel;
    public Mesh trapSprungModel;
    public static bool killedByTrap = false;

    // Audio
    public AudioSource snapSfx;

    // Private variables
    private bool isActive = true;

    private void OnCollisionEnter(Collision collision)
    {
        if (isActive)
        {
            if (collision.collider.tag == "Rat")
            {
                // Collide with rat
                snapSfx.Play();

                collision.collider.GetComponent<ratScript>().Killed();

                TriggerTrap();
            }
            if (collision.collider.tag == "Player")
            {
                // Collider with player
                killedByTrap = true;
                collision.collider.GetComponent<PlayerScript>().Dead();
                TriggerTrap();
            }
        }
    }

    private void TriggerTrap()
    {
        this.GetComponent<MeshFilter>().mesh = trapSprungModel;

        isActive = false;

        Destroy(this.gameObject);
    }
}
