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
    //[SerializeField] Mesh trapReadyModel;
    //[SerializeField] Mesh trapSprungModel;

    // Audio
    [SerializeField] AudioSource snapSfx;

    // Private variables
    /// <summary>
    /// Mouse trap is active and is able to be triggered
    /// </summary>
    public bool isActive = true;

    void OnCollisionEnter(Collision collision)
    {
        if (isActive)
        {
            if (collision.collider.tag == "Rat")
            {
                // Collide with rat

                collision.collider.GetComponent<RatNavScript>().Killed();

                TriggerTrap();
            }
            //if (collision.collider.tag == "Player")
            //{
                // Collider with player
                //killedByTrap = true;
                //collision.collider.GetComponent<PlayerScript>().Dead();

                //TriggerTrap();
            //}
        }
    }

    public void TriggerTrap()
    {
        //this.GetComponent<MeshFilter>().mesh = trapSprungModel;

        snapSfx.Play();
        GetComponentInChildren<Animator>().SetTrigger("Snap");
        this.tag = "SprungTrap";
        isActive = false;

        //Destroy(this.gameObject);
    }

    public void MouseTrigger()
    {
        killedByTrap = true;
        GameObject.Find("Player").GetComponent<PlayerScript>().Dead();

        TriggerTrap();
    }
}
