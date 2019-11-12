using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class AudioHandler : MonoBehaviour
{
    // Public variables
    [SerializeField] AudioSource catDeath;

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    /// <summary>
    /// Play audio if killed by cat
    /// </summary>
    public void PlayCatDeath()
    {
        catDeath.Play();
    }
}
