using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PovGraph : MonoBehaviour
{
    public bool navMesh;
    public bool dijkstra;
    public bool euclidean;
    public bool cluster;

    public Node startNode;
    public Node goalNode;

    public GameObject[] nodeList;
    public List<Node> openList = new List<Node>();
    public List<Node> closedList = new List<Node>();
    public List<Node> pathList = new List<Node>();

    Material fill;
    Material noFill;

    void Start()
    {
        navMesh = false;
        dijkstra = false;
        euclidean = false;
        cluster = false;
        nodeList = GameObject.FindGameObjectsWithTag("Node");
        fill = (Material)(Resources.Load("Fill"));
        noFill = (Material)(Resources.Load("Node"));
    }

    void Update()
    {
        if (Input.GetKeyDown("1"))
        {
            navMesh = true;
            dijkstra = false;
            euclidean = false;
            cluster = false;
        }

        if (Input.GetKeyDown("2"))
        {
            navMesh = false;
            dijkstra = true;
            euclidean = false;
            cluster = false;
        }

        if (Input.GetKeyDown("3"))
        {
            navMesh = false;
            dijkstra = false;
            euclidean = true;
            cluster = false;
        }

        if (Input.GetKeyDown("4"))
        {
            navMesh = false;
            dijkstra = false;
            euclidean = false;
            cluster = true;
        }

    }

    private float distance(Vector3 start, Vector3 destination)
    {
        return (start - destination).magnitude;
    }

    public List<Node> createPath(Vector3 start, Vector3 destination)
    {
        openList.Clear();
        closedList.Clear();
        pathList.Clear();

        foreach (GameObject node in nodeList)
        {
            node.GetComponent<Renderer>().material = noFill;
        }
        
        startNode = nodeList[0].GetComponent<Node>();
        goalNode = nodeList[0].GetComponent<Node>();

        startNode.costSoFar = 0;

        foreach (GameObject node in nodeList)
        {
            if (distance(start, node.transform.position) < distance(start, startNode.transform.position))
            {
                RaycastHit startHit;
                Ray startEdge;
                Vector3 startDirection = node.transform.position - start;
                startEdge = new Ray(start, startDirection);

                Physics.Raycast(startEdge, out startHit);
                if (startHit.collider.tag == "Node")
                {
                    startNode = node.GetComponent<Node>();
                }
            }

            if (distance(node.transform.position, destination) < distance(goalNode.transform.position, destination))
            {
                RaycastHit goalHit;
                Ray goalEdge;
                Vector3 goalDirection = node.transform.position - destination;
                goalEdge = new Ray(destination, goalDirection);

                Physics.Raycast(goalEdge, out goalHit);
                if (goalHit.collider.tag == "Node")
                {
                    goalNode = node.GetComponent<Node>();
                }
            }
        }

        startNode.GetComponent<Renderer>().material = fill;
        goalNode.GetComponent<Renderer>().material = fill;
        Debug.DrawRay(start, startNode.transform.position - start, Color.red, 10);
        Debug.DrawRay(destination, goalNode.transform.position - destination, Color.red, 10);

        if (dijkstra || euclidean || cluster)
        {
            openList.Add(startNode);

            if (dijkstra)
            {
                startNode.heuristic = 0;
            }

            if (euclidean || cluster)
            {
                startNode.heuristic = distance(startNode.transform.position, goalNode.transform.position);
                startNode.total = startNode.costSoFar + startNode.heuristic;
            }

            while (!closedList.Contains(goalNode))
            {
                Node currentNode = openList[0];

                foreach (Node node in openList)
                {
                    if (node.total < currentNode.total)
                    {
                        currentNode = node;
                    }
                }

                currentNode.GetComponent<Renderer>().material = fill;
                openList.Remove(currentNode);
                closedList.Add(currentNode);

                foreach (Node adjacentNode in currentNode.visibleNodes)
                {
                    bool inOpenList = false;
                    bool inClosedList = false;

                    if (closedList.Contains(adjacentNode))
                    {
                        inClosedList = true;
                    }
                    else if (openList.Contains(adjacentNode))
                    {
                        inOpenList = true;
                    }

                    float cost = (currentNode.costSoFar + distance(currentNode.transform.position, adjacentNode.transform.position));

                    if (dijkstra)
                    {
                        adjacentNode.heuristic = 0;
                    }

                    if (euclidean)
                    {
                        adjacentNode.heuristic = distance(adjacentNode.transform.position, goalNode.transform.position);
                    }

                    if (cluster)
                    {
                        adjacentNode.heuristic = distance(adjacentNode.transform.position, goalNode.transform.position) + clusterLookup(adjacentNode, goalNode);
                    }

                    if (closedList.Contains(adjacentNode) && cost < adjacentNode.costSoFar)
                    {
                        adjacentNode.costSoFar = cost;
                        adjacentNode.total = adjacentNode.costSoFar + adjacentNode.heuristic;
                        adjacentNode.previousNode = currentNode;
                        closedList.Remove(adjacentNode);
                        openList.Add(adjacentNode);
                    }

                    else if (inOpenList && cost < adjacentNode.costSoFar)
                    {
                        adjacentNode.costSoFar = cost;
                        adjacentNode.total = adjacentNode.costSoFar + adjacentNode.heuristic;
                        adjacentNode.previousNode = currentNode;
                    }

                    else if (!inClosedList && !inOpenList)
                    {
                        adjacentNode.costSoFar = cost;
                        adjacentNode.total = adjacentNode.costSoFar + adjacentNode.heuristic;
                        adjacentNode.previousNode = currentNode;
                        openList.Add(adjacentNode);
                    }
                }
            }

            pathList.Add(goalNode);

            while (true)
            {
                if (pathList[pathList.Count - 1].previousNode == startNode)
                {
                    pathList.Add(pathList[pathList.Count - 1].previousNode);
                    Debug.DrawRay(pathList[pathList.Count - 1].transform.position, pathList[pathList.Count - 2].transform.position - pathList[pathList.Count - 1].transform.position, Color.red, 10);
                    pathList.Reverse();
                    return pathList;
                }
                else
                {
                    pathList.Add(pathList[pathList.Count - 1].previousNode);
                    Debug.DrawRay(pathList[pathList.Count - 1].transform.position, pathList[pathList.Count - 2].transform.position - pathList[pathList.Count - 1].transform.position, Color.red, 10);
                }
            }
        }
        return pathList;
    }

    private float clusterLookup(Node startNode, Node goalNode)
    {
        switch (startNode.transform.parent.name)
        {
            case "Cluster":
                {
                    switch (goalNode.transform.parent.name)
                    {
                        case "Cluster (1)":
                            return distance(GameObject.Find("Node (5)").transform.position, GameObject.Find("Node (9)").transform.position);
                        case "Cluster (2)":
                            return distance(GameObject.Find("Node (3)").transform.position, GameObject.Find("Node (6)").transform.position);
                        case "Cluster (3)":
                            return distance(GameObject.Find("Node (9)").transform.position, GameObject.Find("Node (5)").transform.position)
                                + distance(GameObject.Find("Node (5)").transform.position, GameObject.Find("Node (17)").transform.position)
                                + distance(GameObject.Find("Node (17)").transform.position, GameObject.Find("Node (20)").transform.position)
                                + distance(GameObject.Find("Node (20)").transform.position, GameObject.Find("Node (21)").transform.position)
                                + distance(GameObject.Find("Node (21)").transform.position, GameObject.Find("Node (31)").transform.position);
                        case "Cluster (4)":
                            return distance(GameObject.Find("Node (6)").transform.position, GameObject.Find("Node (3)").transform.position)
                                + distance(GameObject.Find("Node (3)").transform.position, GameObject.Find("Node (11)").transform.position)
                                + distance(GameObject.Find("Node (11)").transform.position, GameObject.Find("Node (10)").transform.position);
                        case "Cluster (5)":
                            return distance(GameObject.Find("Node (9)").transform.position, GameObject.Find("Node (5)").transform.position)
                                + distance(GameObject.Find("Node (5)").transform.position, GameObject.Find("Node (4)").transform.position)
                                + distance(GameObject.Find("Node (4)").transform.position, GameObject.Find("Node (16)").transform.position)
                                + distance(GameObject.Find("Node (16)").transform.position, GameObject.Find("Node").transform.position)
                                + distance(GameObject.Find("Node").transform.position, GameObject.Find("Node (34)").transform.position);
                    }
                }
                break;
            case "Cluster (1)":
                {
                    switch (goalNode.transform.parent.name)
                    {
                        case "Cluster":
                            return distance(GameObject.Find("Node (5)").transform.position, GameObject.Find("Node (9)").transform.position);
                        case "Cluster (2)":
                            return distance(GameObject.Find("Node (4)").transform.position, GameObject.Find("Node (16)").transform.position);
                        case "Cluster (3)":
                            return distance(GameObject.Find("Node (21)").transform.position, GameObject.Find("Node (31)").transform.position);
                        case "Cluster (4)":
                            return distance(GameObject.Find("Node (4)").transform.position, GameObject.Find("Node (2)").transform.position)
                                + distance(GameObject.Find("Node (2)").transform.position, GameObject.Find("Node (11)").transform.position)
                                + distance(GameObject.Find("Node (11)").transform.position, GameObject.Find("Node (10)").transform.position);
                        case "Cluster (5)":
                            return distance(GameObject.Find("Node (21)").transform.position, GameObject.Find("Node (23)").transform.position)
                                + distance(GameObject.Find("Node (23)").transform.position, GameObject.Find("Node (24)").transform.position);
                    }
                }
                break;
            case "Cluster (2)":
                {
                    switch (goalNode.transform.parent.name)
                    {
                        case "Cluster":
                            return distance(GameObject.Find("Node (3)").transform.position, GameObject.Find("Node (6)").transform.position);
                        case "Cluster (1)":
                            return distance(GameObject.Find("Node (4)").transform.position, GameObject.Find("Node (16)").transform.position);
                        case "Cluster (3)":
                            return distance(GameObject.Find("Node").transform.position, GameObject.Find("Node (34)").transform.position)
                                + distance(GameObject.Find("Node (34)").transform.position, GameObject.Find("Node (24)").transform.position)
                                + distance(GameObject.Find("Node (24)").transform.position, GameObject.Find("Node (25)").transform.position);
                        case "Cluster (4)":
                            return distance(GameObject.Find("Node (10)").transform.position, GameObject.Find("Node (11)").transform.position);
                        case "Cluster (5)":
                            return distance(GameObject.Find("Node").transform.position, GameObject.Find("Node (34)").transform.position);
                    }
                }
                break;
            case "Cluster (3)":
                {
                    switch (goalNode.transform.parent.name)
                    {
                        case "Cluster":
                            return distance(GameObject.Find("Node (5)").transform.position, GameObject.Find("Node (9)").transform.position)
                                + distance(GameObject.Find("Node (5)").transform.position, GameObject.Find("Node (17)").transform.position)
                                + distance(GameObject.Find("Node (17)").transform.position, GameObject.Find("Node (20)").transform.position)
                                + distance(GameObject.Find("Node (20)").transform.position, GameObject.Find("Node (21)").transform.position)
                                + distance(GameObject.Find("Node (21)").transform.position, GameObject.Find("Node (31)").transform.position);
                        case "Cluster (1)":
                            return distance(GameObject.Find("Node (21)").transform.position, GameObject.Find("Node (31)").transform.position);
                        case "Cluster (2)":
                            return distance(GameObject.Find("Node").transform.position, GameObject.Find("Node (34)").transform.position)
                                + distance(GameObject.Find("Node (34)").transform.position, GameObject.Find("Node (24)").transform.position)
                                + distance(GameObject.Find("Node (24)").transform.position, GameObject.Find("Node (25)").transform.position);
                        case "Cluster (4)":
                            return distance(GameObject.Find("Node (25)").transform.position, GameObject.Find("Node (24)").transform.position)
                                + distance(GameObject.Find("Node (24)").transform.position, GameObject.Find("Node (34)").transform.position)
                                + distance(GameObject.Find("Node (34)").transform.position, GameObject.Find("Node (15)").transform.position)
                                + distance(GameObject.Find("Node (15)").transform.position, GameObject.Find("Node (14)").transform.position);
                        case "Cluster (5)":
                            return distance(GameObject.Find("Node (24)").transform.position, GameObject.Find("Node (25)").transform.position);
                    }
                }
                break;
            case "Cluster (4)":
                {
                    switch (goalNode.transform.parent.name)
                    {
                        case "Cluster":
                            return distance(GameObject.Find("Node (6)").transform.position, GameObject.Find("Node (3)").transform.position)
                                + distance(GameObject.Find("Node (3)").transform.position, GameObject.Find("Node (11)").transform.position)
                                + distance(GameObject.Find("Node (11)").transform.position, GameObject.Find("Node (10)").transform.position);
                        case "Cluster (1)":
                            return distance(GameObject.Find("Node (4)").transform.position, GameObject.Find("Node (2)").transform.position)
                                + distance(GameObject.Find("Node (2)").transform.position, GameObject.Find("Node (11)").transform.position)
                                + distance(GameObject.Find("Node (11)").transform.position, GameObject.Find("Node (10)").transform.position);
                        case "Cluster (2)":
                            return distance(GameObject.Find("Node (10)").transform.position, GameObject.Find("Node (11)").transform.position);
                        case "Cluster (3)":
                            return distance(GameObject.Find("Node (25)").transform.position, GameObject.Find("Node (24)").transform.position)
                                + distance(GameObject.Find("Node (24)").transform.position, GameObject.Find("Node (34)").transform.position)
                                + distance(GameObject.Find("Node (34)").transform.position, GameObject.Find("Node (15)").transform.position)
                                + distance(GameObject.Find("Node (15)").transform.position, GameObject.Find("Node (14)").transform.position);
                        case "Cluster (5)":
                            return distance(GameObject.Find("Node (14)").transform.position, GameObject.Find("Node (15)").transform.position);
                    }
                }
                break;
            case "Cluster (5)":
                {
                    switch (goalNode.transform.parent.name)
                    {
                        case "Cluster":
                            return distance(GameObject.Find("Node (9)").transform.position, GameObject.Find("Node (5)").transform.position)
                                + distance(GameObject.Find("Node (5)").transform.position, GameObject.Find("Node (4)").transform.position)
                                + distance(GameObject.Find("Node (4)").transform.position, GameObject.Find("Node (16)").transform.position)
                                + distance(GameObject.Find("Node (16)").transform.position, GameObject.Find("Node").transform.position)
                                + distance(GameObject.Find("Node").transform.position, GameObject.Find("Node (34)").transform.position);
                        case "Cluster (1)":
                            return distance(GameObject.Find("Node (21)").transform.position, GameObject.Find("Node (23)").transform.position)
                                + distance(GameObject.Find("Node (23)").transform.position, GameObject.Find("Node (24)").transform.position);
                        case "Cluster (2)":
                            return distance(GameObject.Find("Node").transform.position, GameObject.Find("Node (34)").transform.position);
                        case "Cluster (3)":
                            return distance(GameObject.Find("Node (24)").transform.position, GameObject.Find("Node (25)").transform.position);
                        case "Cluster (4)":
                            return distance(GameObject.Find("Node (14)").transform.position, GameObject.Find("Node (15)").transform.position);
                    }
                }
                break;
        }
        return distance(startNode.transform.position, goalNode.transform.position); //if nodes are in same cluster, behave like Euclidean
    }
}
