using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class ButtonHandler : MonoBehaviour
{
    public enum ButtonType
    {
        PLAY,
        CLOSE,
        RETURN
    }

    public ButtonType button;

    private void OnMouseDown()
    {
        switch (button)
        {
            case ButtonType.PLAY:
                {
                    SceneManager.LoadScene("Level Design 1");

                    break;
                }

            case ButtonType.CLOSE:
                {
                    Application.Quit();

                    break;
                }

            case ButtonType.RETURN:
                {
                    SceneManager.LoadScene("menu");

                    break;
                }

            default:
                break;
        }
    }
}
