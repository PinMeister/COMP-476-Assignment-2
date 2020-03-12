using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Player : MonoBehaviour
{
    Animator animator;

    Vector3 destination;
    Vector3 target;
    Vector3 velocity;
    float distance;
    public float speed;
    public float maxSpeed;
    public float targetRadius;
    public float rotationDegreesPerSecond;

    public bool arrive;
    public bool flee;
    public bool wander;
    public bool moving;

    UnityEngine.AI.NavMeshAgent navMeshAgent;

    PovGraph povGraph;
    public List<Node> pathList;
    public Node startNode;
    public Node goalNode;
    public Cluster startCluster;
    public Cluster goalCluster;

    void Start()
    {
        //targetRadius = 20;
        rotationDegreesPerSecond = 360;
        animator = GetComponent<Animator>();

        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        povGraph = GameObject.Find("PoV Nodes").GetComponent<PovGraph>();
        destination = transform.position;
        target = transform.position;
        moving = false;
    }

    void Update()
    {
        animator.SetFloat("Blend", speed / maxSpeed);

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
                    moving = true;
                }
            }
        }

        if (moving)
        {
            if ((startNode.transform.position - transform.position).magnitude < 1 && pathList.Contains(startNode))
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
        /*if (this.tag == "Tagged")
        {
            Target("Not Tagged");
            maxSpeed = 5;
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
            transform.GetChild(2).gameObject.SetActive(false);
            transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
        else if (this.tag == "Frozen")
        {
            speed = 0;
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(true);
            transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
        else
        {
            Target("Frozen");
            maxSpeed = 1;
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(false);
            transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }

    
        // Kinematic flee
        if (flee)
        {
            Vector3 velocity = transform.position - destination;
            velocity.Normalize();
            velocity *= maxSpeed;
            speed = maxSpeed;
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(velocity), rotationDegreesPerSecond * Time.deltaTime);
        }

        // Kinematic wander
        if (wander && this.tag != "Frozen")
        {
            destination = new Vector3(Random.Range(0, 50), 0, Random.Range(0, 50));
            velocity = destination - transform.position;
            velocity.Normalize();
            velocity *= maxSpeed;
            speed = maxSpeed;
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(velocity), rotationDegreesPerSecond * Time.deltaTime);
        }
    }

    // Find target depending on tag
    void Target(string target)
    {
        // Use kinematic arrive for chosen target with appropriate tag
        foreach (GameObject opponent in GameObject.FindGameObjectsWithTag(target))
        {
            if (Vector3.Distance(transform.position, opponent.transform.position) < targetRadius)
            {
                distance = Vector3.Distance(transform.position, opponent.transform.position);
                destination = opponent.transform.position;
                arrive = true;
                flee = false;
                wander = false;
            }
            else
            {
                arrive = false;
                flee = false;
                wander = true;
            }
        }

        // Flee from tagged character if they are close enough
        if (this.tag == "Not Tagged")
        {
            GameObject taggedOpponent = GameObject.FindGameObjectWithTag("Tagged");
            if (Vector3.Distance(transform.position, taggedOpponent.transform.position) < targetRadius)
            {
                distance = Vector3.Distance(transform.position, taggedOpponent.transform.position);
                destination = taggedOpponent.transform.position;
                arrive = false;
                flee = true;
                wander = false;
            }
            else
            {
                arrive = false;
                flee = false;
                wander = true;
            }
        }
    }

    // Change tags on collision
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Not Tagged" && this.tag == "Tagged")
        {
            collision.tag = "Frozen";
            arrive = false;
            flee = false;
            wander = true;
            collision.GetComponent<Player>().arrive = false;
            collision.GetComponent<Player>().flee = false;
            collision.GetComponent<Player>().wander = false;
        }
        if (collision.tag == "Frozen" && this.tag == "Not Tagged")
        {
            collision.gameObject.tag = "Not Tagged";
            arrive = false;
            flee = false;
            wander = true;
            collision.GetComponent<Player>().arrive = false;
            collision.GetComponent<Player>().flee = false;
            collision.GetComponent<Player>().wander = true;
        }
    }*/
    }

    public void SetDestination(Vector3 location)
    {
        if (povGraph.navMesh)
        {
            navMeshAgent.SetDestination(location);
        }
        else if (povGraph.dijkstra || povGraph.euclidean || povGraph.cluster)
        {
            if (!pathList.Any())
            {
                povGraph.createPath(this.transform.position, location);
                pathList = povGraph.pathList;
                startNode = pathList[0];
                goalNode = pathList[pathList.Count - 1];
                startCluster = startNode.GetComponentInParent<Cluster>();
                goalCluster = goalNode.GetComponentInParent<Cluster>();
                destination = pathList[0].transform.position;
            }
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Node")
        {
            if (pathList.Count() == 1)
            {
                pathList.Remove(collider.GetComponent<Node>());
                destination = target;
            }
            else
            {
                pathList.Remove(collider.GetComponent<Node>());
                destination = pathList[0].transform.position;
            }
        }
    }
}
