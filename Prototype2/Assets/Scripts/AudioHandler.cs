using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHandler : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
