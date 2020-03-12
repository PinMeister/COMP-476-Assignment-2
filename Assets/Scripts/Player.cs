using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    Animator animator;

    Vector3 destination;
    Vector3 target;
    Vector3 velocity;
    float distance;
    public float speed;
    public float maxSpeed;
    public float rotationDegreesPerSecond;

    public bool moving;

    UnityEngine.AI.NavMeshAgent navMeshAgent;

    PovGraph povGraph;
    public List<Node> pathList;
    GameObject[] clusterArray;
    public List<GameObject> clusterList;
    public List<GameObject> individualList;
    public Node startNode;
    public Node goalNode;
    public GameObject startCluster;
    public GameObject goalCluster;

    void Start()
    {
        rotationDegreesPerSecond = 360;
        animator = GetComponent<Animator>();

        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        povGraph = GameObject.Find("PoV Nodes").GetComponent<PovGraph>();
        destination = transform.position;
        target = transform.position;
        moving = false;

        clusterArray = GameObject.FindGameObjectsWithTag("Cluster");

        foreach (GameObject cluster in clusterArray)
        {
            clusterList.Add(cluster);
        }

        individualList = clusterList;
    }

    void Update()
    {
        animator.SetFloat("Blend", speed / maxSpeed);

        if (SceneManager.GetActiveScene().name == "Astar Demo")
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit mouseHit;

                if (Physics.Raycast(mouseRay, out mouseHit))
                {
                    if (mouseHit.collider.tag == "Floor")
                    {
                        target = mouseHit.point;
                        SetDestination(mouseHit.point);
                        if (povGraph.dijkstra || povGraph.euclidean || povGraph.cluster)
                        {
                            moving = true;
                        }
                    }
                }
            }
        }

        if (moving)
        {
            if ((startNode.transform.position - transform.position).magnitude < 3 && pathList.Contains(startNode))
            {
                transform.position = startNode.transform.position;
            }

            // Kinematic arrive
            Vector3 velocity = destination - transform.position;
            if (velocity.magnitude > maxSpeed)
            {
                velocity.Normalize();
                velocity *= maxSpeed;
                speed = maxSpeed;
            }
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(velocity), rotationDegreesPerSecond * Time.deltaTime);
        }

        // Set target and change character colour based on current tag
        if (this.tag == "Tagged") //detect
        {
            maxSpeed = 2;
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
            transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            individualList.Remove(GameObject.FindGameObjectWithTag("NPC 1").GetComponent<Player>().startCluster);
            individualList.Remove(GameObject.FindGameObjectWithTag("NPC 2").GetComponent<Player>().startCluster);
            int randomIndex = Random.Range(0, 3);
            if (!moving)
            {
                GameObject selectedCluster = individualList[randomIndex];
                int randomNode = Random.Range(0, selectedCluster.transform.childCount);
                goalNode = selectedCluster.transform.GetChild(randomNode).GetComponent<Node>();
                SetDestination(goalNode.transform.position); //move
                moving = true;
            }
        }
        else if (this.tag == "NPC 1")
        {
            maxSpeed = 1;
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            if (!moving)
            {
                GameObject selectedCluster = GameObject.FindGameObjectWithTag("Tagged").GetComponent<Player>().goalNode.transform.parent.gameObject;
                int randomNode = Random.Range(0, selectedCluster.transform.childCount);
                goalNode = selectedCluster.transform.GetChild(randomNode).GetComponent<Node>();
                SetDestination(goalNode.transform.position);
                moving = true;
            }
        }
        else if (this.tag == "NPC 2")
        {
            maxSpeed = 1;
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            if (!moving)
            {
                GameObject selectedCluster = GameObject.FindGameObjectWithTag("Tagged").GetComponent<Player>().goalNode.transform.parent.gameObject;
                int randomNode = Random.Range(0, selectedCluster.transform.childCount);
                goalNode = selectedCluster.transform.GetChild(randomNode).GetComponent<Node>();
                SetDestination(goalNode.transform.position);
                moving = true;
            }
        }
    }

    private void SetDestination(Vector3 location)
    {
        if (povGraph.navMesh)
        {
            navMeshAgent.SetDestination(location);
        }
        else if (povGraph.dijkstra || povGraph.euclidean || povGraph.cluster)
        {
            if (!pathList.Any())
            {
                povGraph.createPath(this.transform.position, location, this.GetComponent<Player>());
                startCluster = startNode.transform.parent.gameObject;
                goalCluster = goalNode.transform.parent.gameObject;
                destination = pathList[0].transform.position;
            }
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Node" && !povGraph.navMesh)
        {
            if (!pathList.Any())
            {
                if (SceneManager.GetActiveScene().name == "Astar Demo")
                {
                    destination = target;
                }
                else //set new goal node
                {
                    if (this.tag == "Tagged") // detect
                    {
                        individualList = clusterList;
                        individualList.Remove(GameObject.FindGameObjectWithTag("NPC 1").GetComponent<Player>().startCluster);
                        individualList.Remove(GameObject.FindGameObjectWithTag("NPC 2").GetComponent<Player>().startCluster);
                        int randomIndex = Random.Range(0, 3);
                        GameObject selectedCluster = individualList[randomIndex];
                        int randomNode = Random.Range(0, selectedCluster.transform.childCount);
                        goalNode = selectedCluster.transform.GetChild(randomNode).GetComponent<Node>();
                        SetDestination(goalNode.transform.position); //move
                    }
                    else if (this.tag == "NPC 1")
                    {
                        GameObject selectedCluster = GameObject.FindGameObjectWithTag("Tagged").GetComponent<Player>().goalCluster;
                        int randomNode = Random.Range(0, selectedCluster.transform.childCount);
                        goalNode = selectedCluster.transform.GetChild(randomNode).GetComponent<Node>();
                        SetDestination(goalNode.transform.position);
                    }
                    else if (this.tag == "NPC 2")
                    {
                        GameObject selectedCluster = GameObject.FindGameObjectWithTag("Tagged").GetComponent<Player>().goalCluster;
                        int randomNode = Random.Range(0, selectedCluster.transform.childCount);
                        goalNode = selectedCluster.transform.GetChild(randomNode).GetComponent<Node>();
                        SetDestination(goalNode.transform.position);
                    }
                }
            }
            else
            {
                pathList.Remove(collider.GetComponent<Node>());
                destination = pathList[0].transform.position;
            }
        }
        if (collider.tag == "Tagged") // reload scene if others tag "it"
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
