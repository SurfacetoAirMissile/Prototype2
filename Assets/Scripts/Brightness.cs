using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamepadInput;

public class Brightness : MonoBehaviour
{
    [Range(0.98f, 1.0f)]
    public float intensity = 0.99f;

    [SerializeField]
    private Color lightestCol;
    [SerializeField]
    private Color darkestCol;

    GamePad.Button lightUp = GamePad.Button.B;
    GamePad.Button lightDown = GamePad.Button.X;
    GamePad.Button stopScript = GamePad.Button.A;

    private bool canChange = true;

    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(this);

        lightestCol = new Color(72, 136, 128, -3);
        darkestCol = new Color(0, 0, 0, -10);
    }

    // Update is called once per frame
    void Update()
    {
        intensity = Mathf.Clamp(intensity, 0.98f, 1.0f);

        RenderSettings.ambientLight = Color.Lerp(lightestCol, darkestCol, intensity);

        if (canChange)
        {
            if (GamePad.GetButtonDown(lightUp, GamePad.Index.One))
            {
                intensity += 0.001f;
            }

            if (GamePad.GetButtonDown(lightDown, GamePad.Index.One))
            {
                intensity -= 0.001f;
            }
        }


        if (GamePad.GetButtonDown(stopScript, GamePad.Index.One))
        {
            canChange = false;
        }
    }
}
