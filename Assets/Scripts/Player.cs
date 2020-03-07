using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    Animator animator;
    
    Vector3 destination;
    Vector3 velocity;
    float distance;
    public float speed;
    public float maxSpeed;
    public float satRadius;
    public float targetRadius;
    public float timeToTarget;
    public float rotationDegreesPerSecond;

    public bool arrive;
    public bool flee;
    public bool wander;

    void Start()
    {
        satRadius = 5;
        targetRadius = 20;
        timeToTarget = 0.5f;
        rotationDegreesPerSecond = 360;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        animator.SetFloat ("Blend", speed / maxSpeed);

        //Appear on other side of arena when crossing a boundary
        if (transform.position.z > 50)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -45);
        }
        if (transform.position.z < -50)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 45);
        }
        if (transform.position.x > 50)
        {
            transform.position = new Vector3(-45, transform.position.y, transform.position.z);
        }
        if (transform.position.x < -50)
        {
            transform.position = new Vector3(45, transform.position.y, transform.position.z);
        }

        // Set target and change character colour based on current tag
        if (this.tag == "Tagged")
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

        // Kinematic arrive
        if (arrive)
        {
            Vector3 velocity = destination - transform.position;
            if (velocity.magnitude < satRadius)
            {
                speed = 0;
            }
            velocity /= timeToTarget;
            if (velocity.magnitude > maxSpeed)
            {
                velocity.Normalize();
                velocity *= maxSpeed;
                speed = maxSpeed;
            }
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(velocity), rotationDegreesPerSecond * Time.deltaTime);
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
    }
}
