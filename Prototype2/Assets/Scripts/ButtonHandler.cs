using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class ButtonHandler : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Level Design 1");
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("menu");
    }
}
