using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagManager : MonoBehaviour
{
    GameObject lastPlayer;

    public bool tagOn;

    // Randomly select tagged character
    void Start()
    {
        int index = Random.Range(0, 4);
        GameObject.FindGameObjectsWithTag("Not Tagged")[index].tag = "Tagged";
        tagOn = false;
    }

    // Last character to be frozen is the new tagged character
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            tagOn = !tagOn;
        }

        if (GameObject.FindGameObjectsWithTag("Not Tagged").Length == 1)
        {
            lastPlayer = GameObject.FindGameObjectWithTag("Not Tagged");
        }

        if (GameObject.FindGameObjectsWithTag("Frozen").Length == 4)
        {
            GameObject.FindGameObjectWithTag("Tagged").tag = "Not Tagged";
            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Frozen"))
            {
                player.tag = "Not Tagged";
            }
            lastPlayer.tag = "Tagged";
        }
    }
}
