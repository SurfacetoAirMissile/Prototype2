using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using GamepadInput;

public class MenuController : MonoBehaviour
{
    // Public
    public enum MenuMode
    {
        MAINMENU,
        WINSCREEN,
        LOSESCREEN,
        NONE
    }
    public MenuMode currentMenu = MenuMode.MAINMENU;
    public uint currentLevel = 1;

    // Serialized
    [SerializeField] Sprite[] playSprites; // Not selected, selected
    [SerializeField] Sprite[] continueSprites; // Not selected, selected
    [SerializeField] Sprite[] quitSprites; // Not selected, selected
    [SerializeField] Sprite[] returnSprites; // Not selected, selected

    [SerializeField] GameObject[] mainMenuObjects; // Title, play, quit
    [SerializeField] GameObject[] winScreenObjects; // Title, continue, exit
    [SerializeField] GameObject[] loseScreenObjects; // Title, exit

    // Private
    bool topItemSelected = true;
    // Stops player from spamming dpad
    float dpadTimer = 0.0f;
    float dpadTimerMax = 0.2f;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    void FixedUpdate()
    {
        if (this.GetComponent<Canvas>().worldCamera == null) { this.GetComponent<Canvas>().worldCamera = Camera.main; }

        UpdateTimer();

        // Player is in a menu, and dpad up or down is pressed
        if (currentMenu != MenuMode.NONE && DpadPressed())
        {
            topItemSelected = !topItemSelected;
            dpadTimer = dpadTimerMax;
        }

        // Update visually the selected sprite
        switch (currentMenu)
        {
            case MenuMode.MAINMENU:
                {
                    // Play button
                    mainMenuObjects[1].GetComponent<SpriteRenderer>().sprite = (topItemSelected) ? playSprites[1] : playSprites[0];
                    // Quit Button
                    mainMenuObjects[2].GetComponent<SpriteRenderer>().sprite = (topItemSelected) ? quitSprites[0] : quitSprites[1];
                    break;
                }

            case MenuMode.WINSCREEN:
                {
                    // Continue - To be changed
                    winScreenObjects[1].GetComponent<SpriteRenderer>().sprite = (topItemSelected) ? continueSprites[1] : continueSprites[0];
                    // Return
                    winScreenObjects[2].GetComponent<SpriteRenderer>().sprite = (topItemSelected) ? returnSprites[0] : returnSprites[1];
                    break;
                }

            case MenuMode.LOSESCREEN:
                {
                    // Exit - always on
                    loseScreenObjects[1].GetComponent<SpriteRenderer>().sprite = returnSprites[1];
                    break;
                }

            default:
                break;
        }

        // Check if item is selected
        // Is in a menu and A is pressed
        if (currentMenu != MenuMode.NONE && GamePad.GetButtonDown(GamePad.Button.A, GamePad.Index.One))
        {
            // Do different things according to what menu you're in plus which item is selected
            switch (currentMenu)
            {
                case MenuMode.MAINMENU:
                    {
                        // Remove the menu and start the game if play is selected, quit otherwise
                        if (topItemSelected) { ChangeMenuMode(MenuMode.NONE); }
                        else { Application.Quit(); }
                        break;
                    }

                case MenuMode.WINSCREEN:
                    {
                        // Load next level if continue selected, otherwise return to menu
                        if (topItemSelected) { LoadNextLevel(); }
                        else { ReturnToMenu(); }
                        break;
                    }

                case MenuMode.LOSESCREEN:
                    {
                        // Only one button - Return to main menu
                        ReturnToMenu();
                        break;
                    }

                default:
                    break;
            }
        }
    }

    public void ChangeMenuMode(MenuMode newMode)
    {
        // Turns all the menu items off
        RemoveAllItems();

        // Makes the top one the selected one
        topItemSelected = true;

        // Turns the corresponding menu items on
        switch (newMode)
        {
            case MenuMode.MAINMENU:
                {
                    for (uint i = 0; i < mainMenuObjects.Length; i++)
                    {
                        mainMenuObjects[i].GetComponent<SpriteRenderer>().enabled = true;
                    }
                    break;
                }

            case MenuMode.WINSCREEN:
                {
                    for (uint i = 0; i < winScreenObjects.Length; i++)
                    {
                        winScreenObjects[i].GetComponent<SpriteRenderer>().enabled = true;
                    }
                    break;
                }

            case MenuMode.LOSESCREEN:
                {
                    for (uint i = 0; i < loseScreenObjects.Length; i++)
                    {
                        loseScreenObjects[i].GetComponent<SpriteRenderer>().enabled = true;
                    }
                    break;
                }

            default:
                break;
        }

        // Sets it as the current menu item
        currentMenu = newMode;
    }

    /// <summary>
    /// Removes all the menu items for the currently selected menu
    /// </summary>
    void RemoveAllItems()
    {
        switch (currentMenu)
        {
            case MenuMode.MAINMENU:
                {
                    for (uint i = 0; i < mainMenuObjects.Length; i++)
                    {
                        mainMenuObjects[i].GetComponent<SpriteRenderer>().enabled = false;
                    }
                    break;
                }

            case MenuMode.LOSESCREEN:
                {
                    for (uint i = 0; i < loseScreenObjects.Length; i++)
                    {
                        loseScreenObjects[i].GetComponent<SpriteRenderer>().enabled = false;
                    }
                    break;
                }

            case MenuMode.WINSCREEN:
                {
                    for (uint i = 0; i < winScreenObjects.Length; i++)
                    {
                        winScreenObjects[i].GetComponent<SpriteRenderer>().enabled = false;
                    }
                    break;
                }

            default:
                break;
        }
    }

    void LoadNextLevel()
    {
        RemoveAllItems();

        switch (currentLevel)
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
                    ReturnToMenu();
                    break;
                }

            default:
                break;
        }
        currentLevel++;
        ChangeMenuMode(MenuMode.NONE);
    }

    public void ReturnToMenu()
    {
        currentLevel = 1;
        ChangeMenuMode(MenuMode.MAINMENU);
        SceneManager.LoadScene("Level 1");
        this.GetComponent<Canvas>().worldCamera = Camera.main;
    }

    bool DpadPressed()
    {
        // If timer is still ticking, return false
        if (dpadTimer > 0.0f) { return false; }

        // If button is pressed, return true
        if (GamePad.GetAxis(GamePad.Axis.Dpad, GamePad.Index.One).y == 1 ||
            GamePad.GetAxis(GamePad.Axis.Dpad, GamePad.Index.One).y == -1)
        {
            return true;
        }

        // If button is not pressed, return false
        return false;
    }

    void UpdateTimer()
    {
        if (dpadTimer == 0.0f) { return; }
        if (dpadTimer < 0.0f) { dpadTimer = 0.0f; }
        else { dpadTimer -= Time.deltaTime; }
    }
}
