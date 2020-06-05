using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/**
 * PlayerController - Script for player movement out-ofcombat and within combat
 *
 * Authors - Omar Ilyas, Ian Rosenberg
 *
 * Tutorials used: Brackys - Navmesh Tutorials - https://www.youtube.com/watch?v=CHV1ymlw-P8&t=11s
 */

public class PlayerController : PartyMember
{
    //Singleton creation
    private static PlayerController instance;
    public static PlayerController Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<PlayerController>();
            return instance;
        }
    }

    // Awake is called before start
    private void Awake()
    {       
        currentBehavior = Player_Travelling;

        gameManager = gameManager.Instance;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("ClickIndicator"))
        {
            agent.ResetPath(); //Stop agent if it hits indicator
            StartCoroutine(gameManager.Instance.ClickOff());
        }
    }

    public Vector3 GetDefaultGridSpawn()
    {
        return grid.defaultSpawn;
    }

    //Move character along path determined through ASta
}