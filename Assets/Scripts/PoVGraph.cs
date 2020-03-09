using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoVGraph : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private float weight(Transform currentNode, Transform adjacentNode)
    {
        return (currentNode.position - adjacentNode.position).magnitude;
    }
}
