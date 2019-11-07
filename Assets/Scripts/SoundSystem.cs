using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSystem : MonoBehaviour
{
    // Audio
    [SerializeField] AudioSource basicAudio;
    [SerializeField] AudioSource dangerAudio;
    [SerializeField] AudioSource munchAudio;
    [SerializeField] AudioSource snapAudio;
    [SerializeField] AudioSource heartAudio;
    
    float enDist = 0.0f;
    /// <summary>
    /// Maximum distance enemy rat can be away from player before losing danger music
    /// </summary>
    const float maxEnemyDist = 5.0f;
    float enemyDangerZone = 2.0f;
    GameObject enemy;
    float timer = 0.0f;
    bool danger = false;
    float heartMultiply = 0.0f;

    // Start is called before the first frame update
    void Awake()
    {
        basicAudio.volume = 1;
        dangerAudio.volume = 0;

        if (!basicAudio.isPlaying)
        {
            basicAudio.Play(0);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        BackgroundMusic();

        //RandomSoundEffects();

        // Play heart beat
        if (!heartAudio.isPlaying)
        {
            heartAudio.Play(0);
        }

        // If there is no enemy close, play heartbeat normally
        if (enemy == null)
        {
            heartAudio.pitch = 1.0f;
        }
        // If there is an enemy close by, change pitch depending on how close they are
        else
        {
            heartMultiply = (1/enDist) * 2;
            heartMultiply = Mathf.Clamp(heartMultiply, 0.0f, 1.5f);
                

            heartAudio.pitch = 1.0f + heartMultiply;
        }

        // Check distance
        danger = (enemy != null && enDist < enemyDangerZone);

        CheckCloseDistance();
    }

    void CheckCloseDistance()
    {
        // Get all rats
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag("Rat");

        float closeDist = Mathf.Infinity;

        // Find the closest rat
        for (int i = 0; i < taggedObjects.Length; i++)
        {
            if (Vector3.Distance(transform.position, taggedObjects[i].transform.position) <= closeDist)
            {
                closeDist = Vector3.Distance(transform.position, taggedObjects[i].transform.position);
                enemy = taggedObjects[i];
                enDist = closeDist;
            }
        }

        // If the closest rat is out of range
        if (closeDist > maxEnemyDist)
        {
            enemy = null;
        }
    }

    /// <summary>
    /// Manages background music
    /// </summary>
    void BackgroundMusic()
    {
        // Danger
        if (danger)
        {
            // Play if not currently playing
            if (!dangerAudio.isPlaying)
            {
                dangerAudio.Play();
            }

            // Crossfade into danger music
            if (dangerAudio.volume < 1)
            {
                basicAudio.volume -= 0.5f * Time.fixedDeltaTime;
                dangerAudio.volume += 0.5f * Time.fixedDeltaTime;
            }
            else
            {
                basicAudio.Pause();
            }
        }
        // Not in danger
        else
        {
            // Lower danger volume
            if (dangerAudio.volume > 0)
            {
                dangerAudio.volume -= 0.5f * Time.deltaTime;
            }
            else
            {
                // Increase normal music
                if (basicAudio.volume < 1)
                {
                    basicAudio.volume += 1.0f * Time.deltaTime;
                    dangerAudio.Stop();

                    if (!basicAudio.isPlaying)
                    {
                        basicAudio.Play(0);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Tick the timer and play random sound effects for ambient noise
    /// </summary>
    void RandomSoundEffects()
    {
        // Timer ticked
        if (timer <= 0)
        {
            int soundChoice = (Random.Range(0, 2));

            if (soundChoice == 0)
            {
                if (!munchAudio.isPlaying)
                {
                    munchAudio.panStereo = Random.Range(-1.5f, 1.5f);
                    munchAudio.Play(0);
                }
            }
            else
            {
                if (!snapAudio.isPlaying)
                {
                    snapAudio.panStereo = Random.Range(-1.5f, 1.5f);
                    snapAudio.Play(0);
                }
            }

            timer = Random.Range(1.0f, 30.0f);
        }
        else
        {
            // Tick timer if not in danger
            if (!danger)
            {
                timer -= Time.deltaTime;
            }
        }
    }
}
