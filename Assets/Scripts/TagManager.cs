using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TagManager : MonoBehaviour
{
    // Randomly select tagged character
    void Start()
    {/*
        if (SceneManager.GetActiveScene().name != "Astar Demo")
        {
            int index = Random.Range(0, 2);
            if (index == 0)
            {
                GameObject.FindGameObjectsWithTag("Not Tagged")[0].tag = "Tagged";
                GameObject.FindGameObjectsWithTag("Not Tagged")[0].tag = "NPC 1";
                GameObject.FindGameObjectsWithTag("Not Tagged")[0].tag = "NPC 2";
            }
            if (index == 1)
            {
                GameObject.FindGameObjectsWithTag("Not Tagged")[1].tag = "Tagged";
                GameObject.FindGameObjectsWithTag("Not Tagged")[0].tag = "NPC 1";
                GameObject.FindGameObjectsWithTag("Not Tagged")[0].tag = "NPC 2";
            }
            if (index == 2)
            {
                GameObject.FindGameObjectsWithTag("Not Tagged")[2].tag = "Tagged";
                GameObject.FindGameObjectsWithTag("Not Tagged")[0].tag = "NPC 1";
                GameObject.FindGameObjectsWithTag("Not Tagged")[0].tag = "NPC 2";
            }
        }*/
    }
}
