using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    GameObject[] nodeList;
    public List<GameObject> visibleNodes;

    // Start is called before the first frame update
    void Start()
    {
        nodeList = GameObject.FindGameObjectsWithTag("Node");
        findVisibleNodes();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void findVisibleNodes()
    {
        RaycastHit hit;
        Ray edge;

        for (int i = 0; i < nodeList.Length; i++)
        {
            Vector3 rayDirection = nodeList[i].transform.position - transform.position;

            if (rayDirection != Vector3.zero)
            {
                edge = new Ray(transform.position, rayDirection);
                Physics.Raycast(edge, out hit);
                if (hit.collider.tag == "Node" && hit.collider.gameObject.name == nodeList[i].name)
                {
                    visibleNodes.Add(nodeList[i]);
                    Debug.DrawRay(transform.position, rayDirection, Color.green, 300);
                }
            }
        }
    }
}
