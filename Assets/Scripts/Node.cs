using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Node : MonoBehaviour
{
    public GameObject[] nodeList;
    public List<Node> visibleNodes;
    public float costSoFar;
    public float heuristic;
    public float total;
    public Node previousNode;

    void Start()
    {
        nodeList = GameObject.FindGameObjectsWithTag("Node");
        findVisibleNodes();
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
                if (hit.collider.tag == "Node" && hit.collider.name == nodeList[i].name)
                {
                    visibleNodes.Add(nodeList[i].GetComponent<Node>());

                    if (SceneManager.GetActiveScene().name == "Astar Demo")
                    {
                        //Debug.DrawRay(transform.position, rayDirection, Color.green, 300);
                    }
                }
            }
        }
    }
}
