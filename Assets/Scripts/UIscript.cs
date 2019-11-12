using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class UIscript : MonoBehaviour
{
    // Serialized
    [SerializeField] Sprite[] sprites;

    PlayerScript player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
    }

    private void FixedUpdate()
    {
        if (player.cheeseHeld)
        {
            this.GetComponent<Image>().sprite = sprites[18];
        }
        else
        {
            this.GetComponent<Image>().sprite = sprites[0];
        }
    }
}
