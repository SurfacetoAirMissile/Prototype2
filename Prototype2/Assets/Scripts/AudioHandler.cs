using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class AudioHandler : MonoBehaviour
{
    // Public variables
    public AudioSource spottedDeath;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void PlayCatDeath()
    {
        spottedDeath.Play();
    }
}
