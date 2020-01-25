using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenEdgeHandler : MonoBehaviour
{

    public Transform[] screenEdges = new Transform[4];


    // Start is called before the first frame update
    void Start()
    {
        foreach(var edge in screenEdges)
        {
            edge.localScale = new Vector3(edge.localScale.x, 100, edge.localScale.z);
            var renderer = edge.GetComponent<MeshRenderer>();
            renderer.enabled = false;
        }
    }
}
