using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSystem : MonoBehaviour
{
    public bool playMusic;
    public bool playEffects;
    public bool playHeartbeat;

    public float maxEnemyDist = 5.0f;
    [SerializeField] private float enemyDangerZone = 2.0f;

    [SerializeField] private Transform player;
    [SerializeField] private GameObject enemy;

    [SerializeField] private float timer = 0.0f;

    [SerializeField] private AudioSource basicAudio;
    [SerializeField] private AudioSource dangerAudio;
    [SerializeField] private AudioSource munchAudio;
    [SerializeField] private AudioSource snapAudio;
    [SerializeField] private AudioSource heartAudio;

    public bool danger;

    private float enDist = 0.0f;
    [SerializeField] private float heartMultiply = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        basicAudio.volume = 1;
        dangerAudio.volume = 0;

        if (!basicAudio.isPlaying)
        {
            basicAudio.Play(0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playMusic)
        {
            if (danger)
            {
                if (!dangerAudio.isPlaying)
                {
                    dangerAudio.Play(0);
                }

                if (dangerAudio.volume < 1)
                {
                    basicAudio.volume -= 0.5f * Time.deltaTime;
                    dangerAudio.volume += 0.5f * Time.deltaTime;
                }
                else
                {
                    basicAudio.Pause();
                }
            }
            else
            {
                if (dangerAudio.volume > 0)
                {
                    dangerAudio.volume -= 0.5f * Time.deltaTime;
                }
                else
                {
                    if (basicAudio.volume < 1)
                    {
                        basicAudio.volume += 1f * Time.deltaTime;
                        dangerAudio.Stop();

                        if (!basicAudio.isPlaying)
                        {
                            basicAudio.Play(0);
                        }
                    }

                }
            }
        }


        if (playEffects)
        {
            if (timer <= 0)
            {
                int soundChoice = (Random.Range(0, 2));

                if (soundChoice == 0)
                {
                    if (!munchAudio.isPlaying)
                    {
                        munchAudio.panStereo = Random.Range(-1.5f, 1.5f);
                        munchAudio.Play(0);
                        Debug.Log("munch");
                    }
                }
                else
                {
                    if (!snapAudio.isPlaying)
                    {
                        snapAudio.panStereo = Random.Range(-1.5f, 1.5f);
                        snapAudio.Play(0);
                        Debug.Log("snap");
                    }
                }

                timer = Random.Range(1.0f, 30.0f);
            }
            else
            {
                if (!danger)
                {
                    timer -= Time.deltaTime;
                }
            }
        }

        if (playHeartbeat)
        {
            CheckCloseDistance();

            if (enemy == null)
            {
                heartAudio.pitch = 1.0f;
            }
            else
            {
                heartMultiply = (1/enDist) * 2;
                heartMultiply = Mathf.Clamp(heartMultiply, 0.0f, 1.5f);
                

                heartAudio.pitch = 1.0f + heartMultiply;
            }
        }

        if (enDist < enemyDangerZone)
        {
            danger = true;
        }
        else
        {
            danger = false;
        }
    }

    void CheckCloseDistance()
    {
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag("Rat");

        float closeDist = Mathf.Infinity;

        for (int i = 0; i < taggedObjects.Length; i++)
        {
            if (Vector3.Distance(player.position, taggedObjects[i].transform.position) <= closeDist)
            {
                closeDist = Vector3.Distance(player.position, taggedObjects[i].transform.position);
                enemy = taggedObjects[i];
                enDist = closeDist;
            }
        }

        if (closeDist > maxEnemyDist)
        {
            enemy = null;
        }
    }
}
