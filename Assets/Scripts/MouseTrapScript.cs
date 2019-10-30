using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTrapScript : MonoBehaviour
{
    // Public
    /// <summary>
    /// If the player was killed by a trap
    /// </summary>
    public static bool killedByTrap = false;

    // Serialized
    [SerializeField] Mesh trapReadyModel;
    [SerializeField] Mesh trapSprungModel;

    // Audio
    [SerializeField] AudioSource snapSfx;

    // Private variables
    /// <summary>
    /// Mouse trap is active and is able to be triggered
    /// </summary>
    bool isActive = true;

    void OnCollisionEnter(Collision collision)
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

    void TriggerTrap()
    {
        this.GetComponent<MeshFilter>().mesh = trapSprungModel;

        isActive = false;

        //Destroy(this.gameObject);
    }
}
