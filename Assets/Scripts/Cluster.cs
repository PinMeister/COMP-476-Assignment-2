using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cluster : MonoBehaviour
{
    public List<Node> clusterList;

    void Start()
    {
        foreach (Transform node in transform)
        {
            clusterList.Add(node.GetComponent<Node>());
        }
    }
}
