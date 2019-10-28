using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class UIscript : MonoBehaviour
{
    // Public sprites
    public Sprite[] sprites;

    public void UpdateSprites(uint newFoodNum)
    {
        // Less than full
        if (newFoodNum <= 18)
        {
            // Change sprite
            this.GetComponent<Image>().sprite = sprites[newFoodNum];
        }
        else
        {
            this.GetComponent<Image>().sprite = sprites[18];
        }
    }
}
