using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManagerScipt : MonoBehaviour
{
    public List<Transform> nodes;

    void Awake()
    {
        nodes = new List<Transform>();
        for (int i = 0; i < this.transform.childCount; i++)
        {
            Transform nodeI = this.transform.GetChild(i);
            nodes.Add(nodeI);
        }
    }
}
