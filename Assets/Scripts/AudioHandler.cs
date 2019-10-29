using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class AudioHandler : MonoBehaviour
{
    // Public variables
    [SerializeField] AudioSource spottedDeath;

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    /// <summary>
    /// Play audio if killed by cat
    /// </summary>
    public void PlayCatDeath()
    {
        spottedDeath.Play();
    }
}
