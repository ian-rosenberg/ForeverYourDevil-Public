using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour
{

    private static gameManager instance;

    public static gameManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<gameManager>();
            return instance;
        }
    }

    public enum STATE { TRAVELING, COMBAT, PAUSED, TALKING}

    public STATE gameState; //Current State of the game

    // Start is called before the first frame update
    void Start()
    {
        gameState = STATE.TRAVELING; //Start out of combat
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
