using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseScreenHandler : MonoBehaviour
{
    // Public variables


    // Audio
    public AudioSource snapSfx;
    public AudioSource munchSfx;

    // Start is called before the first frame update
    void Start()
    {
        if (MouseTrapScript.killedByTrap)
        {
            snapSfx.Play();
            MouseTrapScript.killedByTrap = false;
        }
        if (PlayerScript.killedByRat)
        {
            munchSfx.Play();
            PlayerScript.killedByRat = false;
        }
    }
}
