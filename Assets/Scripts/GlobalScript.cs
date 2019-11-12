using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalScript : MonoBehaviour
{
    uint night = 1;

    public uint GetNight() { return night; }

    public void IncrementNight() { night++; }

    public void ResetNight() { night = 1; }
}
