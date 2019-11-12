using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class ButtonHandler : MonoBehaviour
{
    GlobalScript global;

    public enum ButtonType
    {
        PLAY,
        CLOSE,
        RETURNWIN,
        RETURNLOSE
    }

    public ButtonType button;

    private void Awake()
    {
        global = GameObject.Find("GlobalObj").GetComponent<GlobalScript>();
    }

    private void OnMouseDown()
    {
        switch (button)
        {
            case ButtonType.PLAY:
                {
                    SceneManager.LoadScene("Level 1");

                    break;
                }

            case ButtonType.CLOSE:
                {
                    Application.Quit();

                    break;
                }

            case ButtonType.RETURNWIN:
                {
                    // Check night and load level accordingly
                    switch (global.GetNight())
                    {
                        case 1:
                            {
                                SceneManager.LoadScene("Level 2");

                                break;
                            }
                        case 2:
                            {
                                SceneManager.LoadScene("Level 3");

                                break;
                            }
                        case 3:
                            {
                                SceneManager.LoadScene("Level 4");

                                break;
                            }
                        case 4:
                            {
                                global.ResetNight();
                                SceneManager.LoadScene("menu");
                                break;
                            }
                        default:
                            {
                                global.IncrementNight();
                                break;
                            }
                    }

                    break;
                }

            case ButtonType.RETURNLOSE:
                {
                    SceneManager.LoadScene("menu");

                    break;
                }

            default:
                break;
        }
    }
}
